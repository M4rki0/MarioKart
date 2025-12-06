using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SmartAIKartController : MonoBehaviour
{
    [Header("Racing Line")]
    public List<Transform> waypoints;
    public float baseSpeed = 15f;
    public float turnspeed = 5f;
    public float waypointRadius = 3f;

    [Header("Behaviour Settings")]
    public Transform player;
    public float laneOffsetRange;
    public float boostSpeed;
    public float rubberbandDistance = 30f;

    private int currentWaypoint = 0;
    private Rigidbody rb;
    private float offsetX;
    private float currentSpeed;

    private float itemUseCooldown = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        offsetX = Random.Range(-laneOffsetRange, laneOffsetRange);
        currentSpeed = baseSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (waypoints.Count == 0 || player == null) return;

        Vector3 baseTarget = waypoints[currentWaypoint].position;
        Vector3 offset = waypoints[currentWaypoint].right * offsetX;
        Vector3 target = baseTarget + offset;

        Vector3 direction = (target - transform.position).normalized;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, turnspeed * Time.fixedDeltaTime, 0.0f);
        
        rb.MoveRotation(Quaternion.LookRotation(newDir));

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > rubberbandDistance) currentSpeed = baseSpeed + boostSpeed;
        else if (distanceToPlayer < -rubberbandDistance) currentSpeed = baseSpeed - 3f;
        else currentSpeed = baseSpeed;
        
        rb.MovePosition(rb.position + transform.forward * currentSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, baseTarget) < waypointRadius)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
            offsetX = Random.Range(-laneOffsetRange, laneOffsetRange);
        }

        itemUseCooldown -= Time.fixedDeltaTime;
        if (itemUseCooldown <= 0f)
        {
            TryUseItem();
            itemUseCooldown = Random.Range(3f, 6f);
        }

        AvoidBananas();
    }

    void TryUseItem()
    {
        // TODO: hook into your item system
        float roll = Random.value;

        if (roll < 0.3f)
        {
            Debug.Log(name + "used a banana!");
            // DropBanana();
        }
        else if (roll < 0.6)
        {
            Debug.Log(name + " used a boost!");
            // UseSpeedBoost();
        }
    }

    void AvoidBananas()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 10f))
        {
            if (hit.collider.CompareTag("Banana"))
            {
                offsetX = -offsetX;
                Debug.Log(name + " dodged a banana!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Count == 0) return;
        
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Count; i++)
        {
            Transform wp = waypoints[i];
            if (wp == null) continue;
            
            Gizmos.DrawSphere(wp.position, 1f);

            Transform nextWp = waypoints[(i + 1) % waypoints.Count];
            if (nextWp != null)
            {
                Gizmos.DrawLine(wp.position, nextWp.position);
            }
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(wp.position, wp.position + wp.right * laneOffsetRange);
            Gizmos.DrawLine(wp.position, wp.position - wp.right * laneOffsetRange);
            Gizmos.color = Color.green;
        }

        if (Application.isPlaying && waypoints.Count > currentWaypoint)
        {
            Vector3 baseTarget = waypoints[currentWaypoint].position;
            Vector3 offset = waypoints[currentWaypoint].right * offsetX;
            Vector3 finalTarget = baseTarget + offset;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(finalTarget, 0.5f);
            Gizmos.DrawLine(transform.position, finalTarget);
        }
    }
}
