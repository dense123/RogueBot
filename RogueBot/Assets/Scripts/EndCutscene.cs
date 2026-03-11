using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCutscene : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject PlayerImagePrefab, Panel, Canvas_, PlayerHurt;
    [SerializeField] Enemy[] enemies;
    GameObject player;

    Vector2 localScale;

    // Start is called before the first frame update
    void Start()
    {
        // Set camera for the 1st cutscene
        // .
        camera.transform.position = new Vector3(3.6f, -0.7f, -10f);
        foreach (Enemy enemy in enemies)
        {
            enemy.endCutscene = true; // Stop them from moving at the start
        }
        // Start cutscene
        StartCoroutine(Player());
    }

    bool movePlayer = false;
    bool moveBackPlayer=false;
    bool ranOnce = false;
    bool Cutscene1 = true;
    bool Cutscene2 = false;
    bool playerMoveHurt = false;

    // Update is called once per frame
    void Update()
    {
        if (player != null && !ranOnce)
        {
            ranOnce= true;
            rb = player.GetComponent<Rigidbody2D>();
            localScale = player.transform.localScale;   
        }

        if (Cutscene2)
        {
            rb = PlayerHurt.GetComponent<Rigidbody2D>();
            localScale = PlayerHurt.transform.localScale;
        }

        
        if (Cutscene2)
        {
            StartCoroutine(Cutscene2Time());
        }
    }

    private void FixedUpdate()
    {
        if (movePlayer)
        {
            move(200f * Time.deltaTime);
        }
        if (moveBackPlayer)
        {
            move(100f * Time.deltaTime);
        }

        if (playerMoveHurt)
        {
            move(150f * Time.deltaTime);
        }
    }

    void move(float move)
    {
        rb.velocity = new Vector2(move, rb.velocity.y);
    }

    IEnumerator Player()
    {
        yield return new WaitForSeconds(2f);
        player = Instantiate(PlayerImagePrefab, transform);
        yield return new WaitForSeconds(1f);
        flipSprite();
        yield return new WaitForSeconds(1f);
        flipSprite();
        yield return new WaitForSeconds(1.5f);
        movePlayer = true;
        yield return new WaitForSeconds(1.5f);
        movePlayer = false;
        yield return new WaitForSeconds(0.5f);
        flipSprite();
        yield return new WaitForSeconds(1f);
        flipSprite();
        yield return new WaitForSeconds(1f);
        //spawn enemies
        foreach (Enemy enemy in enemies)
        {
            enemy.endCutscene = false;
        }
        yield return new WaitForSeconds(1f);
        flipSprite();
        yield return new WaitForSeconds(1f);
        moveBackPlayer = true;
        camera.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(3f);
        moveBackPlayer = false;
        Canvas_.SetActive(true);
        Cutscene1 = false;
        yield return new WaitForSeconds(1f);
        Cutscene2 = true;

    }

    IEnumerator Cutscene2Time()
    {
        // add night noises
        yield return new WaitForSeconds(3f);
        Cutscene2 = false;
        //move camera
        camera.transform.position = new Vector3(4.55f, -26.7f, -10f);
        //remove canvas
        Canvas_.SetActive(false);
        yield return new WaitForSeconds(3f);
        playerMoveHurt = true;
        yield return new WaitForSeconds(3f);
        playerMoveHurt = false;
        yield return new WaitForSeconds(3f);
        Canvas_.SetActive(true);
        yield return new WaitForSeconds(5f);
        // Go to back to the main menu which is the 1st scene
        SceneManager.LoadScene(0);

    }

    void flipSprite()
    {
        localScale.x *= -1;
        player.transform.localScale = localScale;
    }

    // cinematic view animation
    //
    // player spawns and drops
    //
    // looks left then right
    //
    // goes right to the door
    //
    // looks left long then right
    //
    // enemies come in
    //
    // player panics
    //
    // sudden darkness
    //
    // transition to outside where everything is black but the city
    //
    // player is blocked in with fences or bars
    //
    // player dies.




}
