using UnityEngine;

namespace manac.Assets.Scripts
{
    public class Spawn : MonoBehaviour
    {
        public static Spawn Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }
    }
}