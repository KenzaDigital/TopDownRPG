// Ce script gère tout le système de combat du joueur: santé, mana, attaques et dégâts
// Il est au cœur du gameplay, permettant au joueur d'interagir avec les ennemis
using UnityEngine;
using UnityEngine.Events;  // Nécessaire pour utiliser UnityEvent
using UnityEngine.InputSystem; // Nécessaire pour utiliser InputSystem

public class PlayerCombatSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Mana Settings")]
    public int maxMana = 50;
    public int currentMana;

    [Header("Combat Settings")]
    public int attackDamage = 10;
    public float attackRange = 1.0f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayers;

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent<int, int> onManaChanged;
    public UnityEvent onPlayerDeath;
    public UnityEvent onPlayerAttack;

    private float lastAttackTime;
    private Animator animator;
    private bool isAttacking = false;

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
        if (Time.time - lastAttackTime < attackCooldown || isAttacking)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
        }

        onPlayerAttack?.Invoke();
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
            animator.SetTrigger("die");
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

        // Réinitialiser l'état d'attaque après avoir infligé des dégâts
        ResetAttack();
    }

    // Méthode pour réinitialiser l'état d'attaque
    public void ResetAttack()
    {
        isAttacking = false;
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
        Debug.Log("ResetAttack: isAttacking = " + isAttacking);
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
