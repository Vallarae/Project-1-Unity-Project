using UnityEngine;

namespace PlayerCode.BaseCode {
    public class AnimationController : MonoBehaviour {
        [SerializeField] private Animator animator;

        //for later optimisation
        private int _xMovementId;
        private int _directionMovingId;
        private int _yVelocityId;
        private int _isDashingId;
        private int _attackIndexId;
        private int _isBlockingId;
        private int _isAbilityId;
        private int _isHitId;

        private void Awake() {
            animator = GetComponentInChildren<Animator>();
        }

        public void SetXMovementValue(float value) {
            if (ReferenceEquals(animator, null)) return;
            animator.SetFloat("xMovement", value);
        }

        public void SetMoveDirection(float value) {
            if (ReferenceEquals(animator, null)) return;
            
            int rotation = (int) animator.transform.rotation.eulerAngles.y;

            float newValue = rotation == 90 ? value : -value;

            animator.SetFloat("DirectionMoving", newValue);
        }

        public void SetYVelocity(float value) {
            if (ReferenceEquals(animator, null)) return;
            animator.SetFloat("yVelocity", value);
        }

        public void UpdateIsDashing(bool value) {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("IsDashing", value);
        }

        public void UpdateAttackInput(int attackIndex) {
            if (ReferenceEquals(animator, null)) return;
            animator.SetInteger("AttackIndex", attackIndex);
        }

        public void UpdateIsBlocking(bool isBlocking) {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("IsBlocking", isBlocking);
        }

        public void OnAbilityUse(bool useAbility) {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("IsAbility", useAbility);
        }

        public void OnPunchAttempt() {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("AttackAttempted", false);
            
            Invoke(nameof(OnAttackAttemptStart), 0.05f);
            Invoke(nameof(OnAttackAttemptEnd), 0.5f);
        }

        public void OnHitTake() {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("IsHit", false);

            Invoke(nameof(OnHitTakeStart), 0.05f);
            Invoke(nameof(OnHitTakeFinish), 0.5f);
        }

        public void TurnToFace(Transform position) {
            if (ReferenceEquals(animator, null)) return;
            float difference = transform.position.x - position.position.x;

            transform.rotation = Quaternion.Euler(0, difference > 0 ? -90 : 90, 0);
        }

        public void HideAnimatedObject() {
            animator.gameObject.SetActive(false);
        }

        private void OnHitTakeStart() {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("IsHit", true);
        }

        private void OnHitTakeFinish() {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("IsHit", false);
        }
        
        private void OnAttackAttemptStart() {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("AttackAttempted", true);
        }

        private void OnAttackAttemptEnd() {
            if (ReferenceEquals(animator, null)) return;
            animator.SetBool("AttackAttempted", false);
        }
    }
}