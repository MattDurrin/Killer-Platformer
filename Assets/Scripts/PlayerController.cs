﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public float moveSpeed;
    public float jumpForce;
    public CharacterController theDude;
    public float gravityScale;
    private Vector3 moveDirection;

    // Animator reference
    public Animator anim;

    public Transform pivot;
    public float rotateSpeed;
    public GameObject playerModel;

    // Use this for initialization
    void Start()
    {
        //theDude = GetComponent<Rigidbody>();
        theDude = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Save the y axis to keep jumping consistent
        float yStore = moveDirection.y;

        //Move based on the x and y orientation
        moveDirection = (transform.forward * Input.GetAxis("Vertical") * moveSpeed) + (transform.right * Input.GetAxis("Horizontal") * moveSpeed);
        moveDirection = moveDirection.normalized * moveSpeed;

        //Reset the y axis
        moveDirection.y = yStore;
                
        //Coutneract gravity so falling off of a ledge looks more normal
        if (theDude.isGrounded)
        {

            moveDirection.y = 0;

            //Jump when the space bar is pressed
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }

        }

        //Account for gravity and normalization for fall speed
        moveDirection.y = moveDirection.y + (gravityScale * Physics.gravity.y * Time.deltaTime);

        //Move the character and normalize for framerate
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

    }
}