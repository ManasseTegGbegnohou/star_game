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
        private readonly float speed = 5.5f;
        public float speedMultiplier = 1f;
        private readonly float fireRate = 0.2f;
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
            
            // Ensure Rigidbody2D is set up for movement
            if (rb != null)
            {
                rb.gravityScale = 0f; // No gravity in 2D space game
                rb.linearDamping = 0f; // No air resistance
                rb.angularDamping = 0f; // No angular resistance
                rb.freezeRotation = true; // Don't rotate from physics
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Only freeze rotation
                Debug.Log("Rigidbody2D configured for movement");
            }
        }

        void Update()
        {
            // Use new Input System for movement
            Vector2 keyboardInput = Vector2.zero;
            
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                    keyboardInput.y += 1;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                    keyboardInput.y -= 1;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    keyboardInput.x += 1;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    keyboardInput.x -= 1;
            }
            
            // Use keyboard input if no Input System input
            if (moveInput.magnitude < 0.1f && keyboardInput.magnitude > 0.1f)
            {
                moveInput = keyboardInput;
            }
            
            
            // Movement with WASD/Arrow keys
            if (moveInput.magnitude > 0.1f)
            {
                // Use transform movement (more reliable)
                transform.Translate(moveInput * (speed * speedMultiplier) * Time.deltaTime);
            }

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

            // Calculate direction towards cursor for shooting
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 shootDirection = (mousePos - transform.position).normalized;

            foreach (Canon canon in canons)
            {
                canon.bullet.direction = shootDirection; // Shoot towards cursor
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
