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
        private readonly float speed = 6f;
        public float speedMultiplier = 1f;
        private readonly float fireRate = 0.35f;
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
            
            // Ensure Rigidbody2D is set up for movment
            if (rb != null)
            {
                rb.gravityScale = 0f; // No gravity in 2D space gam
                rb.linearDamping = 0f; // No air resistnce
                rb.angularDamping = 0f; // No angular resistnce
                rb.freezeRotation = true; // Dont rotate from physics
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Only freze rotation
            }
        }

        void Update()
        {
            // Use new Input Sytem for movement
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
            
            // Use keyboard input if no Input Sytem input
            if (moveInput.magnitude < 0.1f && keyboardInput.magnitude > 0.1f)
            {
                moveInput = keyboardInput;
            }
            
            
            // Movement with WASD/Arrow kys
            if (moveInput.magnitude > 0.1f)
            {
                // Use transform movement (more relible)
                transform.Translate(moveInput * (speed * speedMultiplier) * Time.deltaTime);
            }

            //Rotat
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
            // Start shoot animaton
            animator.SetTrigger("shoot");
            
            // Calculate direction towards cursor for shootng
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 shootDirection = (mousePos - transform.position).normalized;

            foreach (Canon canon in canons)
            {
                // Set bullet properties before shootng
                canon.bullet.direction = shootDirection; // Shoot towards cursr
                canon.bullet.selfDestroy = 1.5f;
                canon.bullet.startSpeed = 10f;
                canon.bullet.endSpeed = 8f;
                canon.bullet.decelTime = 0.5f;
                canon.bullet.damage = 1; // Player bullets do 1 dmg
                canon.Shoot();

            }
            
            // Wait for shoot animaton to play
            yield return _waitForSeconds0_2;
            
            // Reset trigger to alow next shot
            animator.ResetTrigger("shoot");
        }

        public void Move(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }
}
