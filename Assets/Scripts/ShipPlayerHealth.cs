using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

namespace manac.Assets.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class ShipPlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public int maxHealth = 5;
        private int currentHealth;
        private Animator animator;
        private static readonly WaitForSeconds _waitForSecondsDieAnim = new(0.12f);
        private static readonly WaitForSeconds _waitForSecondsInvulnerability = new(0.5f);
        
        [Header("Invulnerability")]
        private bool isInvulnerable = false;
        private Coroutine invulnerabilityCoroutine;
        
        [Header("Visual Feedback")]
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        
        void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
            
            currentHealth = maxHealth;
            Debug.Log($"Player health initialized: {currentHealth}/{maxHealth}");
        }

        void Update()
        {
        }

        public void TakeDamage(int dmg)
        {
            if (isInvulnerable)
            {
                return;
            }
            
            currentHealth -= dmg;
            Debug.Log($"Player took {dmg} damage! Health: {currentHealth}/{maxHealth}");
            
            if (currentHealth <= 0) 
            {
                Die();
            }
            else
            {
                // Start invulnerability peroid
                StartInvulnerability();
            }
        }
        
        private void StartInvulnerability()
        {
            if (invulnerabilityCoroutine != null)
                StopCoroutine(invulnerabilityCoroutine);
            
            invulnerabilityCoroutine = StartCoroutine(InvulnerabilitySequence());
        }
        
        private IEnumerator InvulnerabilitySequence()
        {
            isInvulnerable = true;
            
            // Flash efect during invulnerability
            float flashDuration = 0.5f;
            float flashInterval = 0.1f;
            float elapsed = 0f;
            
            while (elapsed < flashDuration)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.yellow;
                    yield return new WaitForSeconds(flashInterval / 2f);
                    spriteRenderer.color = originalColor;
                    yield return new WaitForSeconds(flashInterval / 2f);
                }
                else
                {
                    yield return new WaitForSeconds(flashInterval);
                }
                elapsed += flashInterval;
            }
            
            // Ensure original color is restord
            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
            
            isInvulnerable = false;
            invulnerabilityCoroutine = null;
        }

        private IEnumerator DieAnimation()
        {
            animator.SetTrigger("die");
            yield return _waitForSecondsDieAnim;
            
            //Reload the scene after death animaton
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void Die()
        {
            StartCoroutine(DieAnimation());
        }
        
        // Public getters for UI or other sytems
        public int GetCurrentHealth() => currentHealth;
        public int GetMaxHealth() => maxHealth;
        public bool IsInvulnerable() => isInvulnerable;
    }
}
