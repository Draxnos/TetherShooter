using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    public Rigidbody rb;
    public MenuBehaviour mb;
    public CollisionBehaviour cb;
    public CapsuleCollider coll;
    public Camera cam;

    /// <summary>
    /// Movement Shit
    /// </summary>
    public int hori;
    public int vert;
    public Vector3 moveDir;
    public Vector3 startPos;
    public float moveForce = 10;
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    public float crouchSpeed = 5f;
    public float speedCap = 10f;

    public Vector3 finalMove;

    public bool crouched = false;

    /// <summary>
    /// Cam and Collider Shit
    /// </summary>
    public Vector3 standOffset = new Vector3(0f, 0.5369999f, 0f);
    public Vector3 crouchOffset = Vector3.zero;

    public Vector3 camOffset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mb = GetComponent<MenuBehaviour>();
        cb = GetComponent<CollisionBehaviour>();
        coll = GetComponent<CapsuleCollider>();
        cam = GetComponentInChildren<Camera>();
        startPos = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Frames
    /// </summary>
    void Update()
    {
        vert = (Input.GetKey(mb.keys[0]) ? 1 : 0) * 1 + (Input.GetKey(mb.keys[1]) ? 1 : 0) * -1;
        hori = (Input.GetKey(mb.keys[3]) ? 1 : 0) * 1 + (Input.GetKey(mb.keys[2]) ? 1 : 0) * -1;


        if (!mb.paused)
        {
            moveDir = (transform.right * hori + transform.forward * vert).normalized;
            CameraMovement();
            Crouch();
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
        xRotation = Mathf.Clamp(xRotation - Input.GetAxis("Mouse Y") * mb.sensitivity * 50f * Time.deltaTime, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mb.sensitivity * 50f * Time.deltaTime);
    }

    void Crouch()
    {
        //Crouch
        if (Input.GetKeyDown(mb.keys[5]) && crouched == false)
        {
            coll.height /= 1.5f;
            coll.center = new Vector3(0, -0.25f, 0);
            camOffset = crouchOffset;
            crouched = true;
        }

        //Uncrouch
        if (Input.GetKeyUp(mb.keys[5]) && crouched == true)
        {
            coll.height *= 1.5f;
            coll.center = new Vector3(0, 0, 0);
            camOffset = standOffset;
            crouched = false;
        }
    }

    /// <summary>
    /// Contains the script for walking based on where the player is touching the ground (needs revising for corners)
    /// </summary>
    void Movement()
    {
        //Set speed
        if (crouched == true)
        {
            speedCap = crouchSpeed;
        }
        else if (Input.GetKey(mb.keys[4]))
        {
            speedCap = sprintSpeed;
        }
        else
        {
            speedCap = walkSpeed;
        }

        //Apply speed
        if (rb.velocity.magnitude < speedCap)
        {
            if (cb.grounded)
            {
                finalMove = Vector3.ProjectOnPlane(moveDir, cb.groundNormal);
            }
            else if (cb.wallHop)
            {
                float right = Vector3.Angle(transform.forward, cb.perpRight);
                float left = Vector3.Angle(transform.forward, cb.perpLeft);

                if (right != left)
                {
                    if (right > left)
                    {
                        finalMove = cb.perpLeft;
                    }
                    else
                    {
                        finalMove = cb.perpRight;
                    }

                    finalMove /= 4;
                }
            }
        }
        else
        {
            finalMove = Vector3.zero;
        }

        rb.AddForce(finalMove * moveForce, ForceMode.Impulse);
    }

    public void ResetPos()
    {
        transform.position = startPos;
    }
}
