using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed;

    public float groundDrag;

    public float JumpForce;
    public float JumpCD;
    public float AirMultiplier;
    bool ReadyToJump;

    [Header("Ground Check")]
    float PlayerHeight;
    public LayerMask WhatIsGround;
    bool Grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        ReadyToJump = true;
        PlayerHeight = transform.localScale.y;
    }

    private void Update()
    {
        //Gleda jesi li na podu
        Grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, WhatIsGround);

        MyInput();
        SpeedControl();

        //dodaje drag
        if (Grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Kada skociti
        if(Input.GetKey(KeyCode.Space) && ReadyToJump && Grounded)
        {
            ReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), JumpCD);
        }
        
    }

    private void MovePlayer()
    {
        //Kalkurira smjer kretanja
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Na podu
        if(Grounded)
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);

        //U zraku
        else
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f * AirMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limitiranje velocity-a
        if (flatVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * MoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        ReadyToJump = true;
    }
}

