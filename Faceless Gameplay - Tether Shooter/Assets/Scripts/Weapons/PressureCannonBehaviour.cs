using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PressureCannonBehaviour : MonoBehaviour
{
    public PlayerControls pcs;

    public InputAction shoot;

    public Text text;
    public MenuBehaviour mb;
    public Rigidbody rb;
    public Camera cam;
    public Transform gun;

    public bool fire;

    public float chargeRate;
    public float maxCharge;
    public float charge;
    public float minSpeed;
    public float maxSpeed;
    public float maxForce;
    public float radius;
    public float correctingUnity;

    public GameObject projectile;

    public float maxDamage;

    public float fireRate;

    public string enemyTeam;

    // Start is called before the first frame update
    void Awake()
    {
        pcs = InputManager.pcs;

        shoot = pcs.Gameplay.Shoot;

        shoot.performed += OnShoot;
        shoot.canceled += OnShoot;

        mb = GetComponent<MenuBehaviour>();
        cam = GetComponentInChildren<Camera>();

        StartCoroutine(Shooting());

        text.text = "0";

        if (tag == "Team1")
        {
            enemyTeam = "Team2";
            gameObject.layer = 7;
        }
        else
        {
            enemyTeam = "Team1";
            gameObject.layer = 8;
        }
    }

    private void OnEnable()
    {
        shoot.Enable();
        shoot.performed += OnShoot;
        shoot.canceled += OnShoot;
    }

    private void OnDisable()
    {
        shoot.performed -= OnShoot;
        shoot.canceled -= OnShoot;
        shoot.Disable();
    }

    private void FixedUpdate()
    {
        if (fire && charge != maxCharge && !mb.paused)
        {
            charge += Time.fixedDeltaTime * chargeRate;

            if (charge > maxCharge)
            {
                charge = maxCharge;
            }

            text.text = charge.ToString("F0");
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            fire = true;
        }
        else if (context.canceled)
        {
            fire = false;
        }
    }

    IEnumerator Shooting()
    {
        while (true)
        {
            if (!fire && charge > 0 && !mb.paused)
            {
                Fire(charge);

                charge = 0;
                fire = false;

                text.text = "0";
                yield return new WaitForSeconds(fireRate);
            }

            yield return null;
        }
    }

    void Fire(float force)
    {
        RaycastHit hit;
        
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, correctingUnity * (force / maxCharge)))
        {
            Explosion(hit.point, force);
        }
        else
        {
            PressureBehaviour pb = Instantiate(projectile, cam.transform.position, Quaternion.Euler(cam.transform.rotation.x, cam.transform.rotation.y, 0)).GetComponent<PressureBehaviour>();
            pb.initial = gameObject;
            pb.force = maxForce * (force / maxCharge);
            pb.damage = maxDamage * (force / maxCharge);
            pb.target = enemyTeam;
            pb.radius = radius;
            pb.gameObject.layer = gameObject.layer;
            Rigidbody rb_ = pb.gameObject.GetComponent<Rigidbody>();
            rb_.velocity = cam.transform.forward * (minSpeed + ((maxSpeed - minSpeed) * (force / maxCharge)));
        }
    }

    void Explosion(Vector3 point, float force)
    {
        Collider[] overlap = Physics.OverlapSphere(transform.position, radius);

        for (int x = 0; x < overlap.Length; x++)
        {
            if (!overlap[x].gameObject.GetComponent<MeshCollider>() || overlap[x].gameObject.GetComponent<MeshCollider>().convex)
            {
                float distance = Vector3.Distance(overlap[x].ClosestPoint(transform.position), point);

                if (overlap[x].gameObject.GetComponent<Rigidbody>())
                {
                    Vector3 dir = (overlap[x].gameObject.transform.position - point).normalized;
                    overlap[x].gameObject.GetComponent<Rigidbody>().AddForce(dir.normalized * maxForce * (force / maxCharge) * ((radius - distance) / radius), ForceMode.Impulse);
                }

                if (overlap[x].gameObject.CompareTag(enemyTeam))
                {
                    if (overlap[x].gameObject.GetComponent<JumpBehaviour>())
                    {
                        overlap[x].gameObject.GetComponent<JumpBehaviour>().ungroundDouble = false;
                    }

                    overlap[x].gameObject.GetComponent<CharacterBehaviour>().ReceiveHit((maxDamage * (force / maxCharge)) * ((radius - distance) / radius));
                }
                else if (overlap[x].gameObject == gameObject)
                {
                    overlap[x].gameObject.GetComponent<JumpBehaviour>().ungroundDouble = false;
                }
            }
        }
    }
}
