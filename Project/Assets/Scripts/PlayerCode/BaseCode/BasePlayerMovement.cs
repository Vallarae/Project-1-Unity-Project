using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using PlayerCode.PlayerJoinSystem;

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
        private LayerMask _groundMask;
        
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
        
        [NonSerialized] protected bool canUseAbility = false;
        
        public bool showInputVariables;

        //stores the player inputs into variables
        public Vector2 moveInput;
        public bool jumpButtonDown;
        public bool dashButtonDown;
        public bool attackKeyDown;
        public bool blockKeyDown;
        public bool abilityKeyDown;
        public bool ultimateKeyDown;

        private float _holdAttackTime;
        private float _holdBlockTime;

        private int _hitCount;
        private bool _hasAirAttacked;
        public bool canMove;

        //references
        protected Rigidbody rb;
        private AudioSource _audioSource;
        protected AnimationController animationController;

        //for other scripts
        [NonSerialized] public bool isBlocking;

        //cooldown handling
        private float _attackCooldown;
        private float _timeSinceLastHit;
        private float _blockCooldown;
        private float _stunDuration;
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
        
        //moved into separate class in case start breaks again
        protected void Initialise()
        {
            rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            animationController = GetComponent<AnimationController>();

            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            _groundMask = LayerMask.GetMask("Ground");
            
            _currentHealth = maxHealth;

            PlayerLookupMap.AddPlayer(GetComponent<Collider>(), this);

            canMove = true;
        }

        private void Update() {
            OnUpdate();
        }

        //to make it work in inheriting scripts
        protected void OnUpdate() {
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
                player.selectedCharacter = null;
                players.Add(player);
            }

            PlayerManager.instance.players = players;

            SceneManager.LoadScene(1);
        }

        #endregion

        #region Input Methods

        public void OnMove(InputAction.CallbackContext cc) {
            moveInput = cc.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext cc) {
            jumpButtonDown = cc.performed;
        }

        public void OnDash(InputAction.CallbackContext cc) {
            dashButtonDown = cc.performed;
        }

        public void OnAttack(InputAction.CallbackContext cc) {
            attackKeyDown = cc.performed;
        }

        public void OnBlock(InputAction.CallbackContext cc) {
            blockKeyDown = cc.performed;
        }

        public void OnAbility(InputAction.CallbackContext cc) {
            abilityKeyDown = cc.performed;
        }

        public void OnUltimate(InputAction.CallbackContext cc) {
            ultimateKeyDown = cc.performed;
        }

        #endregion

        #region Movement Methods

        protected virtual void HandleMovement() {
            rb.linearVelocity = new Vector3(moveInput.x * walkSpeed, rb.linearVelocity.y, 0);
            Vector3 position = transform.position;
            position.z = 0;
            transform.position = position;

            animationController.SetXMovementValue(moveInput.magnitude);
        }

        protected virtual void HandleJump() {
            if (!(jumpButtonDown && isGrounded)) return;

            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        protected virtual void HandleDash() {
            if (!dashButtonDown || _dashCooldown > 0) return;

            _dashCooldown = dashCooldown;
            rb.AddForce(moveInput * dashForce, ForceMode.Impulse);
            animationController.UpdateIsDashing(true);
        }

        #endregion

        #region Combat Methods

        protected virtual void CombatManager() {
            if (abilityKeyDown && canUseAbility) {
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
            
            animationController.OnPunchAttempt();

            bool isLightAttack = _holdAttackTime < heavyAttackHoldTime;

            AttackData data = GetAttackData(isLightAttack);
            _attackCooldown = data.cooldown;

            _holdAttackTime = 0f;

            _audioSource.PlayOneShot(isLightAttack ? lightAttackSound : heavyAttackSound);

            Stun(0.1f);

            if (!isGrounded) _hasAirAttacked = true;

            BasePlayerController target = Hitbox();
            if (ReferenceEquals(target, null)) return;

            if (isLightAttack && target.isBlocking) {
                target._audioSource.PlayOneShot(target.blockSound);
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

            _hitCount++;
            _timeSinceLastHit = 0;

            ApplyAttack(target, data, _hitCount >= 4);

            if (_hitCount >= 4) { 
                _attackCooldown = 1.5f; 
            }
        }

        private AttackData GetAttackData(bool isLight) {
            return isLight
                ? new AttackData(lightAttackDamage, lightAttackKnockbackForce, lightAttackStunDuration,
                    lightAttackCooldownDuration, true)
                : new AttackData(heavyAttackDamage, heavyAttackKnockbackForce, heavyAttackStunDuration,
                    heavyAttackCooldownDuration, false);
        }

        protected void ApplyAttack(BasePlayerController target, AttackData data, bool doesHeavyKnockback = false) {
            target._audioSource.PlayOneShot(target.takeDamageSound);

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
            foreach (Collider curCollider in collidersInRange)
            {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(curCollider);
                if (controllerToCheck == null) continue;
                if (controllerToCheck == this) continue;

                return controllerToCheck;
            }

            return null;
        }
        
        private BasePlayerController Hitbox(Vector3 offset, Vector3 size) {
            Collider[] collidersInRange = Physics.OverlapBox(transform.position + offset, size / 2);
            foreach (Collider curCollider in collidersInRange) {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(curCollider);
                if (controllerToCheck == null) continue;
                if (controllerToCheck == this) continue;

                return controllerToCheck;
            }

            return null;
        }

        private BasePlayerController FindOther()
        {
            Collider[] collidersInRange = Physics.OverlapBox(transform.position, hitbox * 6);
            foreach (Collider curCollider in collidersInRange)
            {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(curCollider);
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

        private bool isGrounded => Physics.Raycast(transform.position, Vector3.down, 1.5f, _groundMask);
        private bool canAttack => !attackKeyDown && _holdAttackTime > 0 && _attackCooldown < 0 && _stunDuration < 0 && !_hasAirAttacked;
        private bool canBlock => blockKeyDown && _blockCooldown < 0 && _stunDuration < 0;

        #endregion

        #region Other Methods

        private void HandleCooldowns() {
            _attackCooldown -= Time.deltaTime;
            _blockCooldown -= Time.deltaTime;
            _stunDuration -= Time.deltaTime;
            _timeSinceLastHit += Time.deltaTime;
            _dashCooldown -= Time.deltaTime;

            canMove = !attackKeyDown;

            if (_timeSinceLastHit > 0.65) _hitCount = 0;
            if (isGrounded && _hasAirAttacked) _holdAttackTime = 0;
            if (isGrounded) _hasAirAttacked = false;

            if (attackKeyDown) _holdAttackTime += Time.deltaTime;
            if (blockKeyDown) _holdBlockTime += Time.deltaTime;
            else _holdBlockTime = 0;
        }

        private void UpdateAnimationValues() {
            animationController.SetYVelocity(rb.linearVelocity.y);
            animationController.UpdateIsBlocking(isBlocking);
            animationController.UpdateAttackInput(_hitCount);
            animationController.SetMoveDirection(moveInput.x);
            if (_dashCooldown < 0.8f) animationController.UpdateIsDashing(false);
            if (!ReferenceEquals(FindOther(), null)) animationController.TurnToFace(FindOther().gameObject.transform);
        }
        
        private void HandleHealthBar() {
            if (ReferenceEquals(healthBarUI, null)) return;
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
