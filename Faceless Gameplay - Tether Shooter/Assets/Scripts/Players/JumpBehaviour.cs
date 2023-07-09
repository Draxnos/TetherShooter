using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpBehaviour : MonoBehaviour
{
    public PlayerControls pcs;

    public InputAction jump;

    public Rigidbody rb;
    public TetherBehaviour tb;
    public CollisionBehaviour cb;
    public MenuBehaviour mb;

    public bool ungroundDouble = true;

    /// <summary>
    /// Jump shit
    /// </summary>
    public float jumpForce = 200f;
    public float minAngle = 10f;

    private void Awake()
    {
        pcs = InputManager.pcs;

        jump = pcs.Gameplay.Jump;
        jump.performed += Jump;

        rb = GetComponent<Rigidbody>();
        tb = GetComponent<TetherBehaviour>();
        cb = GetComponent<CollisionBehaviour>();
        mb = GetComponent<MenuBehaviour>();
    }

    private void OnEnable()
    {
        jump.Enable();
        jump.performed += Jump;
    }

    private void OnDisable()
    {
        jump.performed -= Jump;
        jump.Disable();
    }

    /// <summary>
    /// Used to jump or jump off walls (when possible)
    /// 
    /// Should be considered incomplete, there must be a better way to wall hop
    /// </summary>
    public void Jump(InputAction.CallbackContext context)
    {
        ungroundDouble = false;

        if (cb.grounded == true)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        //Wallhop
        else if (cb.wallHop == true)
        {
            Quaternion correction = Quaternion.identity;
            
            Vector3 left = Quaternion.AngleAxis(minAngle, Vector3.up) * cb.perpLeft;
            Vector3 right = Quaternion.AngleAxis(-minAngle, Vector3.up) * cb.perpRight;
            float angleSize = Vector3.Angle(left, right);

            float angleLeft = Vector3.Angle(transform.forward, left);
            float angleRight = Vector3.Angle(right, transform.forward);
            float angleForward = Vector3.Angle(transform.forward, cb.wallNormal);
            float angleTLeft = Vector3.Angle(cb.perpLeft, transform.forward);
            float angleTRight = Vector3.Angle(transform.forward, cb.perpRight);

            Vector3 hopDir = Quaternion.AngleAxis(45, -transform.right) * transform.forward;

            //Angle depth correction
            if (angleForward > angleSize / 2)
            {
                if (angleTLeft < 2 * minAngle)
                {
                    correction = Quaternion.AngleAxis(angleLeft, Vector3.up);
                }
                else if (angleTRight < 2 * minAngle)
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
        else if (tb && tb.tetherJump == true)
        {
            rb.AddForce((tb.cj.connectedAnchor - transform.position).normalized * jumpForce, ForceMode.Impulse);
        }
    }
}
