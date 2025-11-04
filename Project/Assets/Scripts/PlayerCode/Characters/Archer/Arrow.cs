using PlayerCode.BaseCode;
using UnityEngine;

namespace PlayerCode.Characters.Archer {
    [RequireComponent(typeof(Rigidbody))]
    public class Arrow : MonoBehaviour {
        public float arrowSpeed = 5f;
        public float detectRadius = 0.5f;
        public float offsetDistance = 0.2f;

        public int damage;
        public float stunDuration;
        public float knockback;

        public BasePlayerController owner;
        
        [Space] [SerializeField] private Rigidbody rb;
        
        
        private void Start() {
            rb = GetComponent<Rigidbody>();

            rb.freezeRotation = true;
            rb.useGravity = false;
            
            Invoke(nameof(Destroy), 3f);
        }

        private void Update() {
            rb.linearVelocity = transform.forward * arrowSpeed;

            if (ReferenceEquals(owner, null)) return;
            
            Collider[]  colliders = Physics.OverlapSphere(transform.position, detectRadius);

            if (colliders.Length == 0) return;

            BasePlayerController hit = null;
            foreach (Collider currentCollider in colliders) {
                hit = PlayerLookupMap.GetPlayer(currentCollider);
                
                if (ReferenceEquals(hit, null)) continue;
                break;
            }

            if (ReferenceEquals(hit, null)) {
                return;
            }
            if (ReferenceEquals(owner, hit)) {
                return;
            }

            AttackData data = new AttackData(damage, 0, stunDuration, 0, true);
            hit.ApplyAttack(hit, data, true);
            Destroy(gameObject);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            
            Gizmos.DrawWireSphere(transform.position + transform.forward * offsetDistance, detectRadius);
        }

        private void Destroy() {
            Destroy(gameObject);
        }

        private void onTriggerEnter(Collider other) {
            Destroy();
        }
    }
}
