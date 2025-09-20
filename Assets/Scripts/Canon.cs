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
                return;
            }
            
            
            // Use bullet pool if available, otherwise instantiate
            GameObject bulletObj;
            if (BulletPool.Instance != null)
            {
                bulletObj = BulletPool.Instance.GetBullet();
                if (bulletObj != null)
                {
                    bulletObj.transform.position = transform.position;
                    bulletObj.transform.rotation = transform.rotation;
                }
            }
            else
            {
                bulletObj = Instantiate(bullet.gameObject, transform.position, transform.rotation);
            }
            
            if (bulletObj != null)
            {
                Bullet bulletScript = bulletObj.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    // Set bullet source based on who owns this canon
                    if (transform.root.CompareTag("Player"))
                    {
                        bulletScript.bulletSource = Bullet.BulletSource.Player;
                    }
                    else if (transform.root.CompareTag("Enemy"))
                    {
                        bulletScript.bulletSource = Bullet.BulletSource.Enemy;
                    }
                    else
                    {
                        // Fallback: Check if root name contains "Player" or "Enemy"
                        string rootName = transform.root.name.ToLower();
                        if (rootName.Contains("player"))
                        {
                            bulletScript.bulletSource = Bullet.BulletSource.Player;
                        }
                        else if (rootName.Contains("enemy") || rootName.Contains("star"))
                        {
                            bulletScript.bulletSource = Bullet.BulletSource.Enemy;
                        }
                        else
                        {
                            // Default to enemy if unclear
                            bulletScript.bulletSource = Bullet.BulletSource.Enemy;
                        }
                    }
                    
                    // Copy bullet properties
                    bulletScript.direction = bullet.direction;
                    bulletScript.startSpeed = bullet.startSpeed;
                    bulletScript.endSpeed = bullet.endSpeed;
                    bulletScript.decelTime = bullet.decelTime;
                    bulletScript.damage = bullet.damage;
                    bulletScript.selfDestroy = bullet.selfDestroy;
                }
                else
                {
                }
            }
            else
            {
            }
        }
    }
}
