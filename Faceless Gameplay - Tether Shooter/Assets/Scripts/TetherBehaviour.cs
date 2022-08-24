using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetherBehaviour : MonoBehaviour
{
    public ConfigurableJoint cj;
    public Rigidbody rb;
    public LineRenderer lr;
    public MenuBehaviour mb;
    public Image indicator;

    public bool connected = false;

    public float maxLength;
    public float reelForce;
    public float reelSpeed;
    public bool tetherJump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cj = GetComponent<ConfigurableJoint>();
        lr = GetComponent<LineRenderer>();
        mb = GetComponent<MenuBehaviour>();
    }

    /// <summary>
    /// Runs once per frame
    /// </summary>
    void Update()
    {
        if(connected == false)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxLength))
            {
                indicator.color = Color.green;

                if (Input.GetKeyDown(mb.keys[7]) && !mb.paused)
                {
                    SendTether(hit, Vector3.Distance(hit.point, gameObject.transform.position));
                }
            }
            else
            {
                indicator.color = Color.red;
            }
        }
        else
        {
            lr.SetPosition(0, transform.position);

            if (Input.GetKeyDown(mb.keys[7]) && !mb.paused)
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
            if (Input.GetKeyUp(mb.keys[8]) || Input.GetKeyUp(mb.keys[9]))
            {
                reelTimer = 0.5f;
            }

            if (!mb.paused)
            {
                if (Input.GetKey(mb.keys[8]))
                {
                    TetherReel();
                }
                else if (Input.GetKey(mb.keys[9]) && cj.linearLimit.limit < maxLength)
                {
                    TetherUnreel();
                }
            }

            if ((transform.position - cj.connectedAnchor).magnitude >= cj.linearLimit.limit + 1 && connected == true)
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
    void SendTether(RaycastHit hit, float dist)
    {
        lr.enabled = true;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, hit.point);

        cj.xMotion = ConfigurableJointMotion.Limited;
        cj.yMotion = ConfigurableJointMotion.Limited;
        cj.zMotion = ConfigurableJointMotion.Limited;

        cj.connectedAnchor = hit.point;
        SoftJointLimit ll = cj.linearLimit;
        ll.limit = dist;
        cj.linearLimit = ll;
        connected = true;
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
            rb.AddForce((cj.connectedAnchor - transform.position).normalized * reelForce * reelTimer * Time.deltaTime, ForceMode.Impulse);

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
        lr.enabled = false;
    }
}
