using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public float moveSpeed;
    public float jumpForce;
    public float wallJumpForce;
    public CharacterController theDude;
    public float gravityScale;
    private Vector3 moveDirection;

    // Animator reference
    public Animator anim;

    public Transform pivot;
    public float rotateSpeed;
    public GameObject playerModel;

    private int jumpNum;
    private bool touchWall;
    private bool isWallJumping;
    private int lastTouchedWall;
    private float timeStartWJ;
    public float maxWallJumpTime;
    private Vector3 wallDir;

    private bool isMoving;

    private bool hasJumped;
    private bool hasDoubleJumped;

    // Use this for initialization
    void Start()
    {   
        isMoving = false;
        
        // theDude = GetComponent<Rigidbody>();
        theDude = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        hasJumped = false;
        hasDoubleJumped = false;
        // Save the y axis to keep jumping consistent
        float yStore = moveDirection.y;

        // Move based on the x and y orientation
        moveDirection = (transform.forward * Input.GetAxis("Vertical") * moveSpeed) + (transform.right * Input.GetAxis("Horizontal") * moveSpeed);

        // Removed to stop movement drift while running
        // The avatar should stop immediately when you let up on the w key
        //moveDirection = moveDirection.normalized * moveSpeed;

        // moving checks
        // TODO not used for anything yet
        if(moveDirection.magnitude > 0) {
            isMoving = true;
            //Debug.Log("moving");
        }
        else {
            isMoving = false;
            //Debug.Log("not moving");
        }

        // Reset the y axis
        moveDirection.y = yStore;       

        // Coutneract gravity so falling off of a ledge looks more normal
        if (theDude.isGrounded)
        {
            touchWall = false;
            isWallJumping = false;
            lastTouchedWall = 0;
            moveDirection.y = 0;
            jumpNum = 0;
            wallDir = new Vector3(0,0,0);

            //Jump when the space bar is pressed
            if (Input.GetButtonDown("Jump"))
            {
                hasJumped = true;
                jumpNum += 1;
                moveDirection.y = jumpForce;
            }
        }
        // check if player is allowed to double jump
        else if(jumpNum < 2) {
            // double jump
            if (Input.GetButtonDown("Jump")) {
                // when falling, jumpNum = 0, so add 2 to prevent
                // double jump after falling from edge
                hasDoubleJumped = true;
                jumpNum += 2;
                moveDirection.y = jumpForce;
            }
        }

        if(touchWall) {
            // wall jump
            if (Input.GetButtonDown("Jump")) {
                // when falling, jumpNum = 0, so add 2 to prevent
                // double jump after falling from edge
                moveDirection.y = jumpForce;
                touchWall = false;
                isWallJumping = true;
                timeStartWJ = Time.time;
            }
            // if previously wall jumping and hit a wall but don't jump again
            else {
                isWallJumping = false;
            }
        }

        // check if player is walljumping and if under max wall jump time
        if(isWallJumping && (Time.time-timeStartWJ) <= maxWallJumpTime) {
            // set the wall jump direction in direction normal to wall
            moveDirection.x = wallJumpForce * wallDir.x;
            moveDirection.z = wallJumpForce * wallDir.z;
        }

        // Account for gravity and normalization for fall speed
        moveDirection.y = moveDirection.y + (gravityScale * Physics.gravity.y * Time.deltaTime);

        // Move the character and normalize for framerate
        theDude.Move(moveDirection * Time.deltaTime);

        // Move the character based on the camera direction
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        // Values used in the animations
        anim.SetBool("isGrounded", theDude.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxis("Vertical")) + (Mathf.Abs(Input.GetAxis("Horizontal")))));
        anim.SetBool("doubleJump", hasDoubleJumped);
        anim.SetBool("hasJumped", hasJumped);

    }

    void OnControllerColliderHit(ControllerColliderHit hit){
         // entering collision with wall, change state only if not last wall
         if(hit.collider.tag == "Wall" && lastTouchedWall != hit.collider.gameObject.GetInstanceID()) {
            touchWall = true;
            lastTouchedWall = hit.collider.gameObject.GetInstanceID();
            wallDir = hit.normal;
         }
     }
}