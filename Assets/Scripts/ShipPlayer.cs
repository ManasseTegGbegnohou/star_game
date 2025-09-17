using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace manac.Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class ShipPlayer : MonoBehaviour
    {
        private static readonly WaitForSeconds _waitForSeconds0_2 = new(0.2f);

        public static ShipPlayer Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }
        private readonly float speed = 7f;
        public float speedMultiplier = 1f;
        private readonly float fireRate = 0.6f;
        public float fireRateMultiplier = 1f;

        private Rigidbody2D rb;
        private Animator animator;
        private Vector2 moveInput;
        private float cooldown;
        private Canon[] canons;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            canons = GetComponentsInChildren<Canon>();
        }

        void Update()
        {
            rb.linearVelocity = moveInput * (speed * speedMultiplier);

            //Rotate
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 lookDir = (mousePos - transform.position).normalized;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

            if (cooldown > 0)
                cooldown -= Time.deltaTime;

            if ((Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) && cooldown <= 0)
            {
                cooldown = fireRate / fireRateMultiplier;
                StartCoroutine(ShootSequence());
            }
        }

        private IEnumerator ShootSequence()
        {
            animator.SetTrigger("shoot");
            yield return _waitForSeconds0_2;

            foreach (Canon canon in canons)
            {
                canon.bullet.direction = transform.up;
                canon.bullet.selfDestroy = 1.5f;
                canon.Shoot();
            }
        }

        public void Move(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }
}
