using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController theDude;
    // basic movement
    public float moveSpeed;
    public float jumpForce;
    public float gravityScale;
    public float crouchJumpForce;
    // crouch jump
    public float crouchSpeedMultiplier;
    // wall jump
    public float wallJumpForce;
    public float maxWallJumpTime;
    // slide
    public float maxSlideTime;
    public float slideSpeed;
    // long jump
    public float maxLongJumpTime;
    public float longJumpSpeed;
    // Animator reference
    public Animator anim;
    public Transform pivot;
    public float rotateSpeed;
    public GameObject playerModel;

    // dir
    private Vector3 moveDirection;
    private Vector3 wallDir;
    private Vector3 slideDir;
    // store
    private float moveSpeedStore;
    // checks
    private int jumpNum;
    private int lastTouchedWall;
    private bool touchUniqueWall;
    private bool touchWall;
    private bool isMoving;
    private bool isWallJumping;
    private bool isSliding;
    private bool isLongJumping;
    // time track
    private float timeStartWJ;
    private float timeStartSlide;
    private float timeStartLJ;

    private bool hasJumped;
    private bool hasDoubleJumped;

    // Fall damage
    public float safeFallTime;
    private float fallTime;
    public float damagePerSecond;

    // Player health
    public int playerHealth;
    

    // Use this for initialization
    void Start()
    {
        isMoving = false;
        moveSpeedStore = moveSpeed;
        playerHealth = 100;
        theDude = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        hasJumped = false;
        hasDoubleJumped = false;

        // Check for fall damage
        if (!theDude.isGrounded)
        {
            fallTime += Time.deltaTime;
        }
        else
        {
            // Apply damage if the fall time exceeds the safe time
            // Safe fall time is configurable in the Unity editor
            if (fallTime > safeFallTime)
            {
                playerHealth -= (int)(fallTime * damagePerSecond);
            }
            fallTime = 0.0f;
        }

        // Display players health on the panel
        if (transform.position.y <= 1.5)
        {
            playerHealth -= 1;
            FindObjectOfType<GameManager>().addPlayerHealth(playerHealth);
        }


        // Save the y axis to keep jumping consistent
        float yStore = moveDirection.y;

        if (isLongJumping)
        {
            moveSpeed = longJumpSpeed;
        }

        // Move based on the x and y orientation
        moveDirection = (transform.forward * Input.GetAxis("Vertical") * moveSpeed) + (transform.right * Input.GetAxis("Horizontal") * moveSpeed);

        // Removed to stop movement drift while running
        // The avatar should stop immediately when you let up on the w key
        //moveDirection = moveDirection.normalized * moveSpeed;

        // moving checks
        // TODO not used for anything yet
        if (moveDirection.magnitude > 0)
        {
            isMoving = true;
            //Debug.Log("moving");
        }
        else
        {
            isMoving = false;
            //Debug.Log("not moving");
        }

        // Reset the y axis
        moveDirection.y = yStore;

        // Coutneract gravity so falling off of a ledge looks more normal
        if (theDude.isGrounded && !isSliding)
        {
            moveSpeed = moveSpeedStore;
            touchUniqueWall = false;
            touchWall = false;
            isWallJumping = false;
            isLongJumping = false;
            lastTouchedWall = 0;
            moveDirection.y = 0;
            jumpNum = 0;
            wallDir = new Vector3(0, 0, 0);

            // crouch
            if (Input.GetButton("Crouch"))
            {
                moveDirection.x *= crouchSpeedMultiplier;
                moveDirection.z *= crouchSpeedMultiplier;
            }

            // slide
            if (Input.GetButton("Mod") && Input.GetButtonDown("Crouch"))
            {
                timeStartSlide = Time.time;
                isSliding = true;
                slideDir.x = moveDirection.normalized.x;
                slideDir.z = moveDirection.normalized.z;
            }

            // crouch jump
            if (Input.GetButton("Crouch") && Input.GetButtonDown("Jump"))
            {
                jumpNum += 2;
                moveDirection.y = crouchJumpForce;
            }
            // long jump
            else if (Input.GetButton("Mod") && Input.GetButtonDown("Jump"))
            {
                jumpNum += 2;
                timeStartLJ = Time.time;
                isLongJumping = true;
                moveSpeedStore = moveSpeed;
                moveDirection.y = jumpForce;
            }
            // jump
            else if (Input.GetButtonDown("Jump"))
            {
                hasJumped = true;
                jumpNum += 1;
                moveDirection.y = jumpForce;
            }
        }
        // check if player is allowed to double jump
        else if (jumpNum < 2)
        {
            // double jump
            if (Input.GetButtonDown("Jump"))
            {
                // when falling, jumpNum = 0, so add 2 to prevent
                // double jump after falling from edge
                hasDoubleJumped = true;
                jumpNum += 2;
                moveDirection.y = jumpForce;
            }
        }


        if (touchUniqueWall)
        {
            // wall jump
            if (Input.GetButtonDown("Jump"))
            {
                // when falling, jumpNum = 0, so add 2 to prevent
                // double jump after falling from edge
                moveDirection.y = jumpForce;
                touchUniqueWall = false;
                isWallJumping = true;
                timeStartWJ = Time.time;
            }
            // if previously wall jumping and hit a wall but don't jump again
            else
            {
                isWallJumping = false;
            }
        }

        /* TODO figure out how to reset moveDirection.y after leaving wall
        if(touchWall) {
        	yStore = moveDirection.y;
        	moveDirection.y = 0;
        }*/

        // check if player is walljumping and if under max wall jump time
        if (isWallJumping && (Time.time - timeStartWJ) <= maxWallJumpTime)
        {
            // set the wall jump direction in direction normal to wall
            moveDirection.x = wallJumpForce * wallDir.x;
            moveDirection.z = wallJumpForce * wallDir.z;
        }

        // check if player is sliding and if under max slide time
        if (isSliding && (Time.time - timeStartSlide) <= maxSlideTime)
        {
            // set the wall jump direction in direction normal to wall
            moveDirection.x = slideSpeed * slideDir.x;
            moveDirection.z = slideSpeed * slideDir.z;
        }
        else
        {
            isSliding = false;
        }

        // Account for gravity and normalization for fall speed
        moveDirection.y = moveDirection.y + (gravityScale * Physics.gravity.y * Time.deltaTime);

        // Move the character and normalize for framerate
        theDude.Move(moveDirection * Time.deltaTime);

        // Move the character based on the camera direction
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // entering collision with wall, change state only if not last wall
        if (hit.collider.tag == "Wall" && lastTouchedWall != hit.collider.gameObject.GetInstanceID())
        {
            touchUniqueWall = true;
            lastTouchedWall = hit.collider.gameObject.GetInstanceID();
            wallDir = hit.normal;
        }

        if (hit.collider.tag == "Wall")
        {
            touchWall = true;
        }

        if(hit.collider.tag == "TrapDoor")
        {
            
            HingeJoint hingeJoint = hit.collider.gameObject.AddComponent<HingeJoint>();  //Platform swings on a hinge
           
            AudioSource audioSource = hit.collider.gameObject.GetComponent<AudioSource>();  //Get the objects audio source
            audioSource.Play();  // HA HA!            
                       
        }
        
        if(hit.collider.tag == "Raft")
        {

            theDude.transform.SetParent(hit.collider.gameObject.transform);
        }
        else
        {
            theDude.transform.SetParent(null);
        }
           
    }

    
}

