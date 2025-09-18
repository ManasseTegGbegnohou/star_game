using UnityEngine;

namespace manac.Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Spawn : MonoBehaviour
    {
        public Rigidbody2D rb;
        
        public static Spawn Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
        }
    }
}