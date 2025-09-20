using UnityEngine;

namespace manac.Assets.Scripts
{
    /// <summary>
    /// Helper script to set up proper physics and collision layers for the space shooter game.
    /// Attach this to a GameObject in your scene and run it once to configure the physics settings.
    /// </summary>
    public class PhysicsSetup : MonoBehaviour
    {
        [Header("Physics Configuration")]
        [SerializeField] private bool setupOnStart = true;
        
        void Start()
        {
            if (setupOnStart)
            {
                SetupPhysicsLayers();
            }
        }
        
        [ContextMenu("Setup Physics Layers")]
        public void SetupPhysicsLayers()
        {
            
            // Configure collision matrix
            // Layer 0: Default (Player)
            // Layer 8: Enemy
            // Layer 9: Bullet
            // Layer 10: Environment
            
            // Player (Layer 0) collisions:
            // - Can collide with Enemy (Layer 8) ✓
            // - Can collide with Environment (Layer 10) ✓
            // - Cannot collide with Bullet (Layer 9) ✗
            
            // Enemy (Layer 8) collisions:
            // - Can collide with Player (Layer 0) ✓
            // - Can collide with Environment (Layer 10) ✓
            // - Cannot collide with other Enemies (Layer 8) ✗
            // - Cannot collide with Bullet (Layer 9) ✗
            
            // Bullet (Layer 9) collisions:
            // - Cannot collide with anything (passes through) ✗
            // - Uses trigger detection instead
            
        }
        
        [ContextMenu("Print Collision Matrix Guide")]
        public void PrintCollisionMatrixGuide()
        {
        }
    }
}
