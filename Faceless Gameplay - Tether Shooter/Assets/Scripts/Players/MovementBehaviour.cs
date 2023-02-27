using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementBehaviour : MonoBehaviour
{
    public Rigidbody rb;
    public MenuBehaviour mb;
    public CollisionBehaviour cb;
    public CapsuleCollider coll;
    public Camera cam;

    public PlayerControls pcs;

    private InputAction move;
    private InputAction look;
    private InputAction crouch_;
    private InputAction sprint_;
    private InputAction jump_;

    /// <summary>
    /// Movement Shit
    /// </summary>
    public Vector3 moveDir;
    public Vector3 startPos;
    public bool sprint = false;
    public bool crouch = false;
    public float moveForce = 10;
    public float wallForce = 100;
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    public float crouchSpeed = 5f;
    public float speedCap = 10f;
    public float wallUp = 0.5f;

    public Vector3 finalMove;
    public bool canMove;

    /// <summary>
    /// Cam and Collider Shit
    /// </summary>
    public Vector3 standOffset = new Vector3(0f, 0.5369999f, 0f);
    public Vector3 crouchOffset = Vector3.zero;

    public Vector3 camOffset;

    private void Awake()
    {
        pcs = InputManager.pcs;

        look = pcs.Gameplay.Look;
        move = pcs.Gameplay.Movement;
        crouch_ = pcs.Gameplay.Crouch;
        sprint_ = pcs.Gameplay.Sprint;
        jump_ = pcs.Gameplay.Jump;

        crouch_.performed += OnCrouch;
        crouch_.canceled += OnCrouch;
        sprint_.performed += Sprint;
        sprint_.canceled += Sprint;

        rb = GetComponent<Rigidbody>();
        mb = GetComponent<MenuBehaviour>();
        cb = GetComponent<CollisionBehaviour>();
        coll = GetComponent<CapsuleCollider>();
        cam = GetComponentInChildren<Camera>();
    }

    private void OnEnable()
    {
        look.Enable();
        move.Enable();
        crouch_.Enable();
        sprint_.Enable();
        jump_.Enable();
        crouch_.performed += OnCrouch;
        crouch_.canceled += OnCrouch;
        sprint_.performed += Sprint;
        sprint_.canceled += Sprint;
    }

    private void OnDisable()
    {
        crouch_.performed -= OnCrouch;
        crouch_.canceled -= OnCrouch;
        sprint_.performed -= Sprint;
        sprint_.canceled -= Sprint;
        look.Disable();
        move.Disable();
        crouch_.Disable();
        sprint_.Disable();
        jump_.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Frames
    /// </summary>
    void Update()
    {
        if (!mb.paused)
        {
            canMove = true;
            moveDir = transform.TransformDirection(new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y));
            CameraMovement();
        }
    }

    /// <summary>
    /// Update, but for physics reliant items
    /// </summary>
    void FixedUpdate()
    {
        if (moveDir != Vector3.zero && !mb.paused)
        {
            Movement();
        }
    }

    float xRotation;

    /// <summary>
    /// Controls the camera
    /// </summary>
    void CameraMovement()
    {
        xRotation = Mathf.Clamp(xRotation - look.ReadValue<Vector2>().y * mb.sensitivity * 50f * Time.deltaTime, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * look.ReadValue<Vector2>().x * mb.sensitivity * 50f * Time.deltaTime);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!mb.paused && !sprint)
        {
            if (context.performed)
            {
                crouch = true;
                Crouch();
            }
            else if (context.canceled)
            {
                crouch = false;
                UnCrouch();
            }
        }
    }

    public void Crouch()
    {
        coll.height /= 1.5f;
        coll.center = new Vector3(0, -0.25f, 0);
        camOffset = crouchOffset;
        speedCap = crouchSpeed;
        cam.transform.localPosition = camOffset;
    }

    public void UnCrouch()
    {
        coll.height *= 1.5f;
        coll.center = new Vector3(0, 0, 0);
        camOffset = standOffset;
        speedCap = walkSpeed;
        cam.transform.localPosition = camOffset;
    }

    /// <summary>
    /// Contains the script for walking based on where the player is touching the ground (needs revising for corners)
    /// </summary>
    void Movement()
    {
        Vector3 sideForward = rb.velocity - Vector3.up * rb.velocity.y;
        float force = 0;

        //Apply speed
        if (rb.velocity.magnitude < speedCap)
        {
            if (cb.grounded)
            {
                finalMove = Vector3.ProjectOnPlane(moveDir, cb.groundNormal);
                force = moveForce;
            }
            else if (cb.wallHop)
            {
                float right = Vector3.Angle(moveDir, cb.perpRight);

                if (right != 90)
                {
                    if (right > 90)
                    {
                        finalMove = cb.perpLeft;
                    }
                    else
                    {
                        finalMove = cb.perpRight;
                    }

                    finalMove /= 8;
                    force = wallForce;
                }
            }
        }
        else
        {
            finalMove = Vector3.zero;
        }

        rb.AddForce(finalMove * force, ForceMode.Acceleration);

        if (sideForward.magnitude > speedCap && cb.grounded)
        {
            rb.velocity = Vector3.ClampMagnitude(sideForward, speedCap) + new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        if (crouch)
        {
            UnCrouch();
        }

        if (context.performed)
        {
            sprint = true;
            speedCap = sprintSpeed;
        }
        else if (context.canceled)
        {
            sprint = false;
            speedCap = walkSpeed;
        }
    }

    public void ResetPos()
    {
        transform.position = startPos;
    }
}
