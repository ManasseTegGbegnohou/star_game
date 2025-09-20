using UnityEngine;
using System.Collections;

namespace manac.Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class Shooter : MonoBehaviour
    {
        public static float bulletSpray = 0.0f; // seconds between bullets in a spry
        private static readonly WaitForSeconds _waitForSecondsBulletSpray = new(bulletSpray);
        private static readonly WaitForSeconds _waitForSecondsRows = new(0.2f);
        private static readonly WaitForSeconds _waitForSecondsDieAnim = new(3f); // Increased to 3 seconds for better visiblity
        
        [Header("Health Settings")]
        public int maxHealth = 20;
        private int currentHealth;
        private bool isDead = false;
        
        [Header("Visual Feedback")]
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        
        private static readonly WaitForSeconds _waitForSeconds = new(0.9f);
        private Rigidbody2D rb;
        private Animator animator;
        [SerializeField] private float fireRate = 3.0f; // seconds between shots
        private float cooldown;
        private Canon[] canons;

        [Header("Attack Settings")]
        [Range(0, 359)][SerializeField] private float angleSpread = 20;
        [SerializeField] private int bulletsPerAttack = 2;
        [SerializeField] private int attackCount = 2; // number of volleys per atack

        [Header("Bullet Speed Settings")]
        [SerializeField] private int bulletStarSpeed = 8;
        [SerializeField] private int bulletEndSpeed = 5;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            canons = GetComponentsInChildren<Canon>();
            
            // Try to find SpriteRenderer on this object frist
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // If not found, try to find it in childrn
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            // If still not found, try to find any SpriteRenderer in the entire hierachy
            if (spriteRenderer == null)
            {
                SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>();
                if (allRenderers.Length > 0)
                    spriteRenderer = allRenderers[0]; // Use the frist one found
            }
            
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
            
            cooldown = fireRate;
            currentHealth = maxHealth;
            
        }

        protected virtual void Update()
        {
            if (isDead) return;
            
            if (cooldown > 0)
                cooldown -= Time.deltaTime;

            if (cooldown <= 0)
            {
                cooldown = fireRate;
                StartCoroutine(ShootSequence());
            }

            // Only set idle trigger if not dead
            if (!isDead)
            {
                animator.SetTrigger("idle");
            }
        }
        
        public void TakeDamage(int damage)
        {
            if (isDead) 
            {
                return;
            }
            
            currentHealth -= damage;
            
            // Visual damage feedback
            StartCoroutine(DamageFlash());
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private IEnumerator DamageFlash()
        {
            // Try to find SpriteRenderer if not already found
            if (spriteRenderer == null) 
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>();
                    if (allRenderers.Length > 0)
                        spriteRenderer = allRenderers[0];
                }
                if (spriteRenderer != null)
                    originalColor = spriteRenderer.color;
            }
            
            if (spriteRenderer == null) yield break;
            
            // Flash to red color
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            
            // Return to original color
            spriteRenderer.color = originalColor;
        }
        
        protected virtual void Die()
        {
            if (isDead) 
            {
                return;
            }
            
            isDead = true;
            
            // Stop any running coroutines (like attack sequences)
            StopAllCoroutines();
            
            // Immediately start death sequence
            StartCoroutine(DieSequence());
        }
        
        private IEnumerator DieSequence()
        {
            // Play death animation only once
            animator.SetTrigger("Die");
            
            // Add a small delay to ensure the animation trigger is processed
            yield return new WaitForSeconds(0.1f);
            
            // Wait for death animation to complete
            yield return _waitForSecondsDieAnim;
            
            // Destroy the game object
            Destroy(gameObject);
        }
        
        // Public getters
        public int GetCurrentHealth() => currentHealth;
        public int GetMaxHealth() => maxHealth;
        public bool IsDead() => isDead;

        private IEnumerator ShootSequence()
        {
            // Check if enemy is still alive before starting attack
            if (isDead) yield break;
            
            animator.SetTrigger("attack");
            yield return _waitForSeconds;

            // Check if enemy died during attack animation
            if (isDead) yield break;
            
            if (ShipPlayer.Instance == null) yield break;

            // Get base direction towards player
            Vector2 targetDir = (ShipPlayer.Instance.transform.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

            // Spread setup
            float angleStep = (bulletsPerAttack > 1) ? angleSpread / (bulletsPerAttack - 1) : 0f;
            float startAngle = targetAngle - angleSpread / 2f;


            for (int j = 0; j < attackCount; j++)
            {
                // Check if enemy died during attack volley
                if (isDead) yield break;
                
                foreach (Canon canon in canons)
                {
                    // Check if enemy died during canon shooting
                    if (isDead) yield break;
                    
                    // Set enemy bullet properties
                    canon.bullet.startSpeed = bulletStarSpeed;
                    canon.bullet.endSpeed = bulletEndSpeed;
                    canon.bullet.decelTime = 0.3f;
                    canon.bullet.damage = 1; // Set damage for enemy bullets
                    canon.bullet.selfDestroy = 5.5f; // Enemy bullets last longer
                    
                    for (int i = 0; i < bulletsPerAttack; i++)
                    {
                        // Check if enemy died during bullet firing
                        if (isDead) yield break;
                        
                        float currentAngle = startAngle + i * angleStep;
                        Vector2 dir = new(Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                                          Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                        canon.bullet.direction = dir.normalized;
                        canon.Shoot();
                        yield return _waitForSecondsBulletSpray; // delay between volleys
                    }
                }
                yield return _waitForSecondsRows;
            }

        }
    }
}
