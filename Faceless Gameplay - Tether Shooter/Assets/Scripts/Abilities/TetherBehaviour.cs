using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TetherBehaviour : MonoBehaviour
{
    public PlayerControls pcs;

    public InputAction tetherSend;
    public InputAction tetherReel;
    public InputAction tetherUnreel;

    public ConfigurableJoint cj;
    public Rigidbody rb;
    public JumpBehaviour jb;
    public LineRenderer lr;
    public MenuBehaviour mb;
    public Image indicator;
    public RaycastHit hit;

    public bool canSend = false;
    public bool connected = false;
    public Vector3 camPos;

    public float maxLength;
    public float reelForce;
    public float reelSpeed;
    public int reelDir;
    public bool tetherJump = false;

    public void Awake()
    {
        pcs = InputManager.pcs;

        tetherSend = pcs.Gameplay.SendTether;
        tetherReel = pcs.Gameplay.ReelTether;
        tetherUnreel = pcs.Gameplay.UnreelTether;

        tetherSend.performed += OnTetherSend;
        tetherReel.performed += _ => reelDir++;
        tetherUnreel.performed += _ => reelDir--;
        tetherReel.canceled += _ => reelDir--;
        tetherUnreel.canceled += _ => reelDir++;
    }

    private void OnEnable()
    {
        tetherSend.Enable();
        tetherReel.Enable();
        tetherUnreel.Enable();
        tetherSend.performed += OnTetherSend;
        tetherReel.performed += _ => reelDir++;
        tetherUnreel.performed += _ => reelDir--;
        tetherReel.canceled += _ => reelDir--;
        tetherUnreel.canceled += _ => reelDir++;
    }

    private void OnDisable()
    {
        tetherSend.performed -= OnTetherSend;
        tetherReel.performed -= _ => reelDir++;
        tetherUnreel.performed -= _ => reelDir--;
        tetherReel.canceled -= _ => reelDir--;
        tetherUnreel.canceled -= _ => reelDir++;
        tetherSend.Disable();
        tetherReel.Disable();
        tetherUnreel.Disable();
    }

    void Start()
    {
        camPos = Camera.main.transform.localPosition;
        rb = GetComponent<Rigidbody>();
        cj = GetComponent<ConfigurableJoint>();
        lr = GetComponent<LineRenderer>();
        mb = GetComponent<MenuBehaviour>();
        jb = GetComponent<JumpBehaviour>();
    }

    /// <summary>
    /// Runs once per frame
    /// </summary>
    void Update()
    {
        if (Physics.Raycast(transform.position + camPos, Camera.main.transform.forward, out hit, maxLength))
        {
            indicator.color = Color.green;
            canSend = true;
        }
        else
        {
            indicator.color = Color.red;
            canSend = false;
        }

        if (connected)
        {
            lr.SetPosition(0, transform.position);
        }
    }

    /// <summary>
    /// Runs physics reliant code
    /// </summary>
    void FixedUpdate()
    {
        if (connected == true)
        {
            if (reelDir > 0)
            {
                TetherReel();
            }
            else if (reelDir < 0)
            {
                TetherUnreel();
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

    public void OnTetherSend(InputAction.CallbackContext context)
    {
        if (!mb.paused)
        {
            if (!connected && canSend)
            {
                SendTether(hit, maxLength);
                jb.ungroundDouble = false;
            }
            else
            {
                CutTether();
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
        if (!mb.paused && cj.linearLimit.limit >= 1)
        {
            rb.AddForce((cj.connectedAnchor - transform.position).normalized * reelForce * Time.deltaTime, ForceMode.Impulse);

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
        if (!mb.paused && cj.linearLimit.limit != maxLength)
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
