using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBlocking : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    string BLOCK_ANIM = "IsBlocking";
    string PLAYER_TAG = "Player";
    string ENEMY_TAG = "Enemy";

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        box2d= GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*// block enemy attacks
        //
        if (Input.GetKey(KeyCode.U)){
            box2d.enabled= true;
            animator.SetBool(BLOCK_ANIM, true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PLAYER_TAG), 
                LayerMask.NameToLayer(ENEMY_TAG), true);
        }
        else
        {
            box2d.enabled= false;
            animator.SetBool(BLOCK_ANIM, false);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PLAYER_TAG),
                    LayerMask.NameToLayer(ENEMY_TAG), false);
        }*/
    }
}
