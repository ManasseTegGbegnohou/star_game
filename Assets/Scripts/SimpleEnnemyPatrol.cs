using UnityEngine;
using System.Collections;

namespace manac.Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleEnnemyPatrol : MonoBehaviour
    {
        public int touchDamage = 1;

        void Start()
        {
        }

        void Update()
        {
        } 

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Player"))
            {
                var hp = col.collider.GetComponent<ShipPlayerHealth>();
                if (hp) hp.TakeDamage(touchDamage);
            }
        }
    }
}
