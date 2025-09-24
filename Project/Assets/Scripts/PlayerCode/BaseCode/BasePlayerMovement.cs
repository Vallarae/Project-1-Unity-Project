using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerCode.BaseCode {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    public abstract class BasePlayerController : MonoBehaviour {
        #region Variables

        //default values that can be changed later
        protected virtual float walkSpeed => 3f;
        protected virtual float jumpHeight => 2f;
        protected virtual float lightAttackCooldownDuration => 0.25f;
        protected virtual float heavyAttackCooldownDuration => 0.5f;
        protected virtual float heavyAttackHoldTime => 0.3f;
        protected virtual float lightAttackStunDuration => 0.25f;
        protected virtual float heavyAttackStunDuration => 0.75f;
        protected virtual float lightAttackKnockbackForce => 3f;
        protected virtual float heavyAttackKnockbackForce => 5f;
        protected virtual float maxBlockHoldTime => 2f;
        protected virtual float blockCooldown => 3;
        protected virtual float blockAfterAttackCooldown => 0.2f;
        protected virtual float ultimateCooldownDuration => 100f;
        protected virtual int maxHealth => 100;
        protected virtual int lightAttackDamage => 3;
        protected virtual int heavyAttackDamage => 5;
        protected bool canUseAbility = false;

        protected LayerMask groundMask;

        //stores the player inputs into variables
        private Vector2 _moveInput;
        private bool _jumpButtonDown;
        private bool _attackKeyDown;
        private bool _blockKeyDown;
        private bool _abilityKeyDown;
        private bool _ultimateKeyDown;

        private float _holdAttackTime;
        [SerializeField] private float _holdBlockTime;
        //references
        protected Rigidbody rb;

        //for other scripts
        public bool isBlocking = false;

        //cooldown handling
        private float _attackCooldown;
        [SerializeField] private float _blockCooldown;
        private float _stunDuration;
        private float _ultimateCooldown;

        //other variables
        private int _currentHealth;
        private CombatState _state = CombatState.Normal;

        #endregion

        #region Unity Methods

        private void Start() {
            rb = GetComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            groundMask = LayerMask.GetMask("Ground");

            _ultimateCooldown = ultimateCooldownDuration;
            _currentHealth = maxHealth;

            PlayerLookupMap.AddPlayer(GetComponent<Collider>(), this);
        }

        private void Update() {
            HandleCooldowns();
        }

        private void FixedUpdate() {
            if (_stunDuration > 0) return;
            if (!isBlocking) {
                HandleMovement();
                HandleJump();
            }
            CombatManager();
        }

        private void OnDrawGizmos() {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);

            BasePlayerController otherPlayer = Hitbox();
            Gizmos.color = otherPlayer == null ? Color.red : Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(3, 3, 3));

            switch (_state) {
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

        private void OnDestroy() {
            PlayerLookupMap.RemovePlayer(GetComponent<Collider>());
        }

        #endregion

        #region Input Methods

        public void MoveInput(InputAction.CallbackContext c) {
            _moveInput = c.ReadValue<Vector2>();
        }

        public void HandleJumpInput(InputAction.CallbackContext c) {
            _jumpButtonDown = c.performed;
        }

        public void HandleAttackInput(InputAction.CallbackContext c) {
            _attackKeyDown = c.performed;
        }

        public void HandleBlockInput(InputAction.CallbackContext c) {
            _blockKeyDown = c.performed;
        }

        public void HandleAbilityInput(InputAction.CallbackContext c) {
            _abilityKeyDown = c.performed;
        }

        public void HandleUltimateInput(InputAction.CallbackContext c) {
            _ultimateKeyDown = c.performed;
        }

        #endregion

        #region Movement Methods

        protected virtual void HandleMovement() {
            //single movement, maybe add velocity later
            rb.linearVelocity = new Vector3(_moveInput.x * walkSpeed, rb.linearVelocity.y, 0);
        }

        protected virtual void HandleJump() {
            if (!(_jumpButtonDown && isGrounded)) return;

            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        #endregion

        #region Combat Methods

        protected virtual void CombatManager() {
            if (_ultimateKeyDown && _ultimateCooldown < 0) {
                Ultimate();
                _state = CombatState.Ultimate;
                return;
            }

            if (_abilityKeyDown && canUseAbility) {
                Ability();
                _state = CombatState.Ability;
                return;
            }

            if (canBlock) {
                Block();
                _state = CombatState.Blocking;
                return;
            } else {
                isBlocking = false;
            }

            if (canAttack) {
                Attack();
                _state = CombatState.Attacking;
                return;
            }

            _state = CombatState.Normal;
        }

        protected virtual void Attack() {
            bool isLightAttack = _holdAttackTime < heavyAttackHoldTime;
            Stun(0.25f);
            _holdAttackTime = 0f;

            BasePlayerController otherFighter = Hitbox();

            if (otherFighter == null) return;
            if (isLightAttack && otherFighter.isBlocking) return;

            int finalDamage = isLightAttack ? 
                lightAttackDamage : heavyAttackDamage;
            float finalKnockback = isLightAttack ? 
                lightAttackKnockbackForce : heavyAttackKnockbackForce;
            float finalStunDuration = isLightAttack ? 
                lightAttackStunDuration : heavyAttackStunDuration;

            float finalAttackCooldown = isLightAttack ?
                lightAttackCooldownDuration : heavyAttackCooldownDuration;

            Vector3 direction = 
                otherFighter.gameObject.transform.position
                - transform.position;
            direction.Normalize();
            direction.z = 0;

            _attackCooldown = finalAttackCooldown;

            otherFighter.Damage(finalDamage);
            otherFighter.Knockback(direction, finalKnockback);
            otherFighter.Stun(finalStunDuration);
            if (!isLightAttack && otherFighter.isBlocking) 
                otherFighter.DisableBlock(finalStunDuration + 0.25f);
        }

        protected BasePlayerController Hitbox() {
            Collider[] collidersInRange = Physics.OverlapBox(transform.position, new Vector3(3, 3, 3) / 2);
            BasePlayerController otherFighter = null;
            foreach (Collider collider in collidersInRange) {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(collider);
                if (controllerToCheck == null) continue;
                if (controllerToCheck == this) continue;

                otherFighter = controllerToCheck;
                break;
            }

            return otherFighter;
        }

        protected virtual void Block() {
            if (_holdBlockTime >= maxBlockHoldTime) {
                isBlocking = false;
                _blockCooldown = blockCooldown;
                return;
            }

            isBlocking = true;
        }

        //These two functions can be left empty, as they will be unique per player
        protected virtual void Ability() {
            Debug.Log("Ability");
        }

        protected virtual void Ultimate() {
            Debug.Log("Ultimate");
        }

        protected virtual void Knockback(Vector3 direction, float knockbackForce) {
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
            Debug.Log(this.gameObject.name + " has taken knockback");
        }

        protected virtual void Damage(int inDamage) {
            _currentHealth -= inDamage;

            if (_currentHealth <= 0) {
                Destroy(this.gameObject);
            }
        }

        protected virtual void Stun(float duration) {
            _stunDuration = duration;
        }

        protected virtual void DisableBlock(float duration) {
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

        private void HandleCooldowns() {
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
}

public enum CombatState {
    Normal,
    Ultimate,
    Ability,
    Blocking,
    Attacking
}