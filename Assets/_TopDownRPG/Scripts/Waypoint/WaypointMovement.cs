using System.Collections;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    public Transform WaypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;   
    public bool loopWaypoints = true;

    private Transform[] WayPoint;
    private int currentWaypointIndex ;
    private bool isWaiting;
    private bool loopWaypoint;

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

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

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

        currentWaypointIndex = loopWaypoint ?(currentWaypointIndex + 1) % WayPoint.Length : Mathf.Min(currentWaypointIndex + 1, WayPoint.Length - 1);

        isWaiting = false;

    }
}
