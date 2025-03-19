using UnityEngine;


public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;  // Points de patrouille
    public float patrolSpeed = 2f;   // Vitesse de déplacement
    public float waitTime = 1f;  // Temps d'attente à chaque point

    [Header("Detection Settings")]
    public Transform playerTransform;     // Référence au joueur
    public float detectionRadius = 4f;    // Rayon de détection
    public float chaseSpeed = 2.5f;       // Vitesse de poursuite
    public float chaseTime = 4f;          // Durée de poursuite après avoir perdu le joueur
    public LayerMask obstacleLayer;       // Layer des obstacles

    [Header("Obstacle Avoidance")]
    public float obstacleDetectionDistance = 1.0f;
    public float avoidanceStrength = 1.5f;

    private int currentPointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float lastAttackTime = 0f;    // Pour le cooldown d'attaque

    private Rigidbody2D rb; // Référence au Rigidbody2D
    private EnemyCombatSystem enemyCombatSystem; // Référence à EnemyCombatSystem (pour les dégâts)
    private PlayerCombatSystem playerCombatSystem;

    // Variables pour la poursuite
    private bool isChasing = false;
    private float chaseTimer = 0f;
    private float currentSpeed;

    // États possibles de l'ennemi
    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    private EnemyState currentState = EnemyState.Patrol;

    private void Awake()
    {
        // Récupérer le composant Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = patrolSpeed;

        // Récupérer le composant EnemyCombatSystem
        enemyCombatSystem = GetComponent<EnemyCombatSystem>();

        // Chercher le joueur s'il n'est pas assigné
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerCombatSystem = player.GetComponent<PlayerCombatSystem>();
            }
        }
        else
        {
            playerCombatSystem = playerTransform.GetComponent<PlayerCombatSystem>();
        }
    }

    private void Update()
    {
        if (!playerTransform)
            return;

        // Calculer la distance au joueur
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Déterminer l'état en fonction de la distance
        if (distanceToPlayer <= enemyCombatSystem.attackFrontDistance)
        {
            // Le joueur est à portée d'attaque
            currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer <= detectionRadius && currentState!= EnemyState.Attack)  // Vérifier si le joueur est dans le rayon de détection
        {
            // Vérifier s'il y a une ligne de vue vers le joueur
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
            else
            {
                isChasing = false;
                currentState = EnemyState.Patrol;
            }
        }
        else if (isChasing)
        {
            // Réduire le temps de poursuite
            chaseTimer -= Time.deltaTime;
            // Si le temps est écoulé, retourner à la patrouille
            if (chaseTimer <= 0)
            {
                isChasing = false;
                currentState = EnemyState.Patrol;
            }
        }
        else
        {
            currentState = EnemyState.Patrol;
        }

        // Exécuter le comportement correspondant à l'état actuel
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
        }
    }

    private void HandlePatrol()
    {
        currentSpeed = patrolSpeed;

        // Logique de patrouille (même que dans la partie 1)
        if (patrolPoints.Length == 0)
            return;

        // Si l'ennemi attend à un point de patrouille
        if (isWaiting)
        {
            // Décrémenter le compteur d'attente
            waitTimer -= Time.deltaTime;

            // Si le temps d'attente est écoulé
            if (waitTimer <= 0)
            {
                isWaiting = false; // Arrêter d'attendre
                                   // Passer au point suivant (en bouclant si nécessaire)
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }

            rb.linearVelocity = Vector2.zero;
            // Ne pas continuer le reste de la fonction pendant l'attente
            return;
        }

        // Récupérer la position cible actuelle
        Vector2 targetPosition = patrolPoints[currentPointIndex].position;

        // Calculer la direction vers la cible
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Déplacer l'ennemi vers la cible
        rb.linearVelocity = direction * currentSpeed;

        // Orienter l'ennemi
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        }

        // Vérifier si l'ennemi est arrivé au point de patrouille
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Arrêter le mouvement
            rb.linearVelocity = Vector2.zero;

            // Commencer à attendre
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


        if (Vector2.Distance(transform.position, playerTransform.position) > enemyCombatSystem.attackFrontDistance)
        {
            // Déplacer l'ennemi vers le joueur
            rb.linearVelocity = direction * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Orienter l'ennemi vers le joueur
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        }
    }

    private void HandleAttack()
    {
        // Arrêter le mouvement pendant l'attaque
        rb.linearVelocity = Vector2.zero;

        if (!playerTransform || !enemyCombatSystem)
        {
            return;
        }

        // Orienter l'ennemi vers le joueur
        float directionX = playerTransform.position.x - transform.position.x;
        transform.localScale = new Vector3(directionX > 0 ? 1 : -1, 1, 1);

        // Vérifier si le cooldown d'attaque est terminé
        if (Time.time - lastAttackTime >= enemyCombatSystem.attackCooldown)
        {
            // Attaquer le joueur
            if (playerCombatSystem)
            {
                // Classe qui va attaquer le joueur
                enemyCombatSystem.Attack(playerCombatSystem);
                lastAttackTime = Time.time;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dessiner le rayon de détection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
