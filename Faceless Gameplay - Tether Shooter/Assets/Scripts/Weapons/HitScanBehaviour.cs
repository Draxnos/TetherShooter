using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HitScanBehaviour : MonoBehaviour
{
    public PlayerControls pcs;

    public InputAction shoot;

    public MenuBehaviour mb;
    public Camera cam;
    public TrailRenderer bt;
    public Text clipHUD;

    public int fireType;
    public int burstCount;
    public float burstRate;
    public int burst;
    public int maxAmmo;
    public int ammo;
    public float fireRate;
    public float reloadRate;
    public float damage;
    public bool fire;

    public Vector3 tracerStart;

    public string enemyTeam;

    public Text mode;

    private void Awake()
    {
        pcs = InputManager.pcs;

        shoot = pcs.Gameplay.Shoot;

        shoot.performed += OnShoot;
        shoot.canceled += OnShoot;
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

    // Start is called before the first frame update
    void Start()
    {
        mb = GetComponent<MenuBehaviour>();
        cam = GetComponentInChildren<Camera>();

        ammo = maxAmmo;
        clipHUD.text = ammo + " / " + maxAmmo;

        StartCoroutine(Shooting());
        StartCoroutine(Reload());

        if (tag == "Team1")
        {
            enemyTeam = "Team2";
        }
        else
        {
            enemyTeam = "Team1";
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
            if (ammo > 0 && !mb.paused)
            {
                switch (fireType)
                {
                    //Semi
                    case 0:
                        if (fire)
                        {
                            Fire();
                            fire = false;

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;
                    //Auto
                    case 1:

                        while (fire && ammo > 0)
                        {
                            Fire();

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;
                    //Burst
                    case 2:
                        if (fire)
                        {
                            while (burst < burstCount && ammo > 0)
                            {
                                Fire();

                                burst++;

                                yield return new WaitForSeconds(burstRate);
                            }

                            burst = 0;

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;
                }
            }

            yield return null;
        }
    }

    void Fire()
    {
        ammo--;
        clipHUD.text = ammo + " / " + maxAmmo;

        RaycastHit hit;

        TrailRenderer trail = Instantiate(bt, transform.position, Quaternion.identity);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            StartCoroutine(Tracer(trail, hit.point));

            if (hit.collider.tag == enemyTeam)
            {
                hit.collider.GetComponent<CharacterBehaviour>().ReceiveHit(damage);
                hit.collider.GetComponent<CharacterBehaviour>().player = cam.transform;
            }
        }
        else
        {
            StartCoroutine(Tracer(trail, cam.transform.position + cam.transform.forward * 100));
        }
    }

    IEnumerator Reload()
    {
        while (true)
        {
            if ((Input.GetKeyDown(mb.keys[12]) && ammo < maxAmmo) || ammo == 0f)
            {
                ammo = 0;

                yield return new WaitForSeconds(reloadRate);

                ammo = maxAmmo;
                clipHUD.text = ammo + " / " + maxAmmo;
            }

            yield return null;
        }
    }

    IEnumerator Tracer(TrailRenderer trail, Vector3 hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit, time);

            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = Vector3.Lerp(startPosition, hit, time);

        Destroy(trail.gameObject, trail.time);
    }

    public void SwitchFire()
    {
        fireType++;

        switch (fireType)
        {
            case 1:
                mode.text = "Auto";

                break;
            case 2:
                mode.text = "Burst";

                break;
            case 3:
                mode.text = "Semi";
                fireType = 0;

                break;
        }
    }
}
