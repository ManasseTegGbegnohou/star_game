using UnityEngine;
using System.Collections.Generic;

namespace manac.Assets.Scripts
{
    public class BulletPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int poolSize = 100;
        [SerializeField] private bool expandPool = true;
        
        private Queue<GameObject> bulletPool = new Queue<GameObject>();
        private List<GameObject> activeBullets = new List<GameObject>();
        
        public static BulletPool Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        
        void Start()
        {
            // Initialize pool in Start to ensure all references are set
            if (bulletPrefab != null)
            {
                InitializePool();
            }
            else
            {
            }
        }
        
        void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.SetActive(false);
                bullet.transform.SetParent(transform);
                bulletPool.Enqueue(bullet);
            }
        }
        
        public GameObject GetBullet()
        {
            if (bulletPrefab == null)
            {
                return null;
            }
            
            GameObject bullet;
            
            if (bulletPool.Count > 0)
            {
                bullet = bulletPool.Dequeue();
            }
            else if (expandPool)
            {
                bullet = Instantiate(bulletPrefab);
                bullet.transform.SetParent(transform);
            }
            else if (activeBullets.Count > 0)
            {
                // Reuse oldest active bullet if pool is full
                bullet = activeBullets[0];
                activeBullets.RemoveAt(0);
                bullet.SetActive(false);
            }
            else
            {
                return null;
            }
            
            bullet.SetActive(true);
            activeBullets.Add(bullet);
            return bullet;
        }
        
        public void ReturnBullet(GameObject bullet)
        {
            if (bullet == null) return;
            
            bullet.SetActive(false);
            bullet.transform.SetParent(transform);
            bullet.transform.position = Vector3.zero;
            bullet.transform.rotation = Quaternion.identity;
            
            // Reset bullet properties
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.ResetBullet();
            }
            
            activeBullets.Remove(bullet);
            bulletPool.Enqueue(bullet);
        }
        
        public void ReturnAllBullets()
        {
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                ReturnBullet(activeBullets[i]);
            }
        }
        
        public int GetActiveBulletCount()
        {
            return activeBullets.Count;
        }
        
        public int GetPooledBulletCount()
        {
            return bulletPool.Count;
        }
    }
}
