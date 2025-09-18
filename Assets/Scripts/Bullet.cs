using UnityEngine;

namespace manac.Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        //public int damage = 1;
        public Vector2 velocity;
        public Vector2 direction = new(0, 1);
        //public float rotation = 0f;
        public float startSpeed = 9f;
        public float endSpeed = 6f;
        public float decelTime = 0.5f;
        private float currentSpeed;
        private float timer = 0f;
        public float selfDestroy = 3f;
        public int damage = 1;
        //private string bullet_color;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Destroy(gameObject, selfDestroy);
            currentSpeed = startSpeed;
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
            if (col.collider.CompareTag("Player"))
            {
                var hp = col.collider.GetComponent<ShipPlayerHealth>();
                if (hp) hp.TakeDamage(damage);
            }
        }
    }
}
