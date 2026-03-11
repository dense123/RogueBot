using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [Header("Door")] // The objective
    public GameObject door;
    Animator doorAnimator;
    private string DOOROPEN_ANIM = "isDoorOpen";
    public float doorTime = 1f;

    public float cameraSpeed = 1f; // For camera pan effect Vector3.Lerp()
    public GameObject player;
    Vector3 offset = new Vector3(0,0,-1);

    public bool level2Trigger; // For level 2 camera control

    [Header("Music")]
    [SerializeField] AudioSource music;


    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = door.GetComponent<Animator>();
        level2Trigger = false;
    }

    bool doorOpen = false;
    // Update is called once per frame
    void Update()
    {
        // If the scene is level 1, then the camera will follow these positions
        // .
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            Vector3 clamped = new Vector3(
                    Mathf.Clamp(player.transform.position.x, -24, 5),
                    Mathf.Clamp(player.transform.position.y, -2, 4),
                    0f);

            // If the door isn't open yet
            // .
            if (!doorAnimator.GetBool(DOOROPEN_ANIM))
                transform.position = clamped + offset;

            // Else if the door animation is true, run the else function
            // since door hasn't fully opened yet.
            else
            {
                if (doorOpen)
                {
                    transform.position = clamped + offset;
                }
                else
                {
                    // This will focus the camera to where the door is
                    // This will let the player know where to go next
                    // .
                    Vector3 targetPosition = new Vector3(5, door.transform.position.y, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
                    
                    // Set a time delay before focusing back to player.
                    // The time set will determine how long to focus on the door.
                    // Thus the time should be as long as the door animation
                    // .
                    StartCoroutine(DoorOpenTime(doorTime)); 
                }
            }
        }

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            // Manually sets the starting position of camera based on the level if player
            // hasn't passed through a set trigger "Camera Trigger" which changes the 
            // level2Trigger boolean
            //
            // The former function will run if the player goes through
            // and exits the trigger and is on the right side of the trigger
            // .
            Vector3 targetPos;
            if (level2Trigger)
            {
                targetPos = new Vector3(-0.73f, player.transform.position.y, transform.position.z);
            }

            // The latter function will run if the player is on the left
            // .
            else
            {
                targetPos = new Vector3(-11f, 9, transform.position.z);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, cameraSpeed * Time.deltaTime);
        }
    }

    IEnumerator DoorOpenTime(float time)
    {
        // Sets door open to true as it's fully open, then run the
        // if statement that focuses back to player under Update().
        yield return new WaitForSeconds(time);
        doorOpen = true;
    }

    IEnumerator PanBackToPlayer()
    {
        yield return new WaitForSeconds(1f);
    }
}
