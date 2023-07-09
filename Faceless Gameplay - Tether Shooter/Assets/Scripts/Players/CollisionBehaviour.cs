using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    public CharacterBehaviour chb;
    public CapsuleCollider coll;
    public MovementBehaviour mb;
    public Rigidbody rb;
    public TetherBehaviour tb;
    public JumpBehaviour jb;

    /// <summary>
    /// Grounding Shit
    /// </summary>
    public PhysicMaterial normal;
    public PhysicMaterial slip;
    public PhysicMaterial slide;
    public ContactPoint[] contacts;
    public RaycastHit hit;
    public Vector3 point;
    public Vector3 curveCenterBottom;
    public Vector3 curveCenterTop;
    public bool passthrough;

    /// <summary>
    /// Carried Values
    /// </summary>
    public float maxSlope = 40;
    public bool frictionSlide = false;
    public bool grounded = false;
    public bool jumped = false;
    public bool wallHop = false;
    public Vector3 groundNormal;
    public Vector3 wallNormal;
    public Vector3 perpLeft;
    public Vector3 perpRight;

    private void Start()
    {
        tb = GetComponent<TetherBehaviour>();
        rb = GetComponent<Rigidbody>();
        chb = GetComponent<CharacterBehaviour>();
        coll = GetComponent<CapsuleCollider>();
        jb = GetComponent<JumpBehaviour>();
        mb = GetComponent<MovementBehaviour>();
    }

    /// <summary>
    /// Update, but for physics reliant items
    /// </summary>
    void FixedUpdate()
    {
        if (frictionSlide)
        {
            if (coll.material != slide)
            {
                coll.material = slip;
            }
            else if (rb.velocity.magnitude <= 1 && mb.moveDir != Vector3.zero)
            {
                frictionSlide = false;
            }
        }
        else if (grounded == true && coll.material != normal)
        {
            coll.material = normal;
        }
        else if (coll.material != slip)
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
        passthrough = grounded;

        contacts = new ContactPoint[collision.contactCount];
        collision.GetContacts(contacts);
        GroundCheck(contacts);

        if (passthrough && grounded)
        {
            rb.velocity = Vector3.ProjectOnPlane(mb.moveDir * mb.speedCap, groundNormal);
        }
    }

    /// <summary>
    /// Use in case groundcheck failure
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        if (mb.canMove)
        {
            curveCenterBottom = coll.bounds.center - transform.up * (coll.bounds.extents.y - coll.radius);

            if (!grounded)
            {
                contacts = new ContactPoint[collision.contactCount];
                collision.GetContacts(contacts);
                GroundCheck(contacts);
            }
            else if (Physics.SphereCast(transform.position, coll.radius, -transform.up, out hit) && groundNormal != hit.normal)
            {
                groundNormal = hit.normal;
            }
        }
    }

    /// <summary>
    /// Use in case ground detection failure
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision == null || collision.contactCount == 0)
        {
            if (grounded && jb.ungroundDouble)
            {
                UngroundDoubleCheck();
            }
            else
            {
                grounded = false;
                wallHop = false;
                coll.material = slip;
                jb.ungroundDouble = false;
            }
        }
        else
        {
            contacts = new ContactPoint[collision.contactCount];
            collision.GetContacts(contacts);
            GroundCheck(contacts);
        }
    }

    void UngroundDoubleCheck()
    {
        curveCenterBottom = coll.bounds.center - transform.up * (coll.bounds.extents.y - coll.radius);

        if (tb.connected)
        {
            grounded = false;
            coll.material = slip;
            jb.ungroundDouble = false;
        }
        else if (Physics.SphereCast(transform.position, coll.radius, -transform.up, out hit))
        {
            Vector3 normal = hit.normal;

            if (Physics.Raycast(hit.point, transform.up, out hit, coll.height / 2) && Mathf.Abs(Vector3.Angle(normal, transform.up)) <= maxSlope)
            {
                rb.MovePosition(transform.position - transform.up * hit.distance);
                rb.velocity = Vector3.ProjectOnPlane(mb.moveDir * rb.velocity.magnitude, hit.normal);
            }
            else
            {
                grounded = false;
                coll.material = slip;
                jb.ungroundDouble = false;
            }
        }
        else
        {
            grounded = false;
            coll.material = slip;
            jb.ungroundDouble = false;
        }
    }

    /// <summary>
    /// Script used to find a grounding or wall hop-off point
    /// </summary>
    /// <param name="contacts_">
    /// Contacts gathered when OnCollisionEnter is called
    /// </param>
    void GroundCheck(ContactPoint[] contacts_)
    {
        point = Vector3.zero;
        groundNormal = Vector3.zero;
        wallHop = false;
        jumped = true;

        curveCenterBottom = coll.bounds.center - Vector3.up * (coll.bounds.extents.y - coll.radius);
        curveCenterTop = coll.bounds.center + Vector3.up * (coll.bounds.extents.y - coll.radius);

        foreach (ContactPoint c in contacts_)
        {
            Vector3 dir = curveCenterBottom - c.point;
            Vector3 dir2 = c.point - curveCenterTop;

            //Ground detect
            if (Mathf.Abs(Vector3.Angle(dir, transform.up)) < 90 && Mathf.Abs(Vector3.Angle(c.normal, transform.up)) <= maxSlope)
            {
                if (Mathf.Abs(Vector3.Angle(groundNormal, dir)) <= 4)
                {
                    groundNormal = c.normal;
                }
                else
                {
                    groundNormal = dir;
                }

                groundNormal = c.normal;

                jb.ungroundDouble = true;
                grounded = true;
                jumped = false;

                if (!frictionSlide)
                {
                    coll.material = normal;
                }
            }
            //Wall check
            else if (dir2.y < 0f)
            {
                wallNormal = c.normal;

                perpLeft = Vector3.Cross(wallNormal, Vector3.up);
                perpRight = Vector3.Cross(Vector3.up, wallNormal);

                wallHop = true;
            }
        }
    }
}
