using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace manac.Assets.Scripts
{
    public class HealthUI : MonoBehaviour
    {
    [Header("UI References")]
    public TextMeshProUGUI healthText;
    
        [Header("Display Settings")]
        public string healthFormat = "HP: {0}";
        public float fontSize = 34f;
        
        private ShipPlayerHealth playerHealth;
        
        void Start()
        {
            if (ShipPlayer.Instance != null)
            {
                playerHealth = ShipPlayer.Instance.GetComponent<ShipPlayerHealth>();
            }

            if (playerHealth == null)
            {
                playerHealth = FindObjectOfType<ShipPlayerHealth>();
            }
            
            if (playerHealth == null)
            {
                return;
            }
            
            if (healthText != null)
            {
                healthText.gameObject.SetActive(true);
                healthText.fontSize = fontSize;
                healthText.color = Color.yellow;
                
                // Set the position and sze
                RectTransform rectTransform = healthText.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(-520f, 250f);
                    rectTransform.sizeDelta = new Vector2(150f, rectTransform.sizeDelta.y);
                }
            }
            
            UpdateHealthDisplay();
        }
        
        void Update()
        {
            if (playerHealth != null)
            {
                UpdateHealthDisplay();
            }
        }
        
        private void UpdateHealthDisplay()
        {
            if (playerHealth == null) return;
            
            int currentHealth = playerHealth.GetCurrentHealth();
            
            // Update  txt
            if (healthText != null)
            {
                healthText.text = string.Format(healthFormat, currentHealth);
                healthText.color = Color.yellow;
            }
        }
    }
}
