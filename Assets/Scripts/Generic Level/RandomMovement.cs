using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public bool facePlayerWhenCollide = true;
    public bool idle = false;
    public bool randomFlip = true;
    public bool playSFX = false;

    [Range(0f, 10f)]
    public float moveSpeed = 4f; // Move speed when moving

    public GameObject[] myWaypoints; // To define the movement waypoints

    [Tooltip("How much time in seconds to wait at each waypoint")]
    public float waitAtWaypointTime = 1f; // How long to wait at a waypoint

    public bool loopWaypoints = true; // Should it loop through the waypoints

    // SFXs
    public AudioClip[] randomSpeechSFX;
    public AudioClip[] moveOverSFX;

    // Store references to components on the gameObject
    Transform _transform;
    Rigidbody2D _rigidbody;
    Animator _animator;
    AudioSource _audio;

    // Movement tracking
    [SerializeField]
    int _myWaypointIndex = 0; // Used as index for My_Waypoints
    float _moveTime;
    float _vx = 0f;
    bool _moving = true;

    // Store the layer number the NPC is on (setup in Awake)
    int _NPCLayer;

    void Awake()
    {
        // Get a reference to the components we are going to be changing and store a reference for efficiency purposes
        _transform = GetComponent<Transform>();

        // If Transform is missing
        if (_transform == null)
        {
            Debug.LogError("Transform component missing from this gameobject. Adding one.");
            // Add the Transform component dynamically
            _transform = gameObject.AddComponent<Transform>();
        }

        _rigidbody = GetComponent<Rigidbody2D>();

        // If Rigidbody is missing
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody2D component missing from this gameobject. Adding one.");
            // Add the RigidBody2D component dynamically
            _rigidbody = gameObject.AddComponent<Rigidbody2D>();
        }

        _animator = GetComponent<Animator>();
        // If Animator is missing
        if (_animator == null)
        {
            Debug.LogError("Animator component missing from this gameobject. Adding one.");
            // Add the Animator component dynamically
            _animator = gameObject.AddComponent<Animator>();
        }

        _audio = GetComponent<AudioSource>();
        // if AudioSource is missing
        if (_audio == null && playSFX)
        {
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // Add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }

        // Setup moving defaults
        _moveTime = 0f;
        _moving = true;

        // Determine the NPC specified layer
        _NPCLayer = this.gameObject.layer;
    }

    // Move the NPC randomly
    void Update()
    {
        if (Time.frameCount % Mathf.Round(Random.Range(2000, 3000)) == 0 && Time.frameCount > 0)
        {
            NPCMovement();

            if (playSFX)
            {
                playSound(randomSpeechSFX[Random.Range(0, randomSpeechSFX.Length)]);
            }
        }
    }

    // Move the NPC through its rigidbody based on its waypoints
    void NPCMovement()
    {
        // If there isn't anything in My_Waypoints
        if (myWaypoints.Length != 0 && _moving && !idle)
        {
            // Make sure the NPC is facing the waypoint (based on previous movement)
            FlipDirection(_vx);

            // Determine distance between waypoint and NPC
            _vx = myWaypoints[_myWaypointIndex].transform.position.x - _transform.position.x;

            // If the NPC is close enough to waypoint, make it's new target the next NPC
            if (Mathf.Abs(_vx) <= 0.05f)
            {
                // At waypoint so stop moving
                _rigidbody.velocity = new Vector2(0, 0);

                // Increment to next index in array
                _myWaypointIndex++;

                // Reset waypoint back to 0 for looping
                if (_myWaypointIndex >= myWaypoints.Length)
                {
                    if (loopWaypoints)
                        _myWaypointIndex = 0;
                    else
                        _moving = false;
                }

                // Setup wait time at current waypoint
                _moveTime = Time.time + waitAtWaypointTime;
            }
            else
            {
                // NPC is moving
                _animator.SetBool("Moving", true);

                // Set the NPC's velocity to moveSpeed in the x direction.
                _rigidbody.velocity = new Vector2(
                    _transform.localScale.x * moveSpeed,
                    _rigidbody.velocity.y
                );
            }
        }
        else if (randomFlip)
        {
            Flip();
        }
    }

    // Flip the NPC to face torward the direction he is moving in
    void FlipDirection(float _vx)
    {
        // Get the current scale
        Vector3 localScale = _transform.localScale;

        if ((_vx > 0f) && (localScale.x < 0f))
            localScale.x *= -1;
        else if ((_vx < 0f) && (localScale.x > 0f))
            localScale.x *= -1;

        // Update the scale
        _transform.localScale = localScale;
    }

    // Flip the NPC to face the other direction
    void Flip()
    {
        // Get the current scale
        Vector3 localScale = _transform.localScale;

        localScale.x *= -1;

        // Update the scale
        _transform.localScale = localScale;
    }

    // Collide with player
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Movement player =
                collision.gameObject.GetComponent<Player_Movement>();
            if (player.playerCanMove)
            {
                // Make sure the NPC is facing the player
                if (facePlayerWhenCollide)
                    FlipDirection(collision.transform.position.x - _transform.position.x);

                // Play you are in my way sound
                if (playSFX)
                {
                    playSound(moveOverSFX[Random.Range(0, moveOverSFX.Length)]);
                }

                // Stop moving
                _rigidbody.velocity = new Vector2(0, 0);
            }
        }
    }

    /* If the NPC collides with a MovingPlatform, then make it a child of that platform
    so it will go for a ride on the MovingPlatform */
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            this.transform.parent = other.transform;
        }
    }

    // If the NPC exits a collision with a moving platform, then unchild it
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            this.transform.parent = null;
        }
    }

    // Play sound through the audiosource on the gameobject
    void playSound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }
}
