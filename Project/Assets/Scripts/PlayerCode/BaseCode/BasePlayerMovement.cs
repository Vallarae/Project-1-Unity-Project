using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Device;
using UnityEngine.UIElements;

namespace PlayerCode.BaseCode
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class BasePlayerController : MonoBehaviour
    {
        #region Variables

        [Header("Movement Settings")]
        [SerializeField] protected float walkSpeed = 3f;
        [SerializeField] protected float jumpHeight = 2f;
        protected LayerMask groundMask;

        [Space]
        [Header("Light Attacks Settings")]
        [SerializeField] protected int lightAttackDamage = 3;
        [SerializeField] protected float lightAttackStunDuration = 0.25f;
        [SerializeField] protected float lightAttackKnockbackForce = 3f;
        [SerializeField] protected float lightAttackCooldownDuration = 0.25f;
        [SerializeField] protected AudioClip lightAttackSound;

        [Space]
        [Header("Heavy Attacks Settings")]
        [SerializeField] protected int heavyAttackDamage = 5;
        [SerializeField] protected float heavyAttackHoldTime = 0.3f;
        [SerializeField] protected float heavyAttackStunDuration = 0.75f;
        [SerializeField] protected float heavyAttackKnockbackForce = 5f;
        [SerializeField] protected float heavyAttackCooldownDuration = 0.5f;
        [SerializeField] protected AudioClip heavyAttackSound;

        [Space]
        [Header("Block Settings")]
        [SerializeField] protected float blockCooldown = 3;
        [SerializeField] protected float maxBlockHoldTime = 2f;
        [SerializeField] protected float blockAfterAttackCooldown = 0.2f;
        [SerializeField] protected AudioClip blockSound;

        [Space]
        [Header("Health Settings")]
        [SerializeField] protected int maxHealth = 100;
        [SerializeField] protected AudioClip takeDamageSound;

        [Space]
        [Header("Ultimate Settings")]
        [SerializeField] protected float ultimateCooldownDuration = 100f;

        [NonSerialized] public bool canUseAbility = false;
        [NonSerialized] public InputDevice device;

        //stores the player inputs into variables
        private Vector2 _moveInput;
        private bool _jumpButtonDown;
        private bool _attackKeyDown;
        private bool _blockKeyDown;
        private bool _abilityKeyDown;
        private bool _ultimateKeyDown;

        private float _holdAttackTime;
        private float _holdBlockTime;
        //references
        protected Rigidbody rb;
        protected AudioSource audioSource;

        //for other scripts
        [NonSerialized] public bool isBlocking = false;

        //cooldown handling
        private float _attackCooldown;
        private float _blockCooldown;
        private float _stunDuration;
        private float _ultimateCooldown;

        //other variables
        private int _currentHealth;
        private CombatState _state = CombatState.Normal;

        #endregion

        #region Unity Methods

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();

            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            groundMask = LayerMask.GetMask("Ground");

            _ultimateCooldown = ultimateCooldownDuration;
            _currentHealth = maxHealth;

            PlayerLookupMap.AddPlayer(GetComponent<Collider>(), this);
        }

        private void Update()
        {
            HandleCooldowns();
        }

        private void FixedUpdate()
        {
            if (_stunDuration > 0) return;
            if (!isBlocking)
            {
                HandleMovement();
                HandleJump();
            }
            CombatManager();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);

            BasePlayerController otherPlayer = Hitbox();
            Gizmos.color = otherPlayer == null ? Color.red : Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(3, 3, 3));

            switch (_state)
            {
                case CombatState.Normal:
                    Gizmos.color = Color.blue;
                    break;
                case CombatState.Attacking:
                    Gizmos.color = Color.magenta;
                    break;
                case CombatState.Blocking:
                    Gizmos.color = Color.green;
                    break;
                case CombatState.Ability:
                    Gizmos.color = Color.red;
                    break;
                case CombatState.Ultimate:
                    Gizmos.color = Color.yellow;
                    break;
            }

            Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
        }

        private void OnDestroy()
        {
            PlayerLookupMap.RemovePlayer(GetComponent<Collider>());
        }

        #endregion

        #region Input Methods

        public void OnMove(InputAction.CallbackContext cc)
        {
            _moveInput = cc.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext cc)
        {
            _jumpButtonDown = cc.performed;
        }

        public void onAttack(InputAction.CallbackContext cc)
        {
            _attackKeyDown = cc.performed;
        }

        public void onBlock(InputAction.CallbackContext cc)
        {
            _blockKeyDown = cc.performed;
        }

        public void onAbility(InputAction.CallbackContext cc)
        {
            _abilityKeyDown = cc.performed;
        }

        public void onUltimate(InputAction.CallbackContext cc)
        {
            _ultimateKeyDown = cc.performed;
        }

        #endregion

        #region Movement Methods

        protected virtual void HandleMovement()
        {
            rb.linearVelocity = new Vector3(_moveInput.x * walkSpeed, rb.linearVelocity.y, 0);
        }

        protected virtual void HandleJump()
        {
            if (!(_jumpButtonDown && isGrounded)) return;

            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        #endregion

        #region Combat Methods

        protected virtual void CombatManager()
        {
            if (_ultimateKeyDown && _ultimateCooldown < 0)
            {
                Ultimate();
                _state = CombatState.Ultimate;
                return;
            }

            if (_abilityKeyDown && canUseAbility)
            {
                Ability();
                _state = CombatState.Ability;
                return;
            }

            if (canBlock)
            {
                Block();
                _state = CombatState.Blocking;
                return;
            }
            else
            {
                isBlocking = false;
            }

            if (canAttack)
            {
                Attack();
                _state = CombatState.Attacking;
                return;
            }

            _state = CombatState.Normal;
        }

        protected virtual void Attack()
        {
            bool isLightAttack = _holdAttackTime < heavyAttackHoldTime;
            _holdAttackTime = 0f;

            audioSource.PlayOneShot(isLightAttack ? lightAttackSound : heavyAttackSound);

            Stun(0.25f);

            BasePlayerController target = Hitbox();
            if (target == null) return;

            if (isLightAttack && target.isBlocking)
            {
                target.audioSource.PlayOneShot(target.blockSound);
                return;
            }

            AttackData data = GetAttackData(isLightAttack);

            _attackCooldown = data.cooldown;

            ApplyAttack(target, data);
        }

        private AttackData GetAttackData(bool isLight)
        {
            return isLight
            ? new AttackData(lightAttackDamage, lightAttackKnockbackForce, lightAttackStunDuration, lightAttackCooldownDuration, isLight)
                : new AttackData(heavyAttackDamage, heavyAttackKnockbackForce, heavyAttackStunDuration, heavyAttackCooldownDuration, isLight);
        }

        private void ApplyAttack(BasePlayerController target, AttackData data)
        {
            target.audioSource.PlayOneShot(target.takeDamageSound);

            Vector3 direction =
                target.gameObject.transform.position
                - transform.position;
            direction.Normalize();
            direction.z = 0;

            target.Damage(data.damage);
            target.Knockback(direction, data.knockback);
            target.Stun(data.stunDuration);

            if (!data.isLightAttack && target.isBlocking)
            {
                target.DisableBlock(data.stunDuration + 0.25f);
            }
        }

        protected BasePlayerController Hitbox()
        {
            Collider[] collidersInRange = Physics.OverlapBox(transform.position, new Vector3(3, 3, 3) / 2);
            foreach (Collider collider in collidersInRange)
            {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(collider);
                if (controllerToCheck == null) continue;
                if (controllerToCheck == this) continue;

                return controllerToCheck;
            }

            return null;
        }

        protected virtual void Block()
        {
            if (_holdBlockTime >= maxBlockHoldTime)
            {
                isBlocking = false;
                _blockCooldown = blockCooldown;
                return;
            }

            isBlocking = true;
        }

        //These two functions can be left empty, as they will be unique per player
        protected virtual void Ability()
        {
            Debug.Log("Ability");
        }

        protected virtual void Ultimate()
        {
            Debug.Log("Ultimate");
        }

        protected virtual void Knockback(Vector3 direction, float knockbackForce)
        {
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
            Debug.Log(this.gameObject.name + " has taken knockback");
        }

        protected virtual void Damage(int inDamage)
        {
            _currentHealth -= inDamage;

            if (_currentHealth <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void Stun(float duration)
        {
            _stunDuration = duration;
        }

        protected virtual void DisableBlock(float duration)
        {
            _blockCooldown = duration;
            isBlocking = false;
        }

        #endregion

        #region Check Methods
        protected bool isGrounded => Physics.Raycast(transform.position, Vector3.down, 1.2f, groundMask);
        protected bool canAttack => !_attackKeyDown && _holdAttackTime > 0 && _attackCooldown < 0 && _stunDuration < 0;
        protected bool canBlock => _blockKeyDown && _blockCooldown < 0 && _stunDuration < 0;

        #endregion

        #region Other Methods

        private void HandleCooldowns()
        {
            _attackCooldown -= Time.deltaTime;
            _blockCooldown -= Time.deltaTime;
            _stunDuration -= Time.deltaTime;
            _ultimateCooldown -= Time.deltaTime;

            if (_attackKeyDown) _holdAttackTime += Time.deltaTime;
            if (_blockKeyDown) _holdBlockTime += Time.deltaTime;
            else _holdBlockTime = 0;
        }

        #endregion
    }

    public readonly struct AttackData
    {
        public readonly int damage;
        public readonly float knockback;
        public readonly float stunDuration;
        public readonly float cooldown;
        public readonly bool isLightAttack;

        public AttackData(int damage, float knockback, float stunDuration, float cooldown, bool isLightAttack)
        {
            this.damage = damage;
            this.knockback = knockback;
            this.stunDuration = stunDuration;
            this.cooldown = cooldown;
            this.isLightAttack = isLightAttack;
        }
    }
}

public enum CombatState {
    Normal,
    Ultimate,
    Ability,
    Blocking,
    Attacking
}