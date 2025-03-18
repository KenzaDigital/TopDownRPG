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
    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;  // Déclenché quand la santé change (santé actuelle, santé max)
    public UnityEvent<int, int> onManaChanged;    // Déclenché quand la mana change (mana actuelle, mana max)
    public UnityEvent onPlayerDeath;              // Déclenché quand le joueur meurt
    public UnityEvent onPlayerAttack;             // Déclenché quand le joueur attaque

    // --- VARIABLES PRIVÉES ---
    private float lastAttackTime;    // Moment de la dernière attaque (pour le cooldown)
    private Animator animator;       // Référence au composant Animator pour les animations

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onManaChanged?.Invoke(currentMana, maxMana);
    }

    [ContextMenu("Attack")]
    public void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamage(int attackDamage)
    {
        int damage = 10;
        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log("TakeDamage");

        if (animator)
        {
            Debug.Log("Hit");
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        onManaChanged?.Invoke(currentMana, maxMana);
    }

    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            onManaChanged?.Invoke(currentMana, maxMana);
            return true;
        }
        return false;
    }

    private void Die()
    {
        if (animator)
        {
            animator.SetTrigger("Die");
        }

        CharacterController playerController = GetComponent<CharacterController>();
        if (playerController)
            playerController.enabled = false;

        onPlayerDeath?.Invoke();
    }

    // Méthode d'événement d'animation
    public void OnAttackEvent()
    {
        Vector2 attackPosition = transform.position;
        bool isFacingRight = transform.localScale.x > 0;

        if (isFacingRight)
        {
            attackPosition.x += 0.75f;
        }
        else
        {
            attackPosition.x -= 0.75f;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyCombatSystem enemyStats = enemy.GetComponent<EnemyCombatSystem>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackPosition = transform.position;
        bool isFacingRight = transform.localScale.x > 0;

        if (isFacingRight)
        {
            attackPosition.x += 0.75f;
        }
        else
        {
            attackPosition.x -= 0.75f;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, attackPosition);
    }
}
