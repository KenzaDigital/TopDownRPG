using System.Windows.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [Header("Références")]
    [SerializeField] private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isFacingRight = true;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
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
    }

    private void FixedUpdate()
    {
        // On utilise FixedUpdate pour le mouvement physique
        Move();
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
}
