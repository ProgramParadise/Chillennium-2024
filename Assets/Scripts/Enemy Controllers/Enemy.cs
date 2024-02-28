using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    /*
    public bool animateObject = true;
    public bool enjoy = true;

    [Range(0f, 10f)]
    public float moveSpeed = 4f; // Enemy move speed when moving
    public int damageAmount = 10; // Probably deal a lot of damage to kill player immediately

    [Tooltip("Child gameObject for detecting stun.")]
    public GameObject stunnedCheck; // What gameobject is the stunnedCheck

    public float stunnedTime = 3f; // How long to wait at a waypoint

    public string stunnedLayer = "StunnedEnemy"; // Name of the layer to put enemy on when stunned
    public string playerLayer = "Player"; // aNme of the player layer to ignore collisions with when stunned

    [HideInInspector]
    public bool isStunned = false; // Flag for isStunned

    public GameObject[] myWaypoints; // To define the movement waypoints

    [Tooltip("How much time in seconds to wait at each waypoint")]
    public float waitAtWaypointTime = 1f; // How long to wait at a waypoint

    public bool loopWaypoints = true; // Should it loop through the waypoints

    // SFXs
    public AudioClip stunnedSFX;
    public AudioClip attackSFX;

    // Private variables below

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

    // Store the layer number the enemy is on (setup in Awake)
    int _enemyLayer;

    // Store the layer number the enemy should be moved to when stunned
    int _stunnedLayer;

    void Awake()
    {
        // Get a reference to the components we are going to be changing and store a reference for efficiency purposes
        _transform = GetComponent<Transform>();

        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null) // If Rigidbody is missing
            Debug.LogError("Rigidbody2D component missing from this gameobject");

        _animator = GetComponent<Animator>();
        if (_animator == null) // If Animator is missing
            //Debug.LogError("Animator component missing from this gameobject");

            _audio = GetComponent<AudioSource>();
        if (_audio == null)
        { // If AudioSource is missing
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // Let's just add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }

        if (stunnedCheck == null)
        {
            Debug.LogError("stunnedCheck child gameobject needs to be setup on the enemy");
        }

        // Setup moving defaults
        _moveTime = 0f;
        _moving = true;

        // Determine the enemies specified layer
        _enemyLayer = this.gameObject.layer;

        // Determine the stunned enemy layer number
        _stunnedLayer = LayerMask.NameToLayer(stunnedLayer);

        // Make sure collision are off between the playerLayer and the stunnedLayer
        // Which is where the enemy is placed while stunned
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), _stunnedLayer, true);
    }

    // If not stunned then move the enemy when time is > _moveTime
    void Update()
    {
        if (!isStunned)
        {
            if (Time.time >= _moveTime)
            {
                EnemyManager();
            }
            else
            {
                _animator.SetBool("Moving", false);
            }
        }
    }

    // Move the enemy through its rigidbody based on its waypoints
    void EnemyManager()
    {
        // If there isn't anything in My_Waypoints
        if ((myWaypoints.Length != 0) && (_moving))
        {
            // Make sure the enemy is facing the waypoint (based on previous movement)
            Flip(_vx);

            // Determine distance between waypoint and enemy
            _vx = myWaypoints[_myWaypointIndex].transform.position.x - _transform.position.x;

            // If the enemy is close enough to waypoint, make it's new target the next waypoint
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
                // Enemy is moving
                _animator.SetBool("Moving", true);

                // Set the enemy's velocity to moveSpeed in the x direction.
                _rigidbody.velocity = new Vector2(
                    _transform.localScale.x * moveSpeed,
                    _rigidbody.velocity.y
                );
            }
        }
    }

    // Flip the enemy to face torward the direction he is moving in
    void Flip(float _vx)
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

    // Attack player
    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") && !isStunned)
        {
            CharacterController2D player =
                collision.gameObject.GetComponent<CharacterController2D>();
            if (player.playerCanMove)
            {
                // Make sure the enemy is facing the player on attack
                if (animateObject)
                    Flip(collision.transform.position.x - _transform.position.x);

                // Attack sound
                playSound(attackSFX);

                // Stop moving
                _rigidbody.velocity = new Vector2(0, 0);

                // Apply damage to the player
                player.ApplyDamage(damageAmount, gameObject.name);

                if (enjoy)
                {
                    // Stop to enjoy killing the player
                    _moveTime = Time.time + stunnedTime;
                }
            }
        }
    }

    // If the Enemy collides with a MovingPlatform, then make it a child of that platform
    // So it will go for a ride on the MovingPlatform
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            this.transform.parent = other.transform;
        }
    }

    // If the enemy exits a collision with a moving platform, then unchild it
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

    // Setup the enemy to be stunned
    public void Stunned()
    {
        if (!isStunned)
        {
            isStunned = true;

            // Provide the player with feedback that enemy is stunned
            playSound(stunnedSFX);
            _animator.SetTrigger("Stunned");

            // Stop moving
            _rigidbody.velocity = new Vector2(0, 0);

            // Switch layer to stunned layer so no collisions with the player while stunned
            this.gameObject.layer = _stunnedLayer;
            stunnedCheck.layer = _stunnedLayer;

            // Start coroutine to stand up eventually
            StartCoroutine(Stand());
        }
    }

    // Coroutine to unstun the enemy and stand back up
    IEnumerator Stand()
    {
        yield return new WaitForSeconds(stunnedTime);

        // No longer stunned
        isStunned = false;

        // Switch layer back to regular layer for regular collisions with the player
        this.gameObject.layer = _enemyLayer;
        stunnedCheck.layer = _enemyLayer;

        // Provide the player with feedback
        _animator.SetTrigger("Stand");
    }
    */
}
