using UnityEngine;
using System.Collections;

namespace manac.Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        public enum BulletSource
        {
            Player,
            Enemy
        }
        
        [Header("Bullet Properties")]
        public Vector2 velocity;
        public Vector2 direction = new(0, 1);
        public float startSpeed = 9f;
        public float endSpeed = 6.5f;
        public float decelTime = 0.5f;
        public float selfDestroy = 3f;
        public int damage = 1;
        public BulletSource bulletSource = BulletSource.Player;
        
        private float currentSpeed;
        private float timer = 0f;
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

        void OnTriggerEnter2D(Collider2D col)
        {
            
            // Determine bullet source based on who fired it
            bool isPlayerBullet = IsPlayerBullet();
            
            if (col.CompareTag("Player"))
            {
                // Only enemy bullets can damage the player
                if (!isPlayerBullet)
                {
                    var hp = col.GetComponent<ShipPlayerHealth>();
                    if (hp) 
                    {
                        // Check if player is already dead (though this shouldn't happen with current health system)
                        if (hp.GetCurrentHealth() <= 0)
                        {
                            return;
                        }
                        
                        hp.TakeDamage(damage);
                        // Destroy bullet after hitting player
                        ReturnToPool();
                        return;
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
            else if (col.CompareTag("Enemy") || col.GetComponent<Shooter>() != null)
            {
                // Only player bullets can damage enemies
                if (isPlayerBullet)
                {
                    var shooter = col.GetComponent<Shooter>();
                    if (shooter) 
                    {
                        // Check if enemy is already dead
                        if (shooter.IsDead())
                        {
                            return;
                        }
                        
                        shooter.TakeDamage(damage);
                        // Destroy bullet after hitting enemy
                        ReturnToPool();
                        return;
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
            else
            {
            }
            
            // Bullets that don't hit valid targets continue flying
            // They will be returned to pool after selfDestroy time or when they hit boundaries
        }
        
        private bool IsPlayerBullet()
        {
            return bulletSource == BulletSource.Player;
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
            bulletSource = BulletSource.Player; // Reset to default
            
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }
        }
    }
}
