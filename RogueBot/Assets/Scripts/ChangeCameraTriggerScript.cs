using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraTriggerScript : MonoBehaviour
{ // Level 2 trigger

    CameraFollow camera;
    //CameraFollow camera;

    string PLAYER_TAG = "Player";

    // Start is called before the first frame update
    void Start()
    {
        camera =  GameObject.Find("Main Camera").GetComponent<CameraFollow>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG))
        {
            // if position of player is to the right, set level 2 trigger to true.
            // This will affect the cameraFollow script which will move the camera
            // to the right where all the platforms and enemy are
            // .
            if (transform.position.x < collision.gameObject.transform.position.x)
            {
                camera.level2Trigger = true;
            }
            else
                camera.level2Trigger = false;
        }
    }
}
