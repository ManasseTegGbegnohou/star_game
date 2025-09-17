using UnityEngine;
using System.Collections;

namespace manac.Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class Shooter : MonoBehaviour
    {

        public static float bulletSpray = 0.0f; // seconds between bullets in a spray
        private static readonly WaitForSeconds _waitForSecondsBulletSpray = new(bulletSpray);
        private static readonly WaitForSeconds _waitForSecondsRows = new(0.2f);
        public int health = 20;
        private static readonly WaitForSeconds _waitForSeconds = new(0.9f);
        private Rigidbody2D rb;
        private Animator animator;
        [SerializeField] private float fireRate = 3.0f; // seconds between shots
        private float cooldown;
        private Canon[] canons;

        [Header("Attack Settings")]
        [Range(0, 359)][SerializeField] private float angleSpread = 20;
        [SerializeField] private int bulletsPerAttack = 2;
        [SerializeField] private int attackCount = 2; // number of volleys per attack

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            canons = GetComponentsInChildren<Canon>();
            cooldown = fireRate;
        }

        void Update()
        {
            if (cooldown > 0)
                cooldown -= Time.deltaTime;

            if (cooldown <= 0)
            {
                cooldown = fireRate;
                StartCoroutine(ShootSequence());
            }

            animator.SetTrigger("idle");
        }

        private IEnumerator ShootSequence()
        {
            animator.SetTrigger("attack");
            yield return _waitForSeconds;

            if (ShipPlayer.Instance == null) yield break;

            // Get base direction towards player
            Vector2 targetDir = (ShipPlayer.Instance.transform.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

            // Spread setup
            float angleStep = (bulletsPerAttack > 1) ? angleSpread / (bulletsPerAttack - 1) : 0f;
            float startAngle = targetAngle - angleSpread / 2f;


            for (int j = 0; j < attackCount; j++)
            {
                foreach (Canon canon in canons)
                {
                    canon.bullet.startSpeed = 10f;
                    canon.bullet.endSpeed = 7f;
                    canon.bullet.decelTime = 0.3f;
                    for (int i = 0; i < bulletsPerAttack; i++)
                    {

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
