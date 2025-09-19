using UnityEngine;

namespace manac.Assets.Scripts
{
    public class Canon : MonoBehaviour
    {
        public Bullet bullet;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Shoot()
        {
            if (bullet == null)
            {
                Debug.LogError($"{gameObject.name}: No bullet prefab assigned to canon!");
                return;
            }
            
            GameObject b = Instantiate(bullet.gameObject, transform.position, Quaternion.identity);
        }
    }
}
