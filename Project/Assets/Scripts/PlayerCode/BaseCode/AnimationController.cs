using Unity.VisualScripting;
using UnityEngine;

public class AnimationController : MonoBehaviour {
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void SetXMovementValue(float value) {
        if (animator == null) return;
        animator.SetFloat("xMovement", value);
    }

    public void SetYVelocity(float value) {
        if (animator == null) return;
        animator.SetFloat("yVelocity", value);
    }

    public void UpdateAttackInput(int attackIndex) {
        if (animator == null) return;
        animator.SetInteger("AttackIndex", attackIndex);
    }

    public void UpdateIsBlocking(bool isBlocking) {
        if (animator == null) return;
        animator.SetBool("IsBlocking", isBlocking);
    }
    
    public void OnAbilityUse(bool useAbility) {
        if (animator == null) return;
        animator.SetBool("IsAbility", useAbility);
    }

    public void OnHitTake() {
        if (animator == null) return;
        animator.SetBool("IsHit", false);

        Invoke(nameof(OnHitTakeStart), 0.05f);
        Invoke(nameof(OnHitTakeFinish), 0.5f);
    }
    
    public void TurnToFace(Transform position) {
        if (animator == null) return;
        float difference = transform.position.x - position.position.x;

        transform.rotation = Quaternion.Euler(0, difference > 0 ? -90 : 90, 0);
    }

    private void OnHitTakeStart() {
        if (animator == null) return;
        animator.SetBool("IsHit", true);
    }

    private void OnHitTakeFinish(){
        if (animator == null) return;
        animator.SetBool("IsHit", false);
    }
}
