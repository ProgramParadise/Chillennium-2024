using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

//for future use
public enum DebuffType
{
    Slow_Player,
    Damage_Over_Time,
    Reduce_healing,
    None
};

//these are mutually exclusive shot patterns (for now)
public enum ShotPattern
{
    Wavy,
    Longitudinal, //longitudinal wave (look it up)
    None
};

//TODO: add other things like shot acceleration, deceleration, all that jazz

//These are mutually exclusive bullet behaviors to occur on hit
public enum OnCollisionEffect
{
    Ricochet,
    Split,
    Boomerang,
    None
};

//This class stores bullet information. It's sorta like a "bullet type".
public class BulletInformation
{
    //generic info
    static int bullet_ID = 0;           //used for bullet dictionary indexing
    public float particle_lifetime;     //how long bullet will exist for (in seconds)
    public string vfx_name;             //used for loading VFX (couldve been scriptable object but whateva)
    public int size_modifier;           //bullet scale
    public float base_speed;              //bullet base speed (overrides editor)
    public float base_damage;
    public DebuffType debuff_type;      //debuff attached to bullet, if any

    //Shot pattern info
    public float amplitude;             //for wavy patterns
    public float frequency;             //for wavy patterns
    public ShotPattern shot_pattern;    //shot pattern (in future may include spread, so shots make multiple bullets

    //OnCollisionEffect Info
    public OnCollisionEffect on_hit_effect;   //on hit effect
    public float OCE_cooldown;                //on hit effect cooldown
    public float max_collisions;              //max collisions
    public float curr_collisions;         //current # of collisions

    //just one constructor cuz im lazy
    public BulletInformation(float base_speed = -1, float particle_lifetime = 5, string bullet_name = " ", int size_modifier = 1, OnCollisionEffect on_hit_effect = OnCollisionEffect.None, 
    ShotPattern shot_pattern = ShotPattern.None, DebuffType debuff_type = DebuffType.None, float base_damage = .05f)
    {
        this.base_speed = base_speed;
        this.particle_lifetime = particle_lifetime;
        this.vfx_name = bullet_name;
        this.size_modifier = size_modifier;
        this.on_hit_effect = on_hit_effect;
        this.shot_pattern = shot_pattern;
        this.debuff_type = debuff_type;
        this.base_damage = base_damage;
        amplitude = 1;
        frequency = 1;

        OCE_cooldown = 0;
        max_collisions = 1;
    }

    //store bullet in dict
    public void AddToDict()
    {
        
        EnemyProjectileManager.bullet_dictionary.Add(bullet_ID, this);
        bullet_ID++;
    }

    public BulletInformation Clone()
    {
        return (BulletInformation) this.MemberwiseClone();
    }

}
