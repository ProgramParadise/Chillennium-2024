using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Here's a neighbor struct that holds firing sequence information
[System.Serializable]
public struct FiringInfo
{
    public int bullet_type; //which bullet type is fired? (see EnemyProjectileManager)
    public int num_cycles;  //how many cycles of this FSM before exiting?
    public int bullets_per_cycle;   //how many bullets before reloading?
    public float shot_warmup;       //how long to warm up before shooting?
    public float shot_cooldown;     //how long between firing bullets?
    public float reload_cooldown;   //how long to reload after finishing a cycle?
}

//This is the default firing sequence for an enemy
//I may derive other firing sequences from this class, for now I'm just changing up parameters.
public class FiringSequence : MonoBehaviour
{
    public enum FiringState
    {
        WarmingUp,
        Shooting,
        CoolingDown,
        Reloading,
        Complete
    };

    EnemyManager controller;
    Animator _animator;

    //stylistic choices determined by behavior
    public FiringInfo fire_info;
    public FiringState firing_state = FiringState.WarmingUp;

    float shot_warmup_elapsed = 0;
    float shot_cooldown_elapsed = 0;
    float reload_cooldown_elapsed = 0;
    float curr_cycle_shot_count = 0;    //current shots for a given cycle (magazine)
    float curr_cycle_count = 0;         //current cycles of shots complete
    
    //internal timers

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        controller = GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firing_state == FiringState.WarmingUp)
        {
            _animator.SetBool("Charging", true);
            controller.blackboard.busy = true;
            if (shot_warmup_elapsed > fire_info.shot_warmup)
            {
                shot_warmup_elapsed = 0;
                firing_state = FiringState.Shooting;
                _animator.SetBool("Attack", true);
                _animator.SetBool("Charging", false);
            }
            shot_warmup_elapsed += Time.deltaTime;
        }
        else if (firing_state == FiringState.Shooting)
        {
            controller.blackboard.bullet_queue.Enqueue(fire_info.bullet_type);
            firing_state = FiringState.CoolingDown;
            curr_cycle_shot_count++;
        }
        else if (firing_state == FiringState.CoolingDown)
        {
            if (shot_cooldown_elapsed > fire_info.shot_cooldown)
            {
                shot_cooldown_elapsed = 0;
                if (curr_cycle_shot_count >= fire_info.bullets_per_cycle)
                {
                    curr_cycle_count++;
                    controller.blackboard.busy = false;
                    curr_cycle_shot_count = 0;
                    firing_state = FiringState.Reloading;
                    _animator.SetBool("Attack", false);
                }
                else
                    firing_state = FiringState.Shooting;
            }
            shot_cooldown_elapsed += Time.deltaTime;
        }
        else if (firing_state == FiringState.Reloading)
        {
            if (curr_cycle_count >= fire_info.num_cycles)
            {
                firing_state = FiringState.Complete;
                curr_cycle_count = 0;
            }
            if (reload_cooldown_elapsed > fire_info.reload_cooldown)
            {
                reload_cooldown_elapsed = 0;
                firing_state = FiringState.WarmingUp;
            }
            reload_cooldown_elapsed += Time.deltaTime;
        }
    }
    private void OnEnable()
    {
        firing_state = FiringState.WarmingUp;
    }
}
