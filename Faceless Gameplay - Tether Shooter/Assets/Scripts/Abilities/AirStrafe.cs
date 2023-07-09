using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirStrafe : MonoBehaviour
{
    public PlayerControls pcs;

    public InputAction move;

    public MenuBehaviour mb;

    public MovementBehaviour mvb;
    public CollisionBehaviour cb;
    public Rigidbody rb;

    public bool canMove = true;
    public Vector3 moveDir;
    public float airForce;
    public float speedCap;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mb = GetComponent<MenuBehaviour>();
        cb = GetComponent<CollisionBehaviour>();
        mvb = GetComponent<MovementBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mb.paused)
        {
            canMove = !mb.paused && !cb.grounded;
            moveDir = mvb.moveDir;
        }
    }

    private void FixedUpdate()
    {
        if (moveDir != Vector3.zero && canMove)
        {
            Movement();
        }
    }

    void Movement()
    {
        Vector3 sideForward = rb.velocity - Vector3.up * rb.velocity.y;

        rb.AddForce(moveDir.normalized * airForce, ForceMode.Acceleration);

        if (sideForward.magnitude > speedCap)
        {
            rb.velocity = Vector3.ClampMagnitude(sideForward, speedCap) + new Vector3(0, rb.velocity.y, 0);
        }
    }
}
