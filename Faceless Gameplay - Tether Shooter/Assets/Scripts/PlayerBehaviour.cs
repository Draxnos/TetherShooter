using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public Camera cam;
    public Rigidbody rb;
    public CapsuleCollider coll;

    public bool grounded = false;
    public PhysicMaterial normal;
    public PhysicMaterial slip;

    public Vector3 groundNormal;
    public Vector3 oldNormal;
    public Vector3 groundPoint;
    public Vector3 curveCenterBottom;
    public Vector3 savedVelocity;
    public float speedForce = 100f;
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    public float crouchSpeed = 5f;
    public float speedCap = 10f;
    public Vector3 finalMove;

    public float jumpForce = 200f;
    public bool jumped = false;
    public bool wallHop;
    public bool wallHopped = false;
    public Vector3 wallNormal;
    public Vector3 perpRight;
    public Vector3 perpLeft;

    public bool crouched = false;

    public TetherBehaviour tb;

    public KeyCode jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftControl;
    public KeyCode crouch = KeyCode.LeftShift;
    public float sensitivity = 200f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        tb = gameObject.GetComponent<TetherBehaviour>();
    }

    void Update()
    {
        CameraMovement();
        Crouch();
        Jump();

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKey(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    void FixedUpdate()
    {
        if (grounded == true)
        {
            coll.material = normal;

            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                Movement();
            }
        }
        else
        {
            coll.material = slip;
        }
    }

    /// <summary>
    /// Calls GroundCheck, inserting its contact points
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        GroundCheck(collision.contacts);

    }

    /// <summary>
    /// Un-grounds and disables wall hop (in the case either is enabled
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
        wallHop = false;
    }

    /// <summary>
    /// Controls the camera
    /// </summary>
    void CameraMovement()
    {
        xRotation = Mathf.Clamp(xRotation - Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime);
    }

    /// <summary>
    /// Alters camera height and collider height to simulate crouching
    /// </summary>
    void Crouch()
    {
        if (Input.GetKeyDown(crouch))
        {
            coll.height /= 1.5f;
            coll.center = new Vector3(0, -0.25f, 0);
            cam.transform.localPosition = new Vector3(0, 0, 0);
            crouched = true;
        }

        if (Input.GetKeyUp(crouch))
        {
            coll.height *= 1.5f;
            coll.center = new Vector3(0, 0, 0);
            cam.transform.localPosition = new Vector3(0, 0.5369999f, 0);
            crouched = false;
        }
    }

    /// <summary>
    /// Contains the script for walking based on where the player is touching the ground (needs revising for corners)
    /// </summary>
    void Movement()
    {
        if (crouched == true)
        {
            speedCap = crouchSpeed;
        }
        else if (Input.GetKey(sprint))
        {
            speedCap = sprintSpeed;
        }
        else
        {
            speedCap = walkSpeed;
        }

        Vector3 moveDir = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targDir = Vector3.ProjectOnPlane(moveDir * 0.1f, groundNormal) - groundNormal * coll.radius * 1.2f;

        Ray ray = new Ray(curveCenterBottom, targDir);
        RaycastHit hit;
        float dist = targDir.magnitude;
        Debug.DrawRay(curveCenterBottom, targDir, Color.red, coll.radius * 1.1f);

        if (Physics.Raycast(ray, out hit, dist))
        {
            finalMove = hit.point - groundPoint;
        }
        else
        {
            finalMove = Vector3.ProjectOnPlane(moveDir, groundNormal);
        }

        if (rb.velocity.magnitude < speedCap)
        {
            rb.AddForce(finalMove.normalized * speedForce, ForceMode.Impulse);
        }
        else
        {
            rb.velocity = finalMove.normalized * speedCap;
        }
    }

    /// <summary>
    /// Used to jump or jump off walls (when possible)
    /// 
    /// Should be considered incomplete, there must be a better way to wall hop
    /// </summary>
    void Jump()
    {
        if (Input.GetKeyDown(jump))
        {
            if (grounded == true)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                grounded = false;
                jumped = true;
            }
            else if (wallHop == true)
            {
                Quaternion correction = Quaternion.identity;

                Vector3 left = Quaternion.AngleAxis(10f, Vector3.up) * perpLeft;
                Vector3 right = Quaternion.AngleAxis(-10f, Vector3.up) * perpRight;
                float angleSize = Vector3.Angle(left, right);

                float angleLeft = Vector3.Angle(transform.forward, left);
                float angleRight = Vector3.Angle(right, transform.forward);
                float angleTLeft = Vector3.Angle(perpLeft, transform.forward);
                float angleTRight = Vector3.Angle(transform.forward, perpRight);

                Vector3 hopDir = Quaternion.AngleAxis(45, -transform.right) * transform.forward;

                if (angleLeft + angleRight > angleSize)
                {
                    if (angleTLeft < 20)
                    {
                        correction = Quaternion.AngleAxis(angleLeft, Vector3.up);
                    }
                    else if (angleTRight < 20)
                    {
                        correction = Quaternion.AngleAxis(angleRight, Vector3.down);
                    }
                    else
                    {
                        correction = Quaternion.AngleAxis(180, Vector3.up);
                    }
                }

                rb.AddForce(correction * hopDir.normalized * jumpForce * 1.5f, ForceMode.VelocityChange);
            }
        }
    }

    float xRotation;

    /// <summary>
    /// Script used to find a grounding or wall hop-off point
    /// </summary>
    /// <param name="contacts">
    /// Contacts gathered when OnCollisionEnter is called
    /// </param>
    void GroundCheck(ContactPoint[] contacts)
    {
        curveCenterBottom = coll.bounds.center - Vector3.up * (coll.bounds.extents.y - coll.radius);
        Vector3 curveCenterTop = coll.bounds.center + Vector3.up * (coll.bounds.extents.y - coll.radius);
        float slopeCorrectionLength;

        foreach (ContactPoint c in contacts)
        {
            Vector3 dir = curveCenterBottom - c.point;
            Vector3 dir2 = c.point - curveCenterTop;
            slopeCorrectionLength = dir.magnitude;

            if (dir.y > 0f && Mathf.Abs(Vector3.Angle(c.normal, Vector3.up)) <= 50)
            {
                if(groundNormal != null)
                {
                    oldNormal = groundNormal;
                }
                
                groundNormal = c.normal;
                groundPoint = c.point;

                grounded = true;
                wallHop = false;
                jumped = false;
            }
            else if (dir2.y < 0f && grounded == false)
            {
                wallHop = true;

                wallNormal = c.normal.normalized;
                perpLeft = Vector3.Cross(wallNormal, Vector3.up);
                perpRight = Vector3.Cross(Vector3.up, wallNormal);
            }
        }
    }
}
