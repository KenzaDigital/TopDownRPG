// Ce script gère tout le système de combat du joueur: santé, mana, attaques et dégâts
// Il est au cœur du gameplay, permettant au joueur d'interagir avec les ennemis
using UnityEngine;
using UnityEngine.Events;  // Nécessaire pour utiliser UnityEvent


public class PlayerCombatSystem : MonoBehaviour
{
    // --- PARAMÈTRES DE SANTÉ ---
    [Header("Health Settings")]
    public int maxHealth = 100;      // Santé maximale que le joueur peut avoir
    public int currentHealth;        // Santé actuelle du joueur (initialisée dans Start)

    // --- PARAMÈTRES DE MANA ---
    [Header("Mana Settings")]
    public int maxMana = 50;         // Mana maximale que le joueur peut avoir
    public int currentMana;          // Mana actuelle du joueur (initialisée dans Start)

    // --- PARAMÈTRES DE COMBAT ---
    [Header("Combat Settings")]
    public int attackDamage = 10;    // Quantité de dégâts infligés par une attaque
    public float attackRange = 1.0f;  // Distance à laquelle le joueur peut attaquer
    public float attackCooldown = 0.5f; // Temps minimum entre deux attaques (en secondes)
    public LayerMask enemyLayers;    // Couches (Layers) qui contiennent les ennemis

    // --- ÉVÉNEMENTS ---
    // Les UnityEvents permettent de connecter ce script à d'autres systèmes (comme l'UI)
    // sans créer de dépendances directes
    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;  // Déclenché quand la santé change (santé actuelle, santé max)
    public UnityEvent<int, int> onManaChanged;    // Déclenché quand la mana change (mana actuelle, mana max)
    public UnityEvent onPlayerDeath;              // Déclenché quand le joueur meurt

    // --- VARIABLES PRIVÉES ---
    private float lastAttackTime;    // Moment de la dernière attaque (pour le cooldown)
    private Animator animator;       // Référence au composant Animator pour les animations

    // Awake est appelé quand l'objet est initialisé, avant Start
    private void Awake()
    {
        // Récupérer le composant Animator attaché à ce même GameObject
        animator = GetComponent<Animator>();
    }

    // Start est appelé avant la première mise à jour
    private void Start()
    {
        // Initialiser la santé et la mana à leurs valeurs maximales
        currentHealth = maxHealth;
        currentMana = maxMana;

        // Déclencher les événements pour initialiser l'UI
        // Le ? avant Invoke est un "opérateur de propagation nulle"
        // qui vérifie si onHealthChanged n'est pas null avant d'appeler Invoke
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onManaChanged?.Invoke(currentMana, maxMana);
    }

    // [ContextMenu] permet de tester cette fonction directement depuis l'inspecteur Unity
    // en faisant un clic droit sur le composant
    [ContextMenu("Attack")]
    public void Attack()
    {
        // Vérifier si le cooldown est terminé
        if (Time.time - lastAttackTime < attackCooldown)
            return;  // Sortir de la fonction si le cooldown n'est pas terminé

        // Mettre à jour le temps de la dernière attaque
        lastAttackTime = Time.time;

        // Jouer l'animation d'attaque si un Animator existe
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Note: Ici, il manque le code qui détecterait et infligerait des dégâts aux ennemis
        // C'est probablement intentionnel pour simplifier l'exemple ou sera ajouté plus tard
    }

    // Fonction pour que le joueur subisse des dégâts
    // [ContextMenu] permet de tester cette fonction depuis l'inspecteur
    [ContextMenu("TakeDamage")]
    public void TakeDamage(int attackDamage)
    {
        // Dans cette version, les dégâts sont fixés à 10 pour simplifier
        int damage = 10;

        // Réduire la santé par le montant de dégâts
        currentHealth -= damage;

        // Déclencher l'événement de changement de santé pour mettre à jour l'UI
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        // Message de débogage dans la console
        Debug.Log("TakeDamage");

        // Jouer l'animation de dégâts si un Animator existe
        if (animator)
        {
            Debug.Log("Hit");
            animator.SetTrigger("Hit");
        }

        // Vérifier si le joueur est mort (santé ≤ 0)
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Fonction pour restaurer de la santé (par exemple avec une potion)
    public void RestoreHealth(int amount)
    {
        // Augmenter la santé mais sans dépasser le maximum
        // Mathf.Min retourne la plus petite valeur entre ses deux arguments
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // Mettre à jour l'UI
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Fonction pour restaurer de la mana (par exemple avec une potion)
    public void RestoreMana(int amount)
    {
        // Augmenter la mana mais sans dépasser le maximum
        currentMana = Mathf.Min(currentMana + amount, maxMana);

        // Mettre à jour l'UI
        onManaChanged?.Invoke(currentMana, maxMana);
    }

    // Fonction pour utiliser de la mana (par exemple pour lancer un sort)
    // Retourne true si le joueur a assez de mana, false sinon
    public bool UseMana(int amount)
    {
        // Vérifier si le joueur a assez de mana
        if (currentMana >= amount)
        {
            // Réduire la mana par le montant utilisé
            currentMana -= amount;

            // Mettre à jour l'UI
            onManaChanged?.Invoke(currentMana, maxMana);

            // Le joueur avait assez de mana
            return true;
        }

        // Le joueur n'avait pas assez de mana
        return false;
    }

    // Fonction privée appelée quand le joueur meurt
    private void Die()
    {
        // Jouer l'animation de mort si un Animator existe
        if (animator)
        {
            animator.SetTrigger("Die");
        }

        // Désactiver le contrôleur de joueur pour empêcher tout mouvement
        CharacterController playerController = GetComponent<CharacterController>();
        if (playerController)
            playerController.enabled = false;

        // Déclencher l'événement de mort
        // Cela peut être utilisé par d'autres systèmes pour réagir à la mort du joueur
        onPlayerDeath?.Invoke();

        // Remarque: D'autres actions pourraient être ajoutées ici
        // Comme afficher un écran de game over, jouer un son, etc.
    }
}
