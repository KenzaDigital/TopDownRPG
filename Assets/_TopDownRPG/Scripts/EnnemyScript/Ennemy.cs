using UnityEngine;


public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTime = 1f;

    [Header("Detection Settings")]
    public Transform playerTransform;     // R�f�rence au joueur
    public float detectionRadius = 4f;    // Rayon de d�tection
    public float chaseSpeed = 2.5f;       // Vitesse de poursuite
    public float chaseTime = 4f;          // Dur�e de poursuite apr�s avoir perdu le joueur
    public LayerMask obstacleLayer;       // Layer des obstacles

    private int currentPointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Rigidbody2D rb;

    // Variables pour la poursuite
    private bool isChasing = false;
    private float chaseTimer = 0f;
    private float currentSpeed;

    // �tats possibles de l'ennemi
    private enum EnemyState
    {
        Patrol,
        Chase
    }

    private EnemyState currentState = EnemyState.Patrol;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = patrolSpeed;

        // Chercher le joueur s'il n'est pas assign�
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (!playerTransform)
            return;

        // D�terminer l'�tat actuel en fonction de la distance au joueur
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // V�rifier si le joueur est dans le rayon de d�tection
        if (distanceToPlayer <= detectionRadius)
        {
            // V�rifier s'il y a une ligne de vue vers le joueur
            bool hasLineOfSight = !Physics2D.Raycast(transform.position,
                (playerTransform.position - transform.position).normalized,
                distanceToPlayer, obstacleLayer);

            if (hasLineOfSight)
            {
                // Le joueur est visible, commencer la poursuite
                currentState = EnemyState.Chase;
                isChasing = true;
                chaseTimer = chaseTime;
            }
        }

        // Si on est en poursuite mais que le joueur n'est plus visible
        if (isChasing && currentState == EnemyState.Chase && distanceToPlayer > detectionRadius)
        {
            // R�duire le temps de poursuite
            chaseTimer -= Time.deltaTime;

            // Si le temps est �coul�, retourner � la patrouille
            if (chaseTimer <= 0)
            {
                isChasing = false;
                currentState = EnemyState.Patrol;
            }
        }

        // Ex�cuter le comportement correspondant � l'�tat actuel
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;
        }
    }

    private void HandlePatrol()
    {
        currentSpeed = patrolSpeed;

        // Logique de patrouille (m�me que dans la partie 1)
        if (patrolPoints.Length == 0)
            return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                isWaiting = false;
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }

            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetPosition = patrolPoints[currentPointIndex].position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * currentSpeed;

        // Orienter l'ennemi
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1);
        }

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            isWaiting = true;
            waitTimer = waitTime;
        }
    }

    private void HandleChase()
    {
        // Utiliser la vitesse de poursuite (plus rapide)
        currentSpeed = chaseSpeed;

        // Calculer la direction vers le joueur
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // D�placer l'ennemi vers le joueur
        rb.linearVelocity = direction * currentSpeed;

        // Orienter l'ennemi vers le joueur
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dessiner le rayon de d�tection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
