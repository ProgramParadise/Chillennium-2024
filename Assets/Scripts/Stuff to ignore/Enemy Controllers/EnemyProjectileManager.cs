using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;



//This class should handle projectiles that fire from the enemy. Sorta like the enemy gun, it is its own component.
public class EnemyProjectileManager : MonoBehaviour
{

    //Here we have every type of bullet, keyed by their ID value
    public static Dictionary<int, BulletInformation> bullet_dictionary;

    [Header("Enemy Weapon Field")]
    public GameObject eBul;         //bullet prefab
    public GameObject muzzleFlash;  //muzzle flash effect
    public Transform eBulSpawn;     //bullet spawn location
    private int moveDir;

    EnemyManager controller;        //link to enemy manager
    PlayerDetection player_detection; //not sure why im using this for getting the transform
    Queue<int> bullet_queue;        //bullet queue in case multiple shots fired at same time        

    private void Start()
    {
        //setting up our fire queue.
        controller = GetComponent<EnemyManager>();
        bullet_queue = controller.blackboard.bullet_queue;

        player_detection = GetComponentInChildren<PlayerDetection>();
        moveDir = Random.Range(1, 5); // 1-4  -- 1=down 2=left 3=right 4=up
        if (moveDir < 1 || moveDir > 4)
        {
            while (moveDir < 1 || moveDir > 4) { moveDir = Random.Range(1, 5); }
        }
    }

    private void Update()
    {
        while(bullet_queue.Count > 0)
        {
            BulletInformation curr_bullet = bullet_dictionary[bullet_queue.Dequeue()];
            ShootStandardBullet(curr_bullet);   
        }
    }

    private void FixedUpdate()
    { 
    }


    //For now I am just makign functions that shoot bullets directly at the player
    //In the future, different enemies may have different shooting configurations (lead the player, lag behind the player, add random noise to shots
    //Also, different bullets should have their own system 
    void ShootStandardBullet(BulletInformation curr_bullet)
    {
        Vector3 shootDirection = (player_detection.playerTransform.position - transform.position).normalized;
        Quaternion shootRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg));
        GameObject eBull = Instantiate(eBul, eBulSpawn.position, shootRotation);

        //For now, we only track bullet width, since length is the default scale (sorta like aspect ratio)
        float size_x = eBull.transform.localScale.x; 
        float size_y = curr_bullet.size_modifier * eBull.transform.localScale.y;
        eBull.transform.localScale = new Vector3(size_x, size_y, 1);
        EnemyBullet bullet_script = eBull.GetComponent<EnemyBullet>();

        bullet_script.bullet_information = curr_bullet.Clone();
        bullet_script.shoot_direction = shootDirection.normalized;
        bullet_script.orientation = shootRotation;
        if(muzzleFlash)
            Instantiate(muzzleFlash, eBulSpawn.position, shootRotation);
    }

    //this is where I'm creating all the bullet models to be used ( called by game manager). If I had more time I'd save as json file or something IDFK
    public static void LoadDictionary()
    {
        if(bullet_dictionary == null)
            bullet_dictionary = new Dictionary<int, BulletInformation>();

        // 000
        //default bullet
        BulletInformation bull_info = new BulletInformation();
        bull_info.AddToDict();

        // 001
        //wavy bullet
        bull_info = new BulletInformation(shot_pattern: ShotPattern.Wavy);
        bull_info.frequency = 3;
        bull_info.amplitude = .2f;
        bull_info.vfx_name = "Void_VFX";
        bull_info.AddToDict();

        // 002
        //large bullet
        bull_info = new BulletInformation();
        bull_info.size_modifier = 5;
        bull_info.vfx_name = "Pineapple_VFX";
        bull_info.AddToDict();


        // 003
        //Ricochet
        bull_info = new BulletInformation();
        bull_info.size_modifier = 3;
        bull_info.on_hit_effect = OnCollisionEffect.Ricochet;
        bull_info.max_collisions = 10;
        bull_info.particle_lifetime = 60;
        bull_info.AddToDict();

        // 004
        // Boss1 shotgun projectile
        bull_info = new BulletInformation();
        float dist = 10;
        bull_info.base_speed = 7f;
        bull_info.base_damage = .1f;
        bull_info.particle_lifetime = dist / bull_info.base_speed;
        bull_info.vfx_name = "Rock_VFX";
        bull_info.AddToDict();

        // 005
        // Boss1 ring projectile
        bull_info = new BulletInformation();
        dist = 25;
        bull_info.base_speed = 6f;
        bull_info.base_damage = .12f;
        bull_info.particle_lifetime = dist / bull_info.base_speed;
        bull_info.vfx_name = "Rock_VFX";
        bull_info.AddToDict();

    }
}
