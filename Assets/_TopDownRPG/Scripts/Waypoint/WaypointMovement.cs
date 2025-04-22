using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaypointMovement : MonoBehaviour
{
    public Transform WaypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] WayPoint;
    private int currentWaypointIndex;
    private Vector2 moveDirection;
    private bool isWaiting;
    private bool loopWaypoint;
    private bool isFacingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        WayPoint = new Transform[WaypointParent.childCount];
        for (int i = 0; i < WaypointParent.childCount; i++)
        {
            WayPoint[i] = WaypointParent.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (isWaiting)
        {
            return;
        }
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = WayPoint[currentWaypointIndex];

        // Calculer la direction de mouvement
        Vector2 direction = (target.position - transform.position).normalized;
        SetMoveDirection(direction); // Appeler SetMoveDirection pour gérer le flip

        // Déplacer le NPC vers le waypoint
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Vérifier si le NPC est arrivé au waypoint
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(waitAtWaypoint());
        }
    }

    IEnumerator waitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        //si lopping est activé incrémente currentWaypointIndex et wrap around si besoin
        //si loop pas incrémente currentWaypointIndex mais ne dépasse pas le nombre de waypoints

        currentWaypointIndex = loopWaypoint ? (currentWaypointIndex + 1) % WayPoint.Length : Mathf.Min(currentWaypointIndex + 1, WayPoint.Length - 1);

        isWaiting = false;

    }
    private void Flip()
    {
        Debug.Log("Flip");
        // Inverser l'orientation du sprite
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
        Debug.Log("SetMoveDirection");
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
}
