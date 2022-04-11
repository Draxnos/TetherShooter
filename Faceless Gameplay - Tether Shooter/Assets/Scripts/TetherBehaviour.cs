using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be used
/// </summary>

public class TetherBehaviour : MonoBehaviour
{
    public KeyCode sendTether = KeyCode.E;
    public KeyCode reel = KeyCode.C;
    public KeyCode unreel = KeyCode.V;

    public bool connected = false;

    public ConfigurableJoint cj;
    public Rigidbody rb;
    public float maxLength;
    public float reelSpeed;
    public bool tetherJump = false;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Runs once per frame
    /// </summary>
    void Update()
    {
        if(connected == false)
        {
            if (Input.GetKeyDown(sendTether))
            {
                SendTether();
            }
        }
        else
        {
            if (Input.GetKeyDown(sendTether))
            {
                CutTether();
            }
        }
    }

    /// <summary>
    /// Runs physics reliant code
    /// </summary>
    void FixedUpdate()
    {
        if (connected == true)
        {
            if (Input.GetKeyUp(reel) || Input.GetKeyUp(unreel))
            {
                reelTimer = 0.5f;
            }

            if (Input.GetKey(reel))
            {
                TetherReel();
            }
            else if (Input.GetKey(unreel) && cj.linearLimit.limit < maxLength)
            {
                TetherUnreel();
            }

            if ((transform.position - cj.connectedAnchor).magnitude >= cj.linearLimit.limit)
            {
                tetherJump = true;
            }
            else
            {
                tetherJump = false;
            }
        }
    }

    /// <summary>
    /// Creates a tether if the player is looking at a valid point
    /// </summary>
    void SendTether()
    {
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            float dist = Vector3.Distance(hit.point, gameObject.transform.position);

            if (Vector3.Distance(hit.point, gameObject.transform.position) < maxLength)
            {
                cj.xMotion = ConfigurableJointMotion.Limited;
                cj.yMotion = ConfigurableJointMotion.Limited;
                cj.zMotion = ConfigurableJointMotion.Limited;

                cj.connectedAnchor = hit.point;
                SoftJointLimit ll = cj.linearLimit;
                ll.limit = dist;
                cj.linearLimit = ll;
                connected = true;
            }
        }
    }

    public float reelTimer = 0.5f;

    /// <summary>
    /// Decreases tether length
    /// </summary>
    void TetherReel()
    {
        if (reelTimer < 1)
        {
            reelTimer += Time.fixedDeltaTime / 2;
        }
        else
        {
            reelTimer = 1;
        }

        if (cj.linearLimit.limit >= 1)
        {
            rb.AddForce((cj.connectedAnchor - transform.position).normalized * reelSpeed * reelTimer * Time.deltaTime, ForceMode.Impulse);

            SoftJointLimit distance = cj.linearLimit;
            distance.limit = Vector3.Distance(transform.position, cj.connectedAnchor);
            cj.linearLimit = distance;
        }
    }

    /// <summary>
    /// Increases tether length
    /// </summary>
    void TetherUnreel()
    {
        if (cj.linearLimit.limit != maxLength)
        {
            SoftJointLimit distance = cj.linearLimit;

            if (cj.linearLimit.limit < maxLength)
            {
                distance.limit += reelSpeed * Time.fixedDeltaTime;
                cj.linearLimit = distance;
            }
            else
            {
                distance.limit = maxLength;
                cj.linearLimit = distance;
            }
        }
    }

    /// <summary>
    /// Cuts the tether
    /// </summary>
    void CutTether()
    {
        cj.xMotion = ConfigurableJointMotion.Free;
        cj.yMotion = ConfigurableJointMotion.Free;
        cj.zMotion = ConfigurableJointMotion.Free;

        connected = false;
    }
}
