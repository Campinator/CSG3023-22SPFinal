/**** 
 * Created by: Bob Baloney
 * Date Created: April 20, 2022
 * 
 * Last Edited by: Camp Steiner
 * Last Edited: April 28, 2022
 * 
 * Description: Controls the ball and sets up the intial game behaviors. 
****/

/*** Using Namespaces ***/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Ball : MonoBehaviour
{
    [Header("General Settings")]
    public int numBalls;
    public int score;
    public Text ballTxt;
    public Text scoreTxt;
    public Text endTxt;
    public GameObject paddle;
    public AudioSource audioSource;

    [Header("Ball Settings")]
    public float speed;
    public Vector3 initialForce;
    private bool isInPlay;
    private Rigidbody rb;
    public AudioClip bounceSfx;





    //Awake is called when the game loads (before Start).  Awake only once during the lifetime of the script instance.
    void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        this.audioSource = GetComponent<AudioSource>();
    }//end Awake()


    // Start is called before the first frame update
    void Start()
    {
        SetStartingPos(); //set the starting position

    }//end Start()


    // Update is called once per frame
    void Update()
    {
        ballTxt.text = "Balls: " + numBalls;
        scoreTxt.text = "Score: " + score;
        if (!isInPlay)
        {
            transform.position = new Vector3(paddle.transform.position.x, transform.position.y, transform.position.z);
            
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isInPlay)
        {
            isInPlay = true;
            Move();
        }

        GameObject[] bricksLeft = GameObject.FindGameObjectsWithTag("Brick");
        if(bricksLeft.Length == 0)
        {
            SetStartingPos();
            endTxt.text = "<color='#269A14'>You Win!</color>";
            Invoke("ResetScene", 5f);
        }
    }//end Update()


    private void LateUpdate()
    {
        if (isInPlay)
        {
            rb.velocity = speed * Vector3.Normalize(rb.velocity);
        }
    }//end LateUpdate()


    void SetStartingPos()
    {
        isInPlay = false;//ball is not in play
        rb.velocity = Vector3.zero;//set velocity to keep ball stationary

        Vector3 pos = new Vector3();
        pos.x = paddle.transform.position.x; //x position of paddel
        pos.y = paddle.transform.position.y + paddle.transform.localScale.y; //Y position of paddle plus it's height

        transform.position = pos;//set starting position of the ball 
    }//end SetStartingPos()

    void Move()
    {
        //add force to rigidbody
        rb.AddForce(initialForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(bounceSfx);
        GameObject otherGO = collision.gameObject;
        if(otherGO.tag == "Brick")
        {
            score += 100;
            Destroy(otherGO);
        }
        //if it hits the paddle, transfer some of the momentum
        if(isInPlay && otherGO.tag == "Paddle")
        {
            rb.velocity = new Vector3(Mathf.Min(rb.velocity.x + (speed * Input.GetAxis("Horizontal"))/2, speed), rb.velocity.y, rb.velocity.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "OutBounds")
        {
            numBalls--;
            if(numBalls > 0)
            {
                Invoke("SetStartingPos", 2f);
            }
            else
            {
                isInPlay = false;
                endTxt.text = "<color='#C80000'>You Lose!</color>";
                Invoke("ResetScene", 5f);
            }
        }
    }

    private void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");   
    }

}
