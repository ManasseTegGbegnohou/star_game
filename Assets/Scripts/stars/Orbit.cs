using UnityEngine;

namespace manac.Assets.Scripts.stars
{
    public class Orbit : MonoBehaviour
    {
        public Transform pivotPoint; 
        public float rotationSpeed = 50f; 

        void Update()
        {
            if (pivotPoint != null)
            {
                // Rotate around the pivotPoint's position
                // Vector3.forward is used for rotation in the XY plane (2D)
                transform.RotateAround(Spawn.Instance.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        }
    }
}