using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using d = UnityEngine.Debug;

public class Player : MonoBehaviour
{
    public Animator animator;
    public Level_1_Cutscene cutscene;
    Rigidbody2D rb;
    CapsuleCollider2D cap2d;
    CircleCollider2D circle2d;
    CharacterController2D characterController; // Contains the movement physics
    bool cutscenePlaying = true;


    [SerializeField] private float moveForce = 4f;
    private float horizontal = 0f;
    private bool jump = false;
    public bool canMovement = true;
    
    /*
    [Header("Detect Enemy")]
    public Transform DetectTransform;
    public float DetectRadius = 3.2f;
    public LayerMask enemyLayer;
    */
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cap2d = GetComponent<CapsuleCollider2D>();
        circle2d = GetComponent<CircleCollider2D>();
        characterController = GetComponent<CharacterController2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // if cutscene has finished, (it destroyed itself after playing), or doesn't exist
        // then set this boolean to false which will allow player to move.
        // If there is a cutscene, this boolean will be true and disable movement
        // so that player does not disrupt the cutscene
        // .
        if (cutscene == null)
        {
            cutscenePlaying = false;
        }

        // Disables movement if the cutscene is playing
        //
        if (canMovement && !cutscenePlaying) 
        {
            horizontal = Input.GetAxisRaw("Horizontal") * moveForce;

            // "Jump" is set to space. If "Jump" is pressed, the jump boolean will be set to true
            // jump boolean will be passed on to the Move() method in the character controller under
            // FixedUpdate()
            // animator.SetBool("IsJumping", true) will allow jump animation to play once player
            // presses space
            //
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }
        }
        // Sets it to 0 in case of player holding down the movement button
        // while blocking. Without this, player will be sliding all the way
        // to the direction player last pressed.
        // . 
        else horizontal = 0f; 

        // Sets 'Speed' parameter in the animator to the input received in 'horizontal'
        // Mathf.Abs Makes the value in the bracket always positive, as animation will only
        // activate if the speed is more than 0, so if player is walking left, it wouldn't
        // trigger unless Mathf.Abs is used.
        // This will allow the animation walk to play when player is walking
        //
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
    }

    private void FixedUpdate()
    {
        // Movement method from character controller 2d script.
        // Horizontal will change depending on the user's input.
        // E.g. left arrow or 'A' is -1 while right arrow or 'D' is 1.
        //
        characterController.Move(horizontal * Time.deltaTime, jump);
        jump = false;

    }

    // This is called in the UnityEvent under CharacterController2D.
    // .
    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }




    // Might not use
    /*void DetectEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(DetectTransform.position, DetectRadius, enemyLayer);

        if (enemies != null)
        {
            foreach (Collider2D enemy in enemies)
            {
                enemy.GetComponent<Enemy>().enemyAttack();
            }
        }
    }*/

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(DetectTransform.position, DetectRadius);
    }*/
}
