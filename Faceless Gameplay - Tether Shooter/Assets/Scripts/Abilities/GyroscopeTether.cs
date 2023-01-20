using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopeTether : MonoBehaviour
{
    public KeyCode sendTether = KeyCode.E;
    public KeyCode cutTether = KeyCode.C;
    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public float sensitivity = 5f;

    public Rigidbody rb;

    public Transform center;

    /// <summary>
    /// Armature transforms, Rotate their Y axis to align with the gyroscope joints
    /// </summary>
    public Transform centerRing;
    public Transform middleRing;

    public Transform topPin;
    public Transform bottomPin;

    public Vector3 baseUp;
    public Vector3 centerUp;

    public Vector3 moveNormal; //Aka tether dir from gyro perspective

    public int vert;
    public int hori;
    public Vector3 moveDir; //Parallel to tether, maybe spinning top mode?
    public float maxSpeed; 
    public float moveForce;

    public bool tether1;
    public bool tether2;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        vert = (Input.GetKey(forward) ? 1 : 0) * 1 + (Input.GetKey(back) ? 1 : 0) * -1;
        hori = (Input.GetKey(right) ? 1 : 0) * 1 + (Input.GetKey(left) ? 1 : 0) * -1;

        moveDir = (transform.right * hori + transform.forward * vert).normalized;
        LookMovement();
        BodyMovement();

        if (Input.GetKey(sendTether))
        {
            SendTether();
        }

        if (Input.GetKey(cutTether))
        {
            CutTether();
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {

    }

    void BodyMovement()
    {
        
    }

    void LookMovement()
    {
        centerRing.Rotate(Vector3.right * Input.GetAxis("Mouse X") * sensitivity * 50f * Time.deltaTime);
        center.Rotate(Vector3.up * Input.GetAxis("Mouse Y") * sensitivity * 50f * Time.deltaTime);
    }

    void SendTether()
    {
        
    }

    void CutTether()
    {

    }
}
