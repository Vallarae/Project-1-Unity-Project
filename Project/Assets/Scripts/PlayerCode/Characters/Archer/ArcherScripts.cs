using PlayerCode.BaseCode;
using UnityEngine;

namespace PlayerCode.Characters.Archer {
    public class ArcherScripts : BasePlayerController {
        public Arrow arrow;
        public int hitRequirement;
        public float angleOffset;
        public GameObject arrowSpawnerPoint;

        public int _curHitCount;

        private void Start() {
            Initialise();
        }
        
        private void Update() {
            OnUpdate();

            canUseAbility = _curHitCount >= hitRequirement;
        }

        protected override void Attack() {
            base.Attack();

            BasePlayerController other = Hitbox();

            if (ReferenceEquals(other, null)) return;
            
            _curHitCount++;
        }

        protected override void Ability() {
            base.Ability();
            Arrow arr;
            
            arr = Instantiate(arrow,  new(arrowSpawnerPoint.transform.position.x, arrowSpawnerPoint.transform.position.y, 0), Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));
            arr.owner = this;
            arr = Instantiate(arrow,  new(arrowSpawnerPoint.transform.position.x, arrowSpawnerPoint.transform.position.y, 0), Quaternion.Euler(angleOffset, transform.rotation.eulerAngles.y, 0));
            arr.owner = this;
            arr = Instantiate(arrow,  new(arrowSpawnerPoint.transform.position.x, arrowSpawnerPoint.transform.position.y, 0), Quaternion.Euler(-angleOffset, transform.rotation.eulerAngles.y, 0));
            arr.owner = this;
            
            _curHitCount = 0;
            canUseAbility = false;
        }
    }
}

