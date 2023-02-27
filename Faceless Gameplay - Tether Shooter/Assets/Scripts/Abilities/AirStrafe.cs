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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mb = GetComponent<MenuBehaviour>();
        cb = GetComponent<CollisionBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mb.paused)
        {
            canMove = true;
            moveDir = mvb.moveDir;
        }
    }

    private void FixedUpdate()
    {
        if (moveDir != Vector3.zero && !mb.paused)
        {
            Movement();
        }
    }

    void Movement()
    {

    }
}
