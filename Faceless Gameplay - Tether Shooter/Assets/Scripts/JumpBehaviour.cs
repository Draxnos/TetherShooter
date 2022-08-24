using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehaviour : MonoBehaviour
{
    public Rigidbody rb;
    public TetherBehaviour tb;
    public CollisionBehaviour cb;
    public MenuBehaviour mb;

    /// <summary>
    /// Jump shit
    /// </summary>
    public float jumpForce = 200f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        tb = GetComponent<TetherBehaviour>();
        cb = GetComponent<CollisionBehaviour>();
        mb = GetComponent<MenuBehaviour>();
    }

    /// <summary>
    /// Frames
    /// </summary>
    void Update()
    {
        if (!mb.paused)
        {
            Jump();
        }
    }

    /// <summary>
    /// Used to jump or jump off walls (when possible)
    /// 
    /// Should be considered incomplete, there must be a better way to wall hop
    /// </summary>
    void Jump()
    {
        if (Input.GetKeyDown(mb.keys[6]))
        {
            //Regular jump
            if (cb.grounded == true)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            //Wallhop
            else if (cb.wallHop == true)
            {
                Quaternion correction = Quaternion.identity;

                Vector3 left = Quaternion.AngleAxis(10f, Vector3.up) * cb.perpLeft;
                Vector3 right = Quaternion.AngleAxis(-10f, Vector3.up) * cb.perpRight;
                float angleSize = Vector3.Angle(left, right);

                float angleLeft = Vector3.Angle(transform.forward, left);
                float angleRight = Vector3.Angle(right, transform.forward);
                float angleTLeft = Vector3.Angle(cb.perpLeft, transform.forward);
                float angleTRight = Vector3.Angle(transform.forward, cb.perpRight);

                Vector3 hopDir = Quaternion.AngleAxis(45, -transform.right) * transform.forward;

                //Angle depth correction
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

                rb.AddForce(correction * hopDir.normalized * jumpForce * 1.5f, ForceMode.Impulse);
            }
            //Tether hop
            else if (tb.tetherJump == true)
            {
                rb.AddForce((tb.cj.connectedAnchor - transform.position).normalized * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
