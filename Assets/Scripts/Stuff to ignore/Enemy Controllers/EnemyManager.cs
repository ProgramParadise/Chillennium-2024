using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManager : MonoBehaviour
{
    EnemyProjectileManager controller;
    EnemyDetection wall_checker;
    PlayerDetection player_detection;
    Rigidbody2D rb;
    delegate bool get_player_detection();
    get_player_detection asdfasd;

    //MODE vars
    [SerializeField]
    int mode; //Modes define current AI behaviors. Mode1 is idling, Mode2 is player chasing, Mode3 is attacking player
    bool mode_coroutine = true;
    bool busy_action = true;    

    //IDLE vars
    float move_duration;
    float move_time_elapsed;
    public float idle_speed; //scale of movement speed
    bool idle_ready;
    Vector3 idle_move_dir;

    //AGGRO VARS
    public Aggro behavior = Aggro.Hybrid;
    bool enable_attack = false;

    //This holds all the global state for the hierarchical FSM
    public struct Blackboard
    {
        //Note: queue values are the ID of the bullet to be fired.
        public Queue<int> bullet_queue; //Queue for our bullets, in case we want to fire multiple of them at the same time!
        public bool busy;  //global bool to block state transitions

    }
    public Blackboard blackboard;

    public FiringInfo fire_info; //holds firing sequence data

    //adding this here since other classes depend on it in their start functions
    private void Awake()
    {
        blackboard.bullet_queue = new Queue<int>();
    }
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<EnemyProjectileManager>();
        gameObject.AddComponent(typeof(AggroBehavior));
        rb = GetComponent<Rigidbody2D>();
        player_detection = transform.GetComponentInChildren<PlayerDetection>();
        asdfasd = player_detection.PlayerIsDetected;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode_coroutine)
        {
            StartCoroutine(CheckMode());
            mode_coroutine = false;
        }

        //potentially move these calls to fixedupdate, not sure though.
        if (mode == 1)
        {
            GetComponent<AggroBehavior>().enabled = false;
            enable_attack = false;
            Idle();
        }
        else if (mode == 2)
        {
            GetComponent<AggroBehavior>().enabled = false;
            enable_attack = false;
            Chase();
        }
        else if (mode == 3)
        {
            if (!enable_attack)
            {
                GetComponent<AggroBehavior>().enabled = true;
                enable_attack = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (mode == 1)
        {
            rb.AddForce(idle_move_dir, ForceMode2D.Force);
        }
        if (mode == 2)
        {
            rb.AddForce(idle_speed / 1.5f * (player_detection.playerTransform.position - transform.position).normalized, ForceMode2D.Force);
        }
    }

    //Idling
    void Idle()
    {
        move_time_elapsed += Time.deltaTime;

        //checking if we're good to poll new v alues
        if (idle_ready)
        {
            idle_ready = false;
            PollIdle();
        }

        //if time elapsed > duration, set idle_ready to true and reset time elapsed
        if (move_time_elapsed > move_duration)
        {
            idle_ready = true;
            move_time_elapsed = 0;
        }
    }

    void Chase()
    {
        //naive chase implementation here (also this is where navmesh pathfinding would goooooooo)
    }

    //calculating idle vars
    //these could be set in editor but I'm just hard coding values for now to reduce clutter on editor
    void PollIdle()
    {
        move_duration = Random.Range(.2f, 1.0f);
        float idle_move_delta = idle_speed * Random.Range(.2f, 1.0f); //how much to move per (physics) frame
                                                                        //most of the time, our enemies shouldn't be moving
        if (Random.Range(0, 1.0f) < .2f)
        {
            idle_move_dir = Vector3.zero;
            return;
        }

        idle_move_dir.x = Random.Range(-1.0f, 1.0f);
        idle_move_dir.y = Random.Range(-1.0f, 1.0f);
        idle_move_dir.Normalize();
        idle_move_dir = idle_move_dir * idle_move_delta;


    }

    //check player detection mode
    IEnumerator CheckMode()
    {
        if (!blackboard.busy)
        {
            if (player_detection.playerAggroed)
                mode = 3;
            else if (asdfasd())
                mode = 2;
            else
                mode = 1;
        }   
        yield return new WaitForSeconds(.25f);
        mode_coroutine = true;
    }
}
