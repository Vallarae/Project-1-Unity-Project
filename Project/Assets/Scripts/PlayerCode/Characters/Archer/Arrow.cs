using System;
using PlayerCode.BaseCode;
using UnityEngine;

namespace PlayerCode.Characters.Archer {
    [RequireComponent(typeof(Rigidbody))]
    public class Arrow : MonoBehaviour {
        public float arrowSpeed = 5f;
        public float detectRadius = 0.5f;

        public int damage;
        public float stunDuration;
        public float knockback;
        
        [Space] [SerializeField] private Rigidbody rb;

        private void Start() {
            rb = GetComponent<Rigidbody>();
            
            rb.useGravity = false;
            
            Invoke(nameof(Destroy), 3f);
        }

        private void Update() {
            rb.linearVelocity = transform.forward * arrowSpeed;
            
            Collider[]  colliders = Physics.OverlapSphere(transform.position, detectRadius);

            BasePlayerController hit = null;
            foreach (Collider currentCollider in colliders) {
                hit = PlayerLookupMap.GetPlayer(currentCollider);
                
                if (ReferenceEquals(hit, null)) continue;
                break;
            }
            
            if (ReferenceEquals(hit, null)) return;
            
            
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            
            Gizmos.DrawWireSphere(transform.position + transform.forward, detectRadius);
        }

        private void Destroy() {
            Destroy(gameObject);
        }
    }
}