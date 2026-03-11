using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformScript : MonoBehaviour
{ // Attached to player

    GameObject platform;

    string GROUND_TAG = "Ground";


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (platform != null)
            {
                StartCoroutine(DropDown());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            // If the ground is a platform. This is so that
            // not every ground will allow player to drop down on,
            // only the ones that has PlatformEffector.
            // .
            if (collision.gameObject.GetComponent<PlatformEffector2D>() != null)
            {
                // Assigns the collider game object so that update() will run
                // the coroutine
                // .
                platform = collision.gameObject;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            if (collision.gameObject.GetComponent<PlatformEffector2D>() != null)
            {
                // Once player press the down button, the coroutine function will run
                // and ignore collsion between platform and player to allow player to drop
                // down from the platform
                // .
                platform = null;
            }
        }
    }

    IEnumerator DropDown()
    {
        BoxCollider2D boxCol2d = platform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), boxCol2d, true);
        yield return new WaitForSeconds(0.4f);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), boxCol2d, false);
    }
}
