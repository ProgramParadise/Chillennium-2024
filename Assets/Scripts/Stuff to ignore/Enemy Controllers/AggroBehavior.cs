using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static FiringSequence;

enum AttackState
{
    Shooting,
    Hiding,
    Juking
};
public enum Aggro
{
    Passive,
    Agressive,
    Hybrid,
    Static
};

//This class is a few different FSMS wrapped into one.
//Depending on the nature of the enemy, it could have a different aggro behavior
//This class decides that behavior, and executes an FSM based on that behavior
//It probably could have been a base class with derived special behaviors, but this is fine for now
//R.I.P. the behavior tree dream, i'm simply not smart enough
public class AggroBehavior : MonoBehaviour
{
    Animator _animator;
    Rigidbody2D rb;
    AttackState curr_state = AttackState.Shooting;
    PlayerDetection player_detection;
    EnemyManager parent;
    FiringSequence firing_ref;

    float juke_time;    //duration of moving around
    float hide_time;    //duration of trying to hide
    float time_elapsed = 0; //timer used for any state

    //Juking params
    float juke_swap_time;
    float juke_swap_time_elapsed;

    //chasing params
    bool chase_player = false;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        firing_ref = (FiringSequence)gameObject.AddComponent(typeof(FiringSequence));
        firing_ref.enabled = false;
        player_detection = transform.GetComponentInChildren<PlayerDetection>();
        parent = transform.GetComponentInChildren<EnemyManager>();
        rb = GetComponent<Rigidbody2D>();
        juke_time = 1.5f;
        hide_time = 3;
        juke_swap_time = .5f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (parent.behavior) {
            case Aggro.Hybrid:
                DefaultFSM();
                break;
            case Aggro.Agressive:
                AggressiveFSM();
                break;
            default:
                DefaultFSM();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (parent.behavior)
        {
            case Aggro.Hybrid:
                FixedDefaultFSM();
                break;
            case Aggro.Agressive:
                FixedAggressiveFSM();
                break;
            default:
               FixedDefaultFSM();
                break;
        }
    }

    void DefaultFSM()
    {
        _animator.SetBool("Running", false);
        if (curr_state == AttackState.Juking)
        {
            _animator.SetBool("Running", true);
            if (time_elapsed > juke_time)
            {

                curr_state = AttackState.Shooting;
                firing_ref.firing_state = FiringState.WarmingUp;
                firing_ref.enabled = true;
                time_elapsed = 0;
            }
        }
        if (curr_state == AttackState.Shooting)
        {
            //SHOOTING HERE
            if (firing_ref.firing_state == FiringState.Complete)
            {
                firing_ref.enabled = false;
                curr_state = AttackState.Juking;
                time_elapsed = 0;
            }
        }
        time_elapsed += Time.deltaTime;
    }

    void FixedDefaultFSM()
    {
        if (curr_state == AttackState.Juking)
        {
            //TODO: add randomness here
            if (juke_swap_time_elapsed < .25)
            {
                rb.AddForce(parent.idle_speed / Random.Range(1.0f,2.0f) * (Vector3.Cross(new Vector3(0, 0, 1), player_detection.playerTransform.position - transform.position)).normalized, ForceMode2D.Force);
            }
            else
            {
                rb.AddForce(-parent.idle_speed / Random.Range(1.0f, 2.0f) * (Vector3.Cross(new Vector3(0, 0, 1), player_detection.playerTransform.position - transform.position)).normalized, ForceMode2D.Force);
            }

            if (juke_swap_time_elapsed > juke_swap_time)
                juke_swap_time_elapsed = 0;

            juke_swap_time_elapsed += Time.fixedDeltaTime;
        }
    }

    void AggressiveFSM()
    {
        _animator.SetBool("Running", false);
        if (curr_state == AttackState.Juking)
        {
            //TODO: add randomness here
            if (Vector3.Distance(player_detection.playerTransform.position,transform.position) < 2.5f)
            {
                chase_player = false;
                //skip to next state
                time_elapsed = juke_time + .1f;
            }
            else
            {
                _animator.SetBool("Running", true);
                chase_player = true;
            }

            if (time_elapsed > juke_time)
            {
                curr_state = AttackState.Shooting;
                firing_ref.firing_state = FiringState.WarmingUp;
                firing_ref.enabled = true;
                time_elapsed = 0;
            }
        }
        if (curr_state == AttackState.Shooting)
        {
            //SHOOTING HERE
            if (firing_ref.firing_state == FiringState.Complete)
            {
                firing_ref.enabled = false;
                curr_state = AttackState.Juking;
            }
        }
    }

    void FixedAggressiveFSM()
    {
        if(chase_player)
            rb.AddForce(parent.idle_speed / 1.0f * (player_detection.playerTransform.position - transform.position).normalized, ForceMode2D.Force);
    }


    void InitFiring(int bullet_type, int num_cycles, int bullets_per_cycle, float shot_warmup, float shot_cooldown, float reload_cooldown)
    {
        firing_ref.fire_info.bullet_type = bullet_type;
        firing_ref.fire_info.num_cycles = num_cycles;
        firing_ref.fire_info.bullets_per_cycle = bullets_per_cycle;
        firing_ref.fire_info.shot_warmup = shot_warmup;
        firing_ref.fire_info.shot_cooldown = shot_cooldown;
        firing_ref.fire_info.reload_cooldown = reload_cooldown;
        firing_ref.enabled = false;
    }

    void SetAggroParams()
    {
        juke_time = 1.0f;
        hide_time = 0f;
        juke_swap_time = .5f;
    }


    private void OnEnable()
    {

        time_elapsed = 0.0f;
        //add firing sequence values here based on behavior
        FiringInfo fi = parent.fire_info;
        InitFiring(fi.bullet_type,fi.num_cycles,fi.bullets_per_cycle,fi.shot_warmup,fi.shot_cooldown,fi.reload_cooldown);
        curr_state = AttackState.Shooting;
        firing_ref.enabled = true;
    }

    private void OnDisable()
    {
        GetComponent<FiringSequence>().enabled = false;
    }
}
