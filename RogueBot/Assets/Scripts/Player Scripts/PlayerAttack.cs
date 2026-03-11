using System;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    Rigidbody2D rb;
    BoxCollider2D box2d; // This is for the blocking mechanic
    SpriteRenderer sr;
    HealthScript healthScript;
    Player player;
    
    [SerializeField] GameObject punch1;

    private int playerMaxHealth = 3;
    public int playerCurrentHealth;

    [Header("Attack Point: Punch")]
    [SerializeField] float attackDamage1 = 10f;
    [SerializeField] float attackDamage2 = 20f;
    public static PlayerAttack instance;
    [HideInInspector] public bool isAttacking = false;

    [Header("Sound")]
    public AudioSource attackSound;
    public AudioSource hitSound1;
    public AudioSource hitSound2;

    // This will be used by the Punch1 and Punch2 Script.
    // This for the purpose of changing the damage on the playerAttack script
    // instead of going through the child game objects to get to the punch scripts
    // .
    public float AttackDamage1 
    {
        get { return attackDamage1; }
        set { attackDamage1 = value; }
    }
    public float AttackDamage2
    {
        get { return attackDamage2; }
        set { attackDamage2 = value; }
    }

    [Header("iFrames Attributes")] // After player gets hit, these variables will be used
    public int iFramesFlashes = 3;
    public float iFramesDuration = 2f;

    [Header("Knockback")]
    bool blockKnockback = false;
    public float blockKnockbackForce = 2000f;
    bool knockback = false;
    public float knockbackForce = 2000f;
    [SerializeField] Vector2 knockbackDirection;
    [SerializeField] float knockbackDirectionYAxis = 0.4f; // Sets the vertical Knockback value 
    public float knockbackTime = 1f;
    public string BLOCK_ANIM = "IsBlocking";

    string PLAYER_TAG = "Player";
    string ENEMY_TAG = "Enemy";

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        box2d = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        healthScript= GetComponent<HealthScript>();
        player = GetComponent<Player>();

        // Gets the child gameobject,
        //
        GameObject rightArmSolver = transform.GetChild(0).gameObject;

        // and then punch1 gets the child of rightArmSolver
        // This is for referencing the box trigger inside punch1 to register the 
        // collision with the enemy and the attack.
        // .
                            punch1 = rightArmSolver.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        // For combo. The other codes are in RightArmSolver_Target and LeftArmSolver_Target
        //
        {
            if (Input.GetKeyDown(KeyCode.K) && !isAttacking)
            {
                // transition1 Script will receive "true" from isAttacking and play the
                // first attack animation. Transition1 will set isAttacking to false after
                // the transition is done,
                // then send it to the next transition if "K" is pressed again.
                // 
                // Animation will enable the box trigger (for Solver_Targets) at a certain frame and
                // disable once it's done playing
                //
                isAttacking = true;
                attackSound.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        // Knockback is set to true when player gets damaged
        // .
        if (knockback)
        {
            // Disables the aircontrol to prevent player from moving after getting hit
            // while in the air. .
            GetComponent<CharacterController2D>().m_AirControl = false;
            rb.AddForce(knockbackDirection * knockbackForce * Time.deltaTime, ForceMode2D.Impulse);

            // This function will wait for player to land back on the ground after knock back
            // and re-enable the air control
            // .
            StartCoroutine(KnockbackTime());
            knockback = false; // Sets boolean to false to run this function once

        }

        // Set to true if player is colliding with enemy, but is blocking
        // .
        if (blockKnockback)
        {
            // Vertical Knockback is none as player is blocking, and gives a good visual difference
            // from the normal knockback
            // .
            knockbackDirection.y = 0f;
            rb.AddForce(knockbackDirection * blockKnockbackForce * Time.deltaTime, ForceMode2D.Impulse);
            blockKnockback = false;

            // Wait for enemy to pass through player, then after a time delay,
            // allow for layer collision again. This will allow player to
            // block constantly and not phase through enemies.
            // Without this, player can block once, and then just
            // be invulnerable due to not having layer collision.
            // .
            StartCoroutine(TimeDelayEnemyPlayerCollisionFalse(0.5f));
        }

        // block enemy attacks.
        //
        if (Input.GetKey(KeyCode.L))
        {
            // When blocking, player can't move. This is to prevent players from
            // abusing this to just run through enemies.
            // .
            player.canMovement = false;
            box2d.enabled = true;
            animator.SetBool(BLOCK_ANIM, true);
        }

        // Using this boolean due to a bug where if player isn't blocking,
        // this will always run and enable layer collision which will affect
        // any functions relating to that
        // .
        else if (!isPlayerHurtRunning)
        {
            player.canMovement  = true;
            box2d.enabled = false;
            animator.SetBool(BLOCK_ANIM, false);

            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PLAYER_TAG),
                LayerMask.NameToLayer(ENEMY_TAG), false);
            blockKnockback = false;
        }
    }
            
    private bool isPlayerHurtRunning = false; // Purpose is for running PlayerHurt() once;

    // Function is called under Enemy script whenever enemy collides with player
    //
    IEnumerator PlayerHurt(object[] object_)
    {
        isPlayerHurtRunning = true;

        // Player (Layer 3) will ignore any collision with Enemy (Layer 6)
        // This is for the invulneribilty frames after the player gets hit
        //
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PLAYER_TAG),
            LayerMask.NameToLayer(ENEMY_TAG), true);
        int enemyDamage = (int)object_[0];
        knockbackDirection = (Vector2)object_[1];
        knockbackDirection.y = knockbackDirectionYAxis;

        // If the player isn't blocking, he will take damage,
        // otherwise, set the boolean blockKnockback to true which will run
        // a knockback effect to the player. This is run under FixedUpdate()
        // .
        if (!animator.GetBool(BLOCK_ANIM))
        {
            // This will take away 1 heart of the player
            //
            healthScript.takeDamage(enemyDamage); // send in 1 damage point
            animator.SetTrigger("PlayerHurt");

            // This boolean will run the knockback function in fixedUpdate()
            //
            knockback = true;

            // For loop for the flickering effect after player gets hit
            // The wait for seconds formula will add up to the number set for 
            // the variable "duration", e.g. 3 seconds. Thus the for loop would finish
            // looping in 3 seconds
            //
            for (int i = 0; i < iFramesFlashes; i++)
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(iFramesDuration / (iFramesFlashes * 2));
                sr.color = Color.white;
                sr.color = new Color(1, 1, 1, 0.5f);
                yield return new WaitForSeconds(iFramesDuration / (iFramesFlashes * 2));

            }
            // i frames, flickering of sprite
            // set collision and sprite back to normal
            //
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PLAYER_TAG),
                LayerMask.NameToLayer(ENEMY_TAG), false);
            sr.color = new Color(1, 1, 1, 1f); // Sets player back to original color
        }
        else blockKnockback = true;

        isPlayerHurtRunning = false;
    }


    IEnumerator KnockbackTime()
    {
        yield return new WaitForSeconds(knockbackTime);
        GetComponent<CharacterController2D>().m_AirControl = true;
    }

    IEnumerator TimeDelayEnemyPlayerCollisionFalse(float time)
    {
        yield return new WaitForSeconds(time);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PLAYER_TAG),
                LayerMask.NameToLayer(ENEMY_TAG), false);
    }

    public void playerDie()
    {
        FindObjectOfType<GameManagerScript>().EndGame();
        GetComponent<CapsuleCollider2D>().enabled = false;
        this.enabled = false; // disables this script
    }

   
}
