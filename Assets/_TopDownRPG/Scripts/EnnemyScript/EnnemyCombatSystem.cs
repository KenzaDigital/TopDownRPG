using UnityEngine;
using UnityEngine.Events;


public class EnemyCombatSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    [SerializeField]
    private int currentHealth;

    [Header("Combat Settings")]
    public int attackDamage = 5;
    public float attackRange = 1.0f;
    public float attackCooldown = 1.0f;

    [Header("Reward Settings")]
    public int experienceReward = 10;
    public GameObject[] possibleDrops;
    [Range(0, 1)]
    public float dropChance = 0.3f;

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent onEnemyDeath;

    private float lastAttackTime;
    private Animator animator;
    private Enemy enemy;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Attack(PlayerCombatSystem player)
    {
        if (Time.time - lastAttackTime < attackCooldown || player.currentHealth <= 0)
            return;

        lastAttackTime = Time.time;

        if (animator)
            animator.SetTrigger("Attack");

        player.TakeDamage(attackDamage);
    }

    private void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (enemy != null)
        {
            enemy.enabled = false;
        }

        onEnemyDeath?.Invoke();

        PlayerCombatSystem player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCombatSystem>();
        if (player != null)
        {
            Debug.Log("Joueur reçoit " + experienceReward + " points d'expérience");
        }

        DropItem();
        Destroy(gameObject, 2.0f);
    }

    private void DropItem()
    {
        if (possibleDrops.Length == 0 || Random.value > dropChance)
            return;

        int randomIndex = Random.Range(0, possibleDrops.Length);
        GameObject drop = possibleDrops[randomIndex];

        if (drop != null)
        {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
