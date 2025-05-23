using System;
using System.Collections;
using System.Windows.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.1f;
    [Header("Références")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator; // Référence à l'Animator
    private Vector2 moveDirection;
    private bool isFacingRight = true;
    private bool isDashing = false;

    // Déclaration des booléens
    private bool isMoving = false; // Ici, le joueur est en mouvement ou non
    private bool isAttacking = false; // Booléen pour indiquer si le joueur est en train d'attaquer
    public Transform Player ; // Référence au joueur

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>(); // Utiliser GetComponent car l'Animator est sur le GameObject principal
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.SetPlayerInput(playerInput);
            }
            else
            {
                Debug.LogError("InputManager is not in the scene");
            }
        }
        else
        {
            Debug.LogError("Missing PlayerInput on GameObject");
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;

        // Gestion de l'orientation du sprite
        
        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }

        // Mettre à jour le booléen isMoving
        isMoving = direction != Vector2.zero;

        // Mettre à jour l'Animator
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }
    }

    private void FixedUpdate()
    {
        // On utilise FixedUpdate pour le mouvement physique
        if (!isDashing)
        {
            Move();
        }
    }

    private void Move()
    {
        // Mouvement avec Rigidbody2D pour une meilleure physique
        if (rb)
        {
            rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        }
        else
        {
            Debug.LogError("Rigidbody2D is missing on CharacterController");
        }
    }

    private void Flip()
    {
        // Inverser l'orientation du sprite
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
       
    }

    public void Dash()
    {
        Debug.Log("Dash");
        if (!isDashing && moveDirection != Vector2.zero)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        float dashTime = 0f;

        while (dashTime < dashDuration)
        {
            rb.linearVelocity = moveDirection.normalized * dashSpeed;
            dashTime += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    void Update()
    {
        if (Player != null)
        {
            Vector2 direction = (Player.position - transform.position).normalized;
            SetMoveDirection(direction);
        }
    }
}
