using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace PlayerCode.BaseCode {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AnimationController))]
    public abstract class BasePlayerController : MonoBehaviour {
        #region Variables
        
        public float walkSpeed = 3f;
        public float jumpHeight = 2f;
        public Vector3 dashForce = new Vector3(3, 2, 0);
        public float dashCooldown = 4f;
        protected LayerMask groundMask;
        
        public int lightAttackDamage = 3;
        public float lightAttackStunDuration = 0.25f;
        public float lightAttackKnockbackForce = 3f;
        public float lightAttackCooldownDuration = 0.25f;
        public AudioClip lightAttackSound;
        
        public int heavyAttackDamage = 5;
        public float heavyAttackHoldTime = 0.3f;
        public float heavyAttackStunDuration = 0.75f;
        public float heavyAttackKnockbackForce = 5f;
        public float heavyAttackCooldownDuration = 0.5f;
        public AudioClip heavyAttackSound;

        public Vector3 hitbox;

        public float blockCooldown = 3;
        public float maxBlockHoldTime = 2f;
        public float blockAfterAttackCooldown = 0.2f;
        public AudioClip blockSound;
        
        public int maxHealth = 100;

        public AudioClip takeDamageSound;

        protected float ultimateCooldownDuration = 100f;
        [NonSerialized] public bool canUseAbility = false;
        
        public bool showInputVariables;

        //stores the player inputs into variables
        public Vector2 _moveInput;
        public bool _jumpButtonDown;
        public bool _dashButtonDown;
        public bool _attackKeyDown;
        public bool _blockKeyDown;
        public bool _abilityKeyDown;
        public bool _ultimateKeyDown;

        private float _holdAttackTime;
        private float _holdBlockTime;

        private int hitCount;
        private bool hasAirAttacked;
        public bool canMove;

        //references
        protected Rigidbody rb;
        protected AudioSource audioSource;
        protected AnimationController animationController;

        //for other scripts
        [NonSerialized] public bool isBlocking = false;

        //cooldown handling
        private float _attackCooldown;
        private float _timeSinceLastHit;
        private float _blockCooldown;
        private float _stunDuration;
        private float _ultimateCooldown;
        private float _dashCooldown;

        //other variables
        private int _currentHealth;
        private CombatState _state = CombatState.Normal;
        [NonSerialized] public Slider healthBarUI;
        
        //editor values -> might not be needed
        public bool showInputValues;

        #endregion

        #region Unity Methods

        private void Start()
        {
            Initialise();
        }
        
        //moved into seperate class incase start breaks again
        protected void Initialise()
        {
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            animationController = GetComponent<AnimationController>();

            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            groundMask = LayerMask.GetMask("Ground");

            _ultimateCooldown = ultimateCooldownDuration;
            _currentHealth = maxHealth;

            PlayerLookupMap.AddPlayer(GetComponent<Collider>(), this);

            canMove = true;
        }

        private void Update() {
            onUpdate();
        }

        //to make it work in inheriting scripts
        protected void onUpdate() {
            HandleCooldowns();
            UpdateAnimationValues();
            HandleHealthBar();
        }

        private void FixedUpdate() {
            if (_stunDuration > 0) return;
            if (!isBlocking && canMove && _dashCooldown < 0.8f) {
                HandleMovement();
                HandleJump();
                HandleDash();
            }

            CombatManager();
        }

        private void OnDrawGizmos() {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);

            BasePlayerController otherPlayer = Hitbox();
            Gizmos.color = otherPlayer == null ? Color.red : Color.green;
            Gizmos.DrawWireCube(transform.position, hitbox);

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

            if (_stunDuration > 0) Gizmos.color = Color.white;

            Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
        }

        private void OnDestroy() {
            PlayerLookupMap.RemovePlayer(GetComponent<Collider>());

            List<PlayerInfo> players = new List<PlayerInfo>();

            foreach (PlayerInfo player in PlayerManager.instance.players)
            {
                int index = PlayerManager.instance.players.IndexOf(player);
                player.selectedCharacter = null;
                players.Add(player);
            }

            PlayerManager.instance.players = players;

            SceneManager.LoadScene(1);
        }

        #endregion

        #region Input Methods

        public void OnMove(InputAction.CallbackContext cc) {
            _moveInput = cc.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext cc) {
            _jumpButtonDown = cc.performed;
        }

        public void OnDash(InputAction.CallbackContext cc) {
            _dashButtonDown = cc.performed;
        }

        public void onAttack(InputAction.CallbackContext cc) {
            _attackKeyDown = cc.performed;
        }

        public void onBlock(InputAction.CallbackContext cc) {
            _blockKeyDown = cc.performed;
        }

        public void onAbility(InputAction.CallbackContext cc) {
            _abilityKeyDown = cc.performed;
        }

        public void onUltimate(InputAction.CallbackContext cc) {
            _ultimateKeyDown = cc.performed;
        }

        #endregion

        #region Movement Methods

        protected virtual void HandleMovement() {
            rb.linearVelocity = new Vector3(_moveInput.x * walkSpeed, rb.linearVelocity.y, 0);
            Vector3 position = transform.position;
            position.z = 0;
            transform.position = position;

            animationController.SetXMovementValue(_moveInput.magnitude);
        }

        protected virtual void HandleJump() {
            if (!(_jumpButtonDown && isGrounded)) return;

            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        protected virtual void HandleDash() {
            if (!_dashButtonDown || _dashCooldown > 0) return;

            _dashCooldown = dashCooldown;
            rb.AddForce(_moveInput * dashForce, ForceMode.Impulse);
            animationController.UpdateIsDashing(true);
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
            }
            else {
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
            if (_attackCooldown > 0) return;

            bool isLightAttack = _holdAttackTime < heavyAttackHoldTime;

            AttackData data = GetAttackData(isLightAttack);
            _attackCooldown = data.cooldown;

            _holdAttackTime = 0f;

            audioSource.PlayOneShot(isLightAttack ? lightAttackSound : heavyAttackSound);

            Stun(0.1f);

            if (!isGrounded) hasAirAttacked = true;

            BasePlayerController target = Hitbox(transform.position, hitbox);
            if (target == null) return;

            if (isLightAttack && target.isBlocking) {
                target.audioSource.PlayOneShot(target.blockSound);
                _attackCooldown = 1.5f;

                Vector3 direction =
                target.gameObject.transform.position
                - transform.position;
                direction.Normalize();
                direction.z = 0;

                Knockback(-direction, data.knockback);
                Damage(1);
                return;
            }

            hitCount++;
            _timeSinceLastHit = 0;

            ApplyAttack(target, data, hitCount >= 4);

            if (hitCount >= 4) { 
                _attackCooldown = 1.5f; 
            }
        }

        private AttackData GetAttackData(bool isLight) {
            return isLight
                ? new AttackData(lightAttackDamage, lightAttackKnockbackForce, lightAttackStunDuration,
                    lightAttackCooldownDuration, isLight)
                : new AttackData(heavyAttackDamage, heavyAttackKnockbackForce, heavyAttackStunDuration,
                    heavyAttackCooldownDuration, isLight);
        }

        protected void ApplyAttack(BasePlayerController target, AttackData data, bool doesHeavyKnockback = false) {
            target.audioSource.PlayOneShot(target.takeDamageSound);

            Vector3 direction =
                target.gameObject.transform.position
                - transform.position;
            direction.Normalize();
            direction.z = 0;

            target.Damage(data.damage);
            target.Knockback(direction, doesHeavyKnockback ? data.knockback : data.knockback / 5);
            target.Stun(data.stunDuration);

            if (!data.isLightAttack && target.isBlocking) {
                target.DisableBlock(data.stunDuration + 0.25f);
            }
        }

        protected BasePlayerController Hitbox()
        {
            Collider[] collidersInRange = Physics.OverlapBox(transform.position, hitbox / 2);
            foreach (Collider collider in collidersInRange)
            {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(collider);
                if (controllerToCheck == null) continue;
                if (controllerToCheck == this) continue;

                return controllerToCheck;
            }

            return null;
        }
        
        protected BasePlayerController Hitbox(Vector3 offset, Vector3 size) {
            Collider[] collidersInRange = Physics.OverlapBox(transform.position + offset, size / 2);
            foreach (Collider collider in collidersInRange) {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(collider);
                if (controllerToCheck == null) continue;
                if (controllerToCheck == this) continue;

                return controllerToCheck;
            }

            return null;
        }

        protected BasePlayerController FindOther()
        {
            Collider[] collidersInRange = Physics.OverlapBox(transform.position, hitbox * 2);
            foreach (Collider collider in collidersInRange)
            {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(collider);
                if (controllerToCheck == null) continue;
                if (controllerToCheck == this) continue;

                return controllerToCheck;
            }

            return null;
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
            animationController.OnAbilityUse(true);
        }

        protected virtual void Ultimate() {
            Debug.Log("Ultimate");
        }

        protected virtual void Knockback(Vector3 direction, float knockbackForce) {
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
        }

        protected virtual void Damage(int inDamage) {
            _currentHealth -= inDamage;
            animationController.OnHitTake();

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
        protected bool canAttack => !_attackKeyDown && _holdAttackTime > 0 && _attackCooldown < 0 && _stunDuration < 0 && !hasAirAttacked;
        protected bool canBlock => _blockKeyDown && _blockCooldown < 0 && _stunDuration < 0;

        #endregion

        #region Other Methods

        private void HandleCooldowns() {
            _attackCooldown -= Time.deltaTime;
            _blockCooldown -= Time.deltaTime;
            _stunDuration -= Time.deltaTime;
            _ultimateCooldown -= Time.deltaTime;
            _timeSinceLastHit += Time.deltaTime;
            _dashCooldown -= Time.deltaTime;

            canMove = !_attackKeyDown;

            if (_timeSinceLastHit > 0.65) hitCount = 0;
            if (isGrounded && hasAirAttacked) _holdAttackTime = 0;
            if (isGrounded) hasAirAttacked = false;

            if (_attackKeyDown) _holdAttackTime += Time.deltaTime;
            if (_blockKeyDown) _holdBlockTime += Time.deltaTime;
            else _holdBlockTime = 0;
        }

        private void UpdateAnimationValues() {
            animationController.SetYVelocity(rb.linearVelocity.y);
            animationController.UpdateIsBlocking(isBlocking);
            animationController.UpdateAttackInput(hitCount);
            animationController.SetMoveDirection(_moveInput.x);
            if (_dashCooldown < 0.8f) animationController.UpdateIsDashing(false);
            if (FindOther() != null) animationController.TurnToFace(FindOther().gameObject.transform);
        }
        
        private void HandleHealthBar() {
            if (healthBarUI == null) return;
            float value = (float) _currentHealth / maxHealth;

            healthBarUI.value = value;
        }

        #endregion
    }

    public readonly struct AttackData {
        public readonly int damage;
        public readonly float knockback;
        public readonly float stunDuration;
        public readonly float cooldown;
        public readonly bool isLightAttack;

        public AttackData(int damage, float knockback, float stunDuration, float cooldown, bool isLightAttack) {
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
