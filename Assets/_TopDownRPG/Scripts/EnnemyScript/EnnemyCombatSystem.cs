using UnityEngine;
using UnityEngine.Events;


public class EnemyCombatSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth { get; private set; }

    [Header("Combat Settings")]
    public int attackDamage = 5;
    public float attackRange = 1.0f;
    public float attackCooldown = 1.0f;
    public float attackFrontDistance = 1.0f;
    public LayerMask playerLayer;

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
    private bool isDead = false; // Booléen pour vérifier si l'ennemi est déjà mort

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
        if (isDead) return; // Si l'ennemi est déjà mort, ne pas continuer

        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger("hit");
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
        if (isDead) return; // Si l'ennemi est déjà mort, ne pas continuer

        isDead = true; // Marquer l'ennemi comme mort

        if (animator != null)
        {
            animator.SetTrigger("Die");
            Debug.Log("Enemy is dead");
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

        // Désactiver les collisions et le renderer pour éviter les interférences
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        /*Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }*/

        // Détruire l'ennemi après un délai pour laisser l'animation se jouer
        Destroy(gameObject, 5.0f);

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.EnemyKilled();
        }

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

    public void OnAttackEvent()
    {
        // Déterminer la position de l'attaque devant le joueur
        Vector2 attackPosition = transform.position;

        // Récupérer l'orientation du joueur
        bool isFacingRight = transform.localScale.x > 0;

        // Décaler la position de l'attaque en fonction de l'orientation
        if (isFacingRight)
        {
            attackPosition.x += attackFrontDistance; // Attaque à droite
        }
        else
        {
            attackPosition.x -= attackFrontDistance; // Attaque à gauche
        }

        // Détecter les ennemis et leur infliger des dégâts
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRange,
            playerLayer
        );

        foreach (Collider2D player in hitPlayer)
        {
            PlayerCombatSystem playerCombatSystem = player.GetComponent<PlayerCombatSystem>();
            if (playerCombatSystem != null)
            {
                playerCombatSystem.TakeDamage(attackDamage);
            }
        }
    }
}
