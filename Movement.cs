using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Varijable

    [Header("Movement")]
    private float MoveSpeed;
    public float WalkSpeed;
    public float SprintSpeed;
    public float SlideSpeed;

    float DesiredMoveSpeed;
    float LastDesiredMoveSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float JumpForce;
    public float JumpCD;
    public float AirMultiplier;
    bool ReadyToJump;

    [Header("Crouching")]
    public float CrouchSpeed;
    public float CrouchYScale;
    private float StartYScale;

    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float PlayerHeight;
    public LayerMask WhatIsGround;
    bool Grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Stamina Handling")]
    public float MaxStamina;
    float Stamina;
    private bool IsRunning;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState State;
    public enum MovementState
    {
        //gleda u kojem su modu
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }

    public bool sliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //stavnja da mozes skociti na pocetku
        ReadyToJump = true;

        //gleda koji ti je y scale na pocetku prije crouchanja
        StartYScale = transform.localScale.y;

        //stavlja da ti je stamina na pocetku ista kao i max stamina
        Stamina = MaxStamina;
    }

    private void Update()
    {
        //Gleda jesi li na podu
        Grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.3f, WhatIsGround);

        //Stavja the voide u update da se ponavljaju stalno
        MyInput();
        SpeedControl();
        StateHandler();
        StaminaHandler();

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
        if (Input.GetKey(JumpKey) && ReadyToJump && Grounded)
        {
            ReadyToJump = false;

            Jump();

            //stavlja delay na jump
            Invoke(nameof(ResetJump), JumpCD);
        }

        //Pocinje Crouching
        if (Input.GetKeyDown(CrouchKey))
        {
            //smanjuje te kada cucnes
            transform.localScale = new Vector3(transform.localScale.x, CrouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //Zaustavi Crouching
        if (Input.GetKeyUp(CrouchKey))
        {
            //vraca ti tvoju visinu natrag kada prestanes cucati
            transform.localScale = new Vector3(transform.localScale.x, StartYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //Mode - Sliding
        if (sliding)
        {
            //stavlja te u taj mode
            State = MovementState.sliding;

            //Gleda jesi li na nagibu i onda ti povecava brzinu ako jesi s slidas se
            if (OnSlope() && rb.velocity.y < 0.1f)
                DesiredMoveSpeed = SlideSpeed;

            //Ako nisi vraca ti brzinu na staro
            else
                DesiredMoveSpeed = SprintSpeed;
        }

        //Mode - Crouching
        else if (Input.GetKey(CrouchKey))
        {
            //stavlja te u taj mode
            State = MovementState.crouching;
            //daje ti brzinu tog mode-a
            DesiredMoveSpeed = CrouchSpeed;
        }

        //Mode - trcanje
        else if (Grounded && Input.GetKey(SprintKey))
        {
            //stavja da je trcanje true jer nam to treba za limitiranje koliko dugo mozemo sprintati
            IsRunning = true;
            //stavlja te u taj mode
            State = MovementState.sprinting;
            //daje ti brzinu tog mode-a
            DesiredMoveSpeed = SprintSpeed;
        }

        //Mode - hodanje
        else if (Grounded)
        {
            //stavja da je trcanje false jer nam to treba za limitiranje koliko dugo mozemo sprintati
            IsRunning = false;
            //stavlja te u taj mode
            State = MovementState.walking;
            //daje ti brzinu tog mode-a
            DesiredMoveSpeed = WalkSpeed;
        }

        //Mode - zrak
        else
        {
            //stavlja te u taj mode
            State = MovementState.air;
        }

        //Gleda jeli se "desired move speed" promijenio drasticno
        if(Mathf.Abs(DesiredMoveSpeed - LastDesiredMoveSpeed) > 4f && MoveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            MoveSpeed = DesiredMoveSpeed;
        }

        LastDesiredMoveSpeed = DesiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        //poladano mice brzinu tijekom vremena
        float time = 0;
        float difference = Mathf.Abs(DesiredMoveSpeed - MoveSpeed);
        float startValue = MoveSpeed;

        while (time < difference)
        {
            MoveSpeed = Mathf.Lerp(startValue, DesiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        MoveSpeed = DesiredMoveSpeed;
    }

    private void MovePlayer()
    {
        //Kalkurira smjer kretanja
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Na nagibu
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * MoveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //Na podu
        if (Grounded)
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);

        //U zraku
        else
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f * AirMultiplier, ForceMode.Force);

        //Makni gravitaciju dok si na nagibu
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //limitiranje brzine na nagibg
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > MoveSpeed)
                rb.velocity = rb.velocity.normalized * MoveSpeed;
        }

        //limitiranje brzine na podu ili u zraku
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limitiranje velocity-a
            if (flatVel.magnitude > MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * MoveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        //Restira skok
        ReadyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        //Gleda ako si na Nagibu
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, PlayerHeight * 0.5f + 0.3f))
        {
            //stavja snagu koju stavljas da je u istom kutu kao i taj nagib
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void StaminaHandler()
    {
        if (Grounded && Input.GetKey(SprintKey))
        {
            //oduzima staminu kada drzis Left Shift
            Stamina -= Time.deltaTime;
            if (Stamina < 0)
            {
                //Ako je stamina 0 onda nemozes vise sprintati dok se ne napuni
                Stamina = 0;
                State = MovementState.walking;
                MoveSpeed = WalkSpeed;
            }
        }
        else if (Stamina < MaxStamina)
        {
            //vraca staminu ako ne drzis Left Shift
            Stamina += Time.deltaTime;
        }

    }
}

