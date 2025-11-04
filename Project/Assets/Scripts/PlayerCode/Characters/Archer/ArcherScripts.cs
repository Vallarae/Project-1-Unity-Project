using System;
using PlayerCode.BaseCode;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerCode.Characters.Archer {
    public class ArcherScripts : BasePlayerController {
        public GameObject arrow;
        public int hitRequirement;
        public float angleOffset;
        public GameObject arrowSpawnerPoint;

        private int _curHitCount;
        
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
            Transform curTransform = arrowSpawnerPoint.transform;
            curTransform.rotation = Quaternion.Euler(0, 0, 0);
            Instantiate(arrow, curTransform);
            
            curTransform.rotation = Quaternion.Euler(angleOffset, 0, 0);
            Instantiate(arrow, curTransform);
            
            curTransform.rotation = Quaternion.Euler(-angleOffset, 0, 0);
            Instantiate(arrow, curTransform);
            
            _curHitCount = 0;
        }

        protected void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Transform curTransform = arrowSpawnerPoint.transform;
            curTransform.rotation = Quaternion.Euler(0, 0, 0);
            Gizmos.DrawLine(curTransform.position, curTransform.position + curTransform.forward);
            
            curTransform.rotation = Quaternion.Euler(angleOffset, 0, 0);
            Gizmos.DrawLine(curTransform.position, curTransform.position + curTransform.forward);
            
            curTransform.rotation = Quaternion.Euler(-angleOffset, 0, 0);
            Gizmos.DrawLine(curTransform.position, curTransform.position + curTransform.forward);
        }
    }
}