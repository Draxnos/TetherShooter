using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be used
/// </summary>
public struct TetherSegment
{
    public Vector3 curPos;
    public Vector3 oldPos;

    public TetherSegment(Vector3 pos)
    {
        curPos = pos;
        oldPos = pos;
    }
}

public class TetherBehaviour : MonoBehaviour
{
    public KeyCode sendTether = KeyCode.E;
    public KeyCode reel = KeyCode.C;
    public KeyCode unreel = KeyCode.V;

    public bool connected = false;

    public ConfigurableJoint cj;
    public float length;
    public float reelSpeed;
    public float reelForce;
    public Rigidbody rb;

    /*private LineRenderer lr;
    private List<TetherSegment> tetherSegments = new List<TetherSegment>();
    private float segmentLength;
    private int segments;
    private Vector3 startPoint;
    private Vector3 endPoint;*/

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

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

            if (Input.GetKey(reel))
            {
                TetherReel();
            }

            if (Input.GetKey(unreel))
            {
                TetherUnreel();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void SendTether()
    {
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            float dist = Vector3.Distance(hit.point, gameObject.transform.position);

            if (Vector3.Distance(hit.point, gameObject.transform.position) < 100)
            {
                cj.connectedAnchor = hit.point;
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

    /// <summary>
    /// Decreases the linear limit, reeling in
    /// </summary>
    void TetherReel()
    {
        if(cj.linearLimit.limit >= 1)
        {
            if(rb.velocity.magnitude < 80)
            {
                rb.AddForce((cj.connectedAnchor - transform.position).normalized * reelSpeed * Time.deltaTime);
            }

            SoftJointLimit distance = cj.linearLimit;
            distance.limit = Vector3.Distance(transform.position, cj.connectedAnchor);
            cj.linearLimit = distance;
        }
    }

    /// <summary>
    /// Increases the linear limit, reeling out
    /// </summary>
    void TetherUnreel()
    {
        SoftJointLimit distance = cj.linearLimit;
        float dist = Vector3.Distance(cj.connectedAnchor, gameObject.transform.position);
        distance.limit += reelSpeed * Time.deltaTime;
        cj.linearLimit = distance;
    }

    void CutTether()
    {
        cj.xMotion = ConfigurableJointMotion.Free;
        cj.yMotion = ConfigurableJointMotion.Free;
        cj.zMotion = ConfigurableJointMotion.Free;
        connected = false;
    }
}
