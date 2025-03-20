using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayHUD : MonoBehaviour
{
    [Header("Player Health Bar")]
    public Slider healthSlider;              // Référence au slider pour la barre de vie
    public TextMeshProUGUI healthText;       // Texte affichant les valeurs numériques de santé

    [Header("Player Mana Bar")]
    public Slider manaSlider;                // Référence au slider pour la barre de mana
    public TextMeshProUGUI manaText;         // Texte affichant les valeurs numériques de mana

    private PlayerCombatSystem playerCombatSystem;  // Référence au système de combat du joueur

    private void Start()
    {
        // Trouver le joueur et récupérer son composant PlayerCombatSystem
        playerCombatSystem = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCombatSystem>();

        if (playerCombatSystem != null)
        {
            // S'abonner aux événements de changement de santé et de mana
            playerCombatSystem.onHealthChanged.AddListener(UpdateHealthUI);
            playerCombatSystem.onManaChanged.AddListener(UpdateManaUI);

            // Initialiser l'UI avec les valeurs actuelles
            UpdateHealthUI(playerCombatSystem.currentHealth, playerCombatSystem.maxHealth);
            UpdateManaUI(playerCombatSystem.currentMana, playerCombatSystem.maxMana);
        }
    }

    // Mise à jour de l'UI de santé
    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider == null || healthText == null)
        {
            return;
        }

        if (healthSlider.maxValue != maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }

        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    // Mise à jour de l'UI de mana
    private void UpdateManaUI(int currentMana, int maxMana)
    {
        if (manaSlider == null || manaText == null)
        {
            return;
        }

        if (manaSlider.maxValue != maxMana)
        {
            manaSlider.maxValue = maxMana;
        }

        manaSlider.value = currentMana;
        manaText.text = currentMana.ToString();
    }

}
