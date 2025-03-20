using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthBarUI : MonoBehaviour
{
    // --- SECTION 1: DÉCLARATION DES VARIABLES ---

    public Slider healthSlider;                  // Référence au slider de la barre de santé
    public Vector3 offset = new Vector3(0, 0.5f, 0);  // Décalage au-dessus de l'ennemi (en unités de monde)

    private EnemyCombatSystem enemyCombatSystem;        // Référence au système de combat de l'ennemi
    private Transform target;                    // Transforme de l'ennemi à suivre
    private Camera mainCamera;


    // --- SECTION 2: INITIALISATION ---

    private void Start()
    {
        mainCamera = Camera.main;
        // Récupérer la référence à l'ennemi parent
        enemyCombatSystem = GetComponentInParent<EnemyCombatSystem>();

        if (enemyCombatSystem != null)
        {
            // S'abonner à l'événement de changement de santé
            enemyCombatSystem.onHealthChanged.AddListener(UpdateHealthUI);

            // Initialiser l'UI avec les valeurs actuelles
            UpdateHealthUI(enemyCombatSystem.currentHealth, enemyCombatSystem.maxHealth);

            // Stocker la référence au transform de l'ennemi
            target = enemyCombatSystem.transform;
        }
        else
        {
            Debug.LogError("EnemyHealthBarUI: EnemyCombatSystem not found on parent!");
        }

        // Positionner initialement la barre de santé au-dessus de l'ennemi
        if (target != null)
        {
            // Pour un Canvas en World Space, nous devons positionner directement le Canvas
            // par rapport à l'ennemi
            transform.position = target.position + offset;

            // Faire face à la caméra
            FaceCamera();
        }
    }

    // --- SECTION 3: SUIVI DE L'ENNEMI ---

    private void LateUpdate()
    {
        if (target)
        {
            // Mettre à jour la position pour suivre l'ennemi
            transform.position = target.position + offset;

            // S'assurer que la barre de santé fait toujours face à la caméra
            FaceCamera();
        }
    }

    // Faire pivoter le Canvas pour qu'il fasse face à la caméra
    private void FaceCamera()
    {
        if (mainCamera)
        {
            // Pour une vue top-down 2D typique, nous voulons généralement que l'UI
            // soit alignée avec l'écran, donc nous utilisons la rotation de la caméra
            transform.rotation = mainCamera.transform.rotation;

            // Alternative pour les jeux 3D où la caméra peut tourner dans tous les axes:
            // transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }

    // --- SECTION 4: MISE À JOUR DE L'UI ---
    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider == null)
        {
            return;
        }


        if (healthSlider.maxValue != maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }
        healthSlider.value = currentHealth;

        // Optionnellement, masquer la barre de santé si l'ennemi est à pleine vie
        // gameObject.SetActive(currentHealth < maxHealth);

    }

    // --- SECTION 5: GESTION DE LA VISIBILITÉ ---

    // Ces méthodes seront appelées si le Renderer de l'ennemi parent devient visible/invisible
    // Note: Pour que cela fonctionne, ce script doit être sur le même GameObject que le Renderer
    // ou vous devez utiliser l'approche avec EnemyVisibilityObserver expliquée plus loin
    private void OnBecameVisible()
    {
        // Activer la barre de santé
        gameObject.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        // Désactiver la barre de santé
        gameObject.SetActive(false);
    }

    // --- SECTION 6: NETTOYAGE ---
    private void OnDestroy()
    {
        // Se désabonner de l'événement pour éviter les fuites de mémoire
        if (enemyCombatSystem != null)
        {
            enemyCombatSystem.onHealthChanged.RemoveListener(UpdateHealthUI);
        }
    }
}
