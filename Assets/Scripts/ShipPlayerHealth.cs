using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

namespace manac.Assets.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class ShipPlayerHealth : MonoBehaviour
    {

        public int maxHealth = 5;
        private int currentHealth;
        private Animator animator;
        private static readonly WaitForSeconds _waitForSecondsDieAnim = new(2f);
        void Start()
        {
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;
        }

        void Update()
        {
            if (currentHealth <= 0)
                Die();
        }

        public void TakeDamage(int dmg)
        {
            currentHealth -= dmg;
            if (currentHealth <= 0) Die();
        }

        private IEnumerator DieAnimation()
        {
            //animator.SetTrigger("shoot");
            yield return _waitForSecondsDieAnim;
        }

        void Die()
        {
            StartCoroutine(DieAnimation());
            Debug.Log("Player est mort !");
        }
    }
}
