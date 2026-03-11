using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_1_Cutscene : MonoBehaviour
{
    public bool cutscenePlaying = true;

    GameObject levelText; // The beginning level text
    Animator levelTextAnimator;

    GameObject Cinematic; // The 2 black bars 
    Animator cinematic_top;
    Animator cinematic_bottom;

    [Header("Movement")]
    public CharacterController2D charController;
    public float moveForce = 100f; // Movement force of player
    public float moveTime = 1f; // Movement time of player for the start cutscene

    string CINEMATIC_START_ANIM = "move";
    string CINEMATIC_END_ANIM = "disappear";
    // Start is called before the first frame update
    void Start()
    {
        // Get the child game object of Level_1_Cutscene, which is the text
        // .
        levelText = transform.Find("Start Level")
            .transform.Find("Start Level Text").gameObject;  

        levelTextAnimator = levelText.GetComponent<Animator>();

        Cinematic = transform.Find("Cinematic View").gameObject;
        cinematic_top = Cinematic.transform.Find("Top").GetComponent<Animator>();
        cinematic_bottom = Cinematic.transform.Find("Bottom").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check whether the level text animation has finished.
        // If level text faded out, it will trigger the cinematic animation
        // .
        if (levelTextAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            cinematic_top.SetTrigger(CINEMATIC_START_ANIM);
            cinematic_bottom.SetTrigger(CINEMATIC_START_ANIM);

            // Starts moving player only after the cinematic bars are in view
            // .
            StartCoroutine(PlayerMoveCutscene());

        }
    }

    bool move = false;
    IEnumerator PlayerMoveCutscene()
    {
        if (!move)
        {
            // Player will be forced to move right for a certain amount of time
            // for a cutscene
            // .
            yield return new WaitForSeconds(moveTime); // Waits for level text to disappear, and display level
            charController.Move(1f * moveForce * Time.deltaTime, false);    // Moves the player...
            move = true;
            yield return new WaitForSeconds(0.7f);                      // ...for this amount of time

            charController.Move(0f, false);     // Stops the player...
            yield return new WaitForSeconds(1f);//... and let the player take in whats happening

            cinematic_top.SetTrigger(CINEMATIC_END_ANIM);
            cinematic_bottom.SetTrigger(CINEMATIC_END_ANIM);
            yield return new WaitForSeconds(1f); // Wait for the animation to finish...
            cutscenePlaying = false;            // ... and disable cutscene, to allow movement

            Destroy(gameObject);
        }
    }
}
