using UnityEngine;
using System.Collections;

namespace manac.Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleEnnemyPatrol : MonoBehaviour
    {
        [Header("Collision Damage")]
        public int touchDamage = 1;
        
        [Header("Physics")]
        public bool canCollideWithPlayer = true;
        public bool canCollideWithOtherEnemies = false;

        void Start()
        {
        }


        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Player") && canCollideWithPlayer)
            {
                var hp = col.collider.GetComponent<ShipPlayerHealth>();
                if (hp) 
                {
                    hp.TakeDamage(touchDamage);
                }
            }
            else if (col.collider.CompareTag("Enemy") && !canCollideWithOtherEnemies)
            {
            }
        }
    }
}
