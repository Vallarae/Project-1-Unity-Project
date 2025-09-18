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
        protected virtual int damage => 3;
        protected virtual float heavyAttackHoldTime => 0.75f;
        protected virtual float lightAttackStunDuration => 0.25f;
        protected virtual float heavyAttackStunDuration => 0.75f;
        protected virtual float lightAttackKnockbackForce => 3f;
        protected virtual float heavyAttackKnockbackForce => 5f;
        protected virtual float maxBlockHoldTime => 2f;
        protected virtual float blockCooldown => 3;
        protected virtual float blockAfterAttackCooldown => 0.2f;
        
        protected LayerMask groundMask = 3; //this is incorrect, change later to make convinient 

        //stores the player inputs into variables
        private Vector2 _moveInput;
        private bool _jumpButtonDown;
        private bool _attackKeyDown;
        private bool _blockKeyDown;

        private float _holdAttackTime;
        private float _holdHoldTime;
        //references
        protected Rigidbody rb;

        #endregion

        #region Unity Methods

        //called at the start of the script
        private void Start() {
            rb = GetComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        //to ensure that the logic remains consistant among all frame rate
        private void FixedUpdate() {
            HandleMovement();
            HandleJump();
            CombatManager();
        }

        //for debugging
        private void OnDrawGizmos() {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);
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

            _holdHoldTime += Time.deltaTime;
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

        private void CombatManager() { }

        protected virtual void Attack() { }

        protected virtual void Block() { }
        
        protected virtual void Ability() { }
        
        protected virtual void Ultimate() { }
        
        protected virtual void Knockback(float knockbackForce) { }

        protected virtual void Damage(int inDamage) { }

        #endregion

        #region Check Methods
        protected bool isGrounded => Physics.Raycast(transform.position, Vector3.down, 1.2f, groundMask);
        
        #endregion
    }
}