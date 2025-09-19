using UnityEngine;
using System.Collections;

namespace manac.Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        //public int damage = 1;
        public Vector2 velocity;
        public Vector2 direction = new(0, 1);
        //public float rotation = 0f;
        public float startSpeed = 9f;
        public float endSpeed = 6.5f;
        public float decelTime = 0.5f;
        private float currentSpeed;
        private float timer = 0f;
        public float selfDestroy = 3f;
        public int damage = 1;
        //private string bullet_color;
        
        private Coroutine destroyCoroutine;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            currentSpeed = startSpeed;
        }
        
        void OnEnable()
        {
            // Start the destroy timer when bullet becomes active
            if (destroyCoroutine != null)
                StopCoroutine(destroyCoroutine);
            destroyCoroutine = StartCoroutine(DestroyAfterTime());
        }

        // Update is called once per frame
        void Update()
        {
            if (timer < decelTime)
            {
                timer += Time.deltaTime;
                currentSpeed = Mathf.Lerp(startSpeed, endSpeed, timer / decelTime);
            }
            else
            {
                currentSpeed = endSpeed; // clamp to final speed
            }
            velocity = direction * currentSpeed;
        }

        private void FixedUpdate()
        {
            Vector2 pos = transform.position;

            pos += velocity * Time.fixedDeltaTime;

            transform.position = pos;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            Debug.Log($"Bullet hit: {col.collider.name} with tag: {col.collider.tag}");
            
            if (col.collider.CompareTag("Player"))
            {
                var hp = col.collider.GetComponent<ShipPlayerHealth>();
                if (hp) 
                {
                    Debug.Log($"Player took {damage} damage!");
                    hp.TakeDamage(damage);
                }
                else
                {
                    Debug.Log("No ShipPlayerHealth component found!");
                }
            }
            
            // Return bullet to pool instead of destroying
            ReturnToPool();
        }
        
        private IEnumerator DestroyAfterTime()
        {
            yield return new WaitForSeconds(selfDestroy);
            ReturnToPool();
        }
        
        private void ReturnToPool()
        {
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }
            
            if (BulletPool.Instance != null)
            {
                BulletPool.Instance.ReturnBullet(gameObject);
            }
            else
            {
                // Fallback to destroy if pool doesn't exist
                Destroy(gameObject);
            }
        }
        
        public void ResetBullet()
        {
            // Reset bullet properties when returning to pool
            timer = 0f;
            currentSpeed = startSpeed;
            velocity = Vector2.zero;
            direction = new Vector2(0, 1);
            
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }
        }
    }
}
