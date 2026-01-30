using UnityEngine;
using System.Collections;
using PlayerCode.BaseCode;
using System.Runtime.CompilerServices;

namespace PlayerCode.Characters.Knight {
    public class KnightScript : BasePlayerController {
        public Vector3 abilityHitboxSize = new Vector3(2f, 2f, 2f);
        public Vector3 abilityPositionOffset = new Vector3(2f, 2f, 2f);
        public Vector3 abilityDashForce = new Vector3(20f, 20f, 0f);
        public float animationTime = 1f;
        public float abilityDurationTime = 2f;
        public int maxHitCount = 7;
        public int abilityDamage = 15;
        public float abilityStunTime = 3f;
        public float abilityKnockbackForce = 10f;

        public int _abilityHitCount = 0;
        private bool _inAbility = false;
        private float _abilityDuration = 0f;

        private void Start() {
            Initialise();

        }

        protected void Update() {
            OnUpdate();
            
            if (_abilityHitCount >= maxHitCount) {
                canUseAbility = true;
            }
            else {
                canUseAbility = false;
            }

            if (_inAbility) {
                _abilityDuration += Time.deltaTime;
                canMove = false;
                if (_abilityDuration >= abilityDurationTime) {
                    _inAbility = false;
                    _abilityDuration = 0f;
                    canMove = true;
                    return;
                }

                BasePlayerController hitEnemy = Hitbox();
                if (hitEnemy == null) return;

                AttackData attackData = new AttackData(abilityDamage, abilityKnockbackForce, abilityStunTime, 0f, true);

                if (hitEnemy.isBlocking) return;

                ApplyAttack(hitEnemy, attackData, true);
                _inAbility = false;
            }
        }

        protected override void Ability() {
            base.Ability();
            rb.AddForce(moveInput * abilityDashForce, ForceMode.Impulse);
            canUseAbility = false;
            _abilityHitCount = 0;
            _inAbility = true;
            Invoke(nameof(FinishAniamtion), animationTime);
        }

        protected override void Attack() {
            base.Attack();
            _abilityHitCount++;
        }

        private void FinishAniamtion() {
            animationController.OnAbilityUse(false);
        }
    }
}
