using UnityEngine;
using System.Collections;

public class PlatformMover : MonoBehaviour
{
    public GameObject platform; // Reference to the platform to move

    public GameObject[] myWaypoints; // Array of all the waypoints

    [Range(0.0f, 10.0f)] // Create a slider in the editor and set limits on moveSpeed
    public float moveSpeed = 5f; // Enemy move speed
    public float waitAtWaypointTime = 1f; // How long to wait at a waypoint before _moving to next waypoint

    public bool loop = true; // Should it loop through the waypoints

    // Private variables

    Transform _transform;
    int _myWaypointIndex = 0; // Used as index for My_Waypoints
    float _moveTime;
    bool _moving = true;

    // Use this for initialization
    void Start()
    {
        _transform = platform.transform;
        _moveTime = 0f;
        _moving = true;
    }

    // Game loop
    void Update()
    {
        // If beyond _moveTime, then start moving
        if (Time.time >= _moveTime)
        {
            Movement();
        }
    }

    void Movement()
    {
        // If there isn't anything in My_Waypoints
        if ((myWaypoints.Length != 0) && (_moving))
        {
            // Move towards waypoint
            _transform.position = Vector3.MoveTowards(
                _transform.position,
                myWaypoints[_myWaypointIndex].transform.position,
                moveSpeed * Time.deltaTime
            );

            // If the enemy is close enough to waypoint, make it's new target the next waypoint
            if (
                Vector3.Distance(
                    myWaypoints[_myWaypointIndex].transform.position,
                    _transform.position
                ) <= 0
            )
            {
                _myWaypointIndex++;
                _moveTime = Time.time + waitAtWaypointTime;
            }

            // Reset waypoint back to 0 for looping, otherwise flag not moving for not looping
            if (_myWaypointIndex >= myWaypoints.Length)
            {
                if (loop)
                    _myWaypointIndex = 0;
                else
                    _moving = false;
            }
        }
    }
}
