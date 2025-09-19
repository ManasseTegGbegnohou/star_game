using UnityEngine;

namespace manac.Assets.Scripts.stars
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Orbit))]
    public class Star : Shooter
    {
        [Header("Star Settings")]
        [SerializeField] private float orbitRadius = 3f;
        [SerializeField] private float rotationSpeed = 15f; // Slow clockwise rotation
        
        private Orbit orbitComponent;
        
        void Start()
        {
            // Call parent Start() to initialize shooting
            base.Start();
            
            // Get the Orbit component
            orbitComponent = GetComponent<Orbit>();
            
            // Configure orbit settings - simple and consistent
            if (orbitComponent != null)
            {
                orbitComponent.SetOrbitRadius(orbitRadius);
                orbitComponent.SetRotationSpeed(rotationSpeed);
            }
        }

        void Update()
        {
            // Call parent Update() to handle shooting
            base.Update();
            
            // Star-specific behavior can be added here
            // The orbital movement is handled by the Orbit component
        }
        
        // Public methods for runtime control
        public void ChangeOrbitRadius(float newRadius)
        {
            orbitRadius = newRadius;
            if (orbitComponent != null)
            {
                orbitComponent.SetOrbitRadius(newRadius);
            }
        }
        
        public void ChangeRotationSpeed(float newSpeed)
        {
            rotationSpeed = newSpeed;
            if (orbitComponent != null)
            {
                orbitComponent.SetRotationSpeed(newSpeed);
            }
        }
        
        public void SetNewPivot(Transform newPivot)
        {
            if (orbitComponent != null)
            {
                orbitComponent.SetPivot(newPivot);
            }
        }
    }
}
