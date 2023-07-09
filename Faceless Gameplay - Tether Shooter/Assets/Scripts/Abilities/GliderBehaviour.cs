using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GliderBehaviour : MonoBehaviour
{
    private PlayerControls pcs;

    private InputAction look;
    private InputAction jump;

    public GameObject cam;
    public CollisionBehaviour cb;
    public Rigidbody rb;

    public Vector3 lookDir;

    public float glideRatio;

    public bool glide = false;

    private void Awake()
    {
        pcs = InputManager.pcs;

        look = pcs.Gameplay.Look;
        jump = pcs.Gameplay.Jump;

        rb = GetComponent<Rigidbody>();
        cb = GetComponent<CollisionBehaviour>();
        cam = Camera.main.gameObject;
    }

    private void OnEnable()
    {
        jump.performed += _ => glide = true;
        jump.canceled += _ => glide = false;
    }

    private void OnDisable()
    {
        jump.performed -= _ => glide = true;
        jump.canceled -= _ => glide = false;
    }

    private void FixedUpdate()
    {
        if (!cb.grounded && glide)
        {
            float forwardAngle = Vector3.Angle(cam.transform.forward, Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up));
            float scalar = glideRatio * ((90 - forwardAngle) / 90);

            rb.velocity = new Vector3(0, rb.velocity.y, 0) + transform.forward * new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

            rb.AddForce((cam.transform.up * cam.transform.InverseTransformDirection(-rb.velocity).y) + cam.transform.forward * -rb.velocity.y, ForceMode.Acceleration);
        }
    }
}
