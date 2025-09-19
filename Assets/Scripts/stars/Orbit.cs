using UnityEngine;

namespace manac.Assets.Scripts.stars
{
    public class Orbit : MonoBehaviour
    {
        [Header("Orbit Settings")]
        public Transform pivotPoint; 
        public float rotationSpeed = 15f; // Slow clockwise rotation
        public float orbitRadius = 3f;
        public bool autoFindClosestPivot = true;
        public bool maintainRadius = true;
        
        [Header("Movement Settings")]
        public bool useSmoothMovement = true;
        public float smoothSpeed = 5f;
        
        private Vector3 centerPosition;
        private float currentAngle;
        private Vector3 targetPosition;
        
        void Start()
        {
            // Auto-find closest pivot if enabled
            if (autoFindClosestPivot)
            {
                FindClosestPivot();
            }
            
            // Set initial position and angle
            if (pivotPoint != null)
            {
                centerPosition = pivotPoint.position;
                Vector3 directionToCenter = (centerPosition - transform.position).normalized;
                currentAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x);
                
                // Set initial orbit radius if maintaining radius
                if (maintainRadius)
                {
                    float distanceToCenter = Vector3.Distance(transform.position, centerPosition);
                    orbitRadius = distanceToCenter;
                }
            }
        }

        void Update()
        {
            if (pivotPoint != null)
            {
                // Update center position (in case pivot moves)
                centerPosition = pivotPoint.position;
                
                // Calculate target position on orbit
                currentAngle += rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;
                targetPosition = centerPosition + new Vector3(
                    Mathf.Cos(currentAngle) * orbitRadius,
                    Mathf.Sin(currentAngle) * orbitRadius,
                    0f
                );
                
                // Move to target position - use direct positioning to avoid vibration
                transform.position = targetPosition;
            }
        }
        
        private void FindClosestPivot()
        {
            // Find all possible pivot points (you can add more pivot types here)
            Transform[] possiblePivots = new Transform[0];
            
            // Add Spawn as a pivot point
            if (Spawn.Instance != null)
            {
                System.Array.Resize(ref possiblePivots, possiblePivots.Length + 1);
                possiblePivots[possiblePivots.Length - 1] = Spawn.Instance.transform;
            }
            
            // Find the closest pivot
            if (possiblePivots.Length > 0)
            {
                Transform closestPivot = possiblePivots[0];
                float closestDistance = Vector3.Distance(transform.position, closestPivot.position);
                
                for (int i = 1; i < possiblePivots.Length; i++)
                {
                    float distance = Vector3.Distance(transform.position, possiblePivots[i].position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPivot = possiblePivots[i];
                    }
                }
                
                pivotPoint = closestPivot;
                Debug.Log($"{gameObject.name} found closest pivot: {pivotPoint.name} at distance {closestDistance:F2}");
            }
        }
        
        // Public methods for runtime control
        public void SetPivot(Transform newPivot)
        {
            pivotPoint = newPivot;
            if (pivotPoint != null)
            {
                centerPosition = pivotPoint.position;
            }
        }
        
        public void SetOrbitRadius(float newRadius)
        {
            orbitRadius = newRadius;
        }
        
        public void SetRotationSpeed(float newSpeed)
        {
            rotationSpeed = newSpeed;
        }
    }
}