using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyBullet : MonoBehaviour
{
    //bullet raycasting stuff
    LayerMask raycast_mask;
    RaycastHit2D hit;

    //TODO: add speed to bullet_info
    public float speed = 20;    
    float existence; //how long has the bullet existed
    public float damage = 0.15f;
    public GameObject wallDamageExplosion;
    public GameObject tableDamageExplosion;
    public GameObject damageExplosion;
    public AudioClip damageSFX;
    AudioSource _audio;
    SpriteRenderer _spriteRenderer;
    Rigidbody2D rb;

    public BulletInformation bullet_information;

    public Vector3 shoot_direction;     //bullet movement direction
    public Quaternion orientation;      //bullet rotation (orientation)

    void Awake()
    {
        raycast_mask = LayerMask.GetMask("Player") | LayerMask.GetMask("Wall");
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        { // if AudioSource is missing
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //spawn VFX
        if (bullet_information.vfx_name != " ")
            LoadVFX();
        
        
        rb = GetComponent<Rigidbody2D>();
        //bullet only lasts so long
        Destroy(gameObject, bullet_information.particle_lifetime);

        if(bullet_information.base_speed >= 0)
            speed = bullet_information.base_speed;
        damage = bullet_information.base_damage;


    }

    // Update is called once per frame
    void Update()
    {
        existence += Time.deltaTime;
        

        //TODO: check raycast for collision logic before applying movement
        //TODO: fix bullet movement to not rely on transform position
    }

    private void FixedUpdate()
    {
        //tick movement to find movement vector
        Vector3 move_delta = TickMovement();

        //check raycasts for collision detection
        hit = Physics2D.Linecast(transform.position, transform.position + move_delta, raycast_mask);
        if (hit)
            LinecastDetected(hit);
        else
            rb.MovePosition(transform.position + move_delta);
    }

    void LinecastDetected(RaycastHit2D hit)
    {
        RaycastHit2D hit_2;
        //in case player moves fast and we phase into player, back the projectile up in time until it was about to hit the player
        int i = 0;
        int count = 0;
        do
        {
            count++;
            i++;
            hit_2 = Physics2D.Linecast(transform.position - shoot_direction * speed * Time.fixedDeltaTime * i, transform.position - shoot_direction * speed * Time.fixedDeltaTime * (i-1), raycast_mask);
        } while (hit_2 && count < 1000);

        //if we are embedded in collider, move back in time
        if (i > 1)
        {
            hit = Physics2D.Linecast(transform.position - shoot_direction * speed * Time.fixedDeltaTime * (i - 1), transform.position - shoot_direction * speed * Time.fixedDeltaTime * (i - 2), raycast_mask);
            transform.position = transform.position - shoot_direction * speed * Time.fixedDeltaTime * i;
        }



        switch (bullet_information.on_hit_effect)
        {
            case OnCollisionEffect.None:
                DefaultCollision();
                break;
            case OnCollisionEffect.Split:
                //TODO
                break;
            case OnCollisionEffect.Ricochet:
                Ricochet(hit);
                break;
            case OnCollisionEffect.Boomerang:
                //TODO - would need to make this trigger at end of life as well, if so desired... reset lifespan and reverse speed? simple enough???
                break;
            default:
                DefaultCollision();
                break;
        }
    }

    //Returns the movement vector of the given host bullet
    Vector3 TickMovement()
    {
        Vector3 move_delta = Vector3.zero;
        switch (bullet_information.shot_pattern)
        {
            case ShotPattern.Wavy:
                move_delta = WavyPattern();
                break;
            case ShotPattern.Longitudinal:
                break;
            case ShotPattern.None:
                move_delta = DefaultPattern();
                break;
            default:
                move_delta = DefaultPattern();
                break;
        }
        return move_delta;
    }

    Vector3 DefaultPattern()
    {
        Vector3 move_delta = Vector3.zero;
        move_delta += shoot_direction * speed * Time.fixedDeltaTime;
        return move_delta;
    }

    Vector3 WavyPattern()
    {
        Vector3 move_delta = DefaultPattern(); //get the default pattern
        move_delta += Vector3.Cross(new Vector3(0, 0, 1), shoot_direction).normalized * bullet_information.amplitude * Mathf.Sin(existence * bullet_information.frequency * 2 * Mathf.PI);
        return move_delta;
    }

    void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    private void OnDestroy()
    {
        //to do: add destroy particle effects
        //something like... "Instantiate bullet explosion effect"   
    }

    // Very hacky solution for collision detection. Basically if the player would be moving faster than the bullet,
    // we speed the bullet up to compensate.
    // I could mess with negative numbers here at some point if bullets are phasing through players.
    void Ricochet(RaycastHit2D hit)
    {

        //if its player we just kill the object
        if(hit.collider.tag == "Player")
        {
            DefaultCollision();
            return;
        }
        //if we've reached max # of collisions, kill the object
        if (bullet_information.curr_collisions >= bullet_information.max_collisions)
        {
            DefaultCollision();
            return;
        }
        bullet_information.curr_collisions++;

        //static objects are simpler
        if (!hit.rigidbody || hit.rigidbody.velocity.magnitude < .01f)
        {
            shoot_direction = Vector3.Reflect(shoot_direction, hit.normal).normalized;
            rb.MovePosition(transform.position + TickMovement() * 1.05f);
            orientation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(shoot_direction.y, shoot_direction.x) * Mathf.Rad2Deg));
            transform.rotation = orientation;
            return;
        }


            //find the new shoot direction and scale it up for ensuing calculations

        Vector3 collider_velocity = hit.rigidbody.velocity;
      
        shoot_direction = Vector3.Reflect(shoot_direction, hit.normal).normalized;
        shoot_direction *= speed;
        //breaking vector up into its components
        Vector3 parallel_projection = Vector3.Project(shoot_direction, collider_velocity.normalized);
        Vector3 perpendicular_component = shoot_direction - parallel_projection;

        //scale parallel component up to match the collider's velocity
        Vector3 parallel_component = parallel_projection.normalized * Mathf.Max(parallel_projection.magnitude, collider_velocity.magnitude);

        //redefine our shoot vector to now be the scaled version
        shoot_direction = perpendicular_component + parallel_component;

        //scale it back down and make the new speed the magnitude
        speed = shoot_direction.magnitude * 1.05f;
        shoot_direction.Normalize();
        //tick our movement in the correct new direction so we dont get stuck in anything (not 100% accurate but should work)
        rb.MovePosition(transform.position + TickMovement() * 1.05f);

        //add rotation to our thingy so it reorients correctly
        orientation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(shoot_direction.y, shoot_direction.x) * Mathf.Rad2Deg));
        transform.rotation = orientation;

        //TODO:
        //Start a coroutine to dampen our speed back down to the original bullet velocity
        //if there's another collision, reset our speed and halt the coroutine, recalculating everything
    }

    void DefaultCollision()
    {
        //TODO: fix bullet destruction to play music but not also exist anymore...
        Destroy(gameObject);
        //Destroy(gameObject, damageSFX.length);
    }
    void LoadVFX()
    {
        GameObject vfx_gameobject = Instantiate(Resources.Load<GameObject>(bullet_information.vfx_name), transform.localPosition, new Quaternion(0, 0, 0, 0), transform);
        Vector3 new_scale = Vector3.zero;
        new_scale.x = 1.0f / transform.localScale.x;
        new_scale.y = 1.0f / transform.localScale.y;
        new_scale.z = 1.0f / transform.localScale.z;
        vfx_gameobject.transform.localScale = new_scale;
        _spriteRenderer.enabled = false;
    }

}
