using NUnit.Framework;
using System.Runtime.InteropServices.WindowsRuntime;
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
        protected virtual int health => 100;
        protected virtual int lightAttackDamage => 3;
        protected virtual int heavyAttackDamage => 5;
        protected virtual float lightAttackCooldownDuration => 0.25f;
        protected virtual float heavyAttackCooldownDuration => 0.5f;
        protected virtual float heavyAttackHoldTime => 0.75f;
        protected virtual float lightAttackStunDuration => 0.25f;
        protected virtual float heavyAttackStunDuration => 0.75f;
        protected virtual float lightAttackKnockbackForce => 3f;
        protected virtual float heavyAttackKnockbackForce => 5f;
        protected virtual float maxBlockHoldTime => 2f;
        protected virtual float blockCooldown => 3;
        protected virtual float blockAfterAttackCooldown => 0.2f;
        protected virtual float ultimateCooldownDuration => 100f;
        protected virtual bool canUseAbility => false;

        protected LayerMask groundMask;

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

        //for other scripts
        public bool isBlocking = false;

        //cooldown handling
        private float _attackCooldown;
        private float _blockCooldown;
        [SerializeField] private float _stunDuration;
        private float _ultimateCooldown;

        #endregion

        #region Unity Methods

        //called at the start of the script
        private void Start() {
            rb = GetComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            groundMask = LayerMask.GetMask("Ground");

            _ultimateCooldown = ultimateCooldownDuration;

            PlayerLookupMap.AddPlayer(GetComponent<Collider>(), this);
        }

        private void Update() {
            HandleCooldowns();
        }

        //to ensure that the logic remains consistant among all frame rate
        private void FixedUpdate(){
            if (_stunDuration > 0) return;
            HandleMovement();
            HandleJump();
            CombatManager();
        }

        //for debugging
        private void OnDrawGizmos() {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);

            //Collider[] collidersInRange = Physics.OverlapBox(transform.position, new Vector3(3, 3, 3));

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(3, 3, 3));
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
            if (!c.performed) return;

            _holdAttackTime += Time.deltaTime;
        }

        public void HandleBlockInput(InputAction.CallbackContext c) { 
            _blockKeyDown = c.performed;
            if (!c.performed) return;

            _holdBlockTime += Time.deltaTime;
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
                return;
            }

            if (_abilityKeyDown && canUseAbility) {
                Ability();
                return;
            }

            if (canBlock) {
                Block();
                return;
            }

            if (canAttack) {
                Attack();
                return;
            }
        }

        protected virtual void Attack() {
            bool isLightAttack = _holdAttackTime > heavyAttackHoldTime;
            _holdAttackTime = 0;
            Stun(0.25f);

            //get other fighter
            Collider[] collidersInRange = Physics.OverlapBox(transform.position, new Vector3(3, 3, 3) / 2);
            BasePlayerController otherFighter = null;
            foreach (Collider collider in collidersInRange) {
                BasePlayerController controllerToCheck = PlayerLookupMap.GetPlayer(collider);
                if (controllerToCheck == null) continue;

                if (controllerToCheck == this) continue;

                otherFighter = controllerToCheck;
                break;
            }

            if (otherFighter == null) return;

            if (isLightAttack && otherFighter.isBlocking) return;

            int finalDamage = isLightAttack ? lightAttackDamage : heavyAttackDamage;
            float finalKnockback = isLightAttack ? lightAttackKnockbackForce : heavyAttackKnockbackForce;
            float finalStunDuration = isLightAttack ? lightAttackStunDuration : heavyAttackStunDuration;

            otherFighter.Damage(finalDamage);
            otherFighter.Knockback(finalKnockback);
            otherFighter.Stun(finalStunDuration);
        }

        protected virtual void Block() {
            Debug.Log("Block");
        }
        
        protected virtual void Ability() {
            Debug.Log("Ability");
        }
        
        protected virtual void Ultimate() {
            Debug.Log("Ultimate");
        }
        
        protected virtual void Knockback(float knockbackForce) { 
            rb.AddForce(Vector3.right * knockbackForce, ForceMode.Impulse);
            Debug.Log(this.gameObject.name + " has taken knockback");
        }

        protected virtual void Damage(int inDamage) { }

        protected virtual void Stun(float duration) { 
            _stunDuration = duration;
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
        }

        #endregion
    }
}