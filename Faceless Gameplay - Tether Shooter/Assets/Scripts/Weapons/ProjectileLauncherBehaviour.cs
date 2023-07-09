using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ProjectileLauncherBehaviour : MonoBehaviour
{
    public PlayerControls pcs;

    public InputAction shoot;
    public InputAction reload;

    public MenuBehaviour mb;
    public Camera cam;
    public Text clipHUD;
    public GameObject projectile;

    public float projectileForce;
    public float maxProjectileForce;

    public int fireType;
    public int reloadType;

    public int burstCount;
    public float burstRate;
    public int burst;

    public float chargeRate;
    public float maxCharge;
    public float charge;

    public bool fire = false;
    public bool reloading = false;

    public int maxAmmo;
    public int ammo;

    public float fireRate;
    public float reloadRate;

    public float damage;
    public float maxDamage;

    public string enemyTeam;

    public Text mode;

    // Start is called before the first frame update
    private void Awake()
    {
        pcs = InputManager.pcs;

        shoot = pcs.Gameplay.Shoot;
        reload = pcs.Gameplay.Reload;

        shoot.performed += OnShoot;
        shoot.canceled += OnShoot;
        reload.performed += OnReload;

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

    private void OnEnable()
    {
        shoot.Enable();
        reload.Enable();
        reload.performed += OnReload;
        shoot.performed += OnShoot;
        shoot.canceled += OnShoot;
    }

    private void OnDisable()
    {
        reload.performed -= OnReload;
        shoot.performed -= OnShoot;
        shoot.canceled -= OnShoot;
        shoot.Disable();
        reload.Disable();
    }

    private void FixedUpdate()
    {
        if (fireType == 3 && fire && charge != maxCharge)
        {
            
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            fire = true;
            
            if (ammo > 0 && reloading)
            {
                reloading = false;
            }
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
            while (!mb.paused && ammo > 0)
            {
                switch (fireType)
                {
                    //Semi
                    case 0:
                        if (fire)
                        {
                            Fire(maxProjectileForce);
                            fire = false;

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;

                    //Auto
                    case 1:

                        while (fire)
                        {
                            Fire(maxProjectileForce);

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;

                    //Burst
                    case 2:
                        if (fire)
                        {
                            while (burst < burstCount && ammo > 0)
                            {
                                Fire(maxProjectileForce);

                                burst++;

                                yield return new WaitForSeconds(burstRate);
                            }

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;

                    //Charge
                    case 3:
                        while (fire && charge != maxCharge)
                        {
                            charge += Time.fixedDeltaTime * chargeRate;

                            if (charge > maxCharge)
                            {
                                charge = maxCharge;
                            }

                            yield return null;
                        }

                        if (!fire && charge > 0)
                        {
                            Fire(charge);

                            charge = 0;

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;
                }
            }

            yield return null;
        }
    }

    void Fire(float force)
    {
        ammo--;
        clipHUD.text = ammo + " / " + maxAmmo;

        Rigidbody rb_ = Instantiate(projectile, cam.transform.position, Quaternion.Euler(cam.transform.rotation.x, cam.transform.rotation.y, 0)).GetComponent<Rigidbody>();
        rb_.AddForce(cam.transform.forward * force);
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed && ammo < maxAmmo)
        {
            if (!reloading)
            {
                reloading = true;
            }
            else
            {
                reloading = false;
            }
        }
    }

    IEnumerator Reload()
    {
        while (true)
        {
            if ((!mb.paused && ammo < maxAmmo) || ammo == 0f)
            {
                switch (reloadType)
                {
                    //Clip
                    case 0:
                        if (reloading)
                        {
                            ammo = 0;

                            yield return new WaitForSeconds(reloadRate);

                            ammo = maxAmmo;
                            clipHUD.text = ammo + " / " + maxAmmo;
                        }
                        
                        break;

                    //Per Shot
                    case 1:

                        while (reloading && ammo < maxAmmo)
                        {
                            yield return new WaitForSeconds(reloadRate);

                            ammo++;
                            clipHUD.text = ammo + " / " + maxAmmo;
                        }

                        break;
                }
            }

            yield return null;
        }
    }

    public void SwitchFire()
    {
        fireType++;

        switch (fireType)
        {
            default:
                fireType = 0;
                mode.text = "Semi";

                break;
            case 1:
                mode.text = "Auto";

                break;
            case 2:
                mode.text = "Burst";

                break;
            case 3:
                mode.text = "Charge";

                break;
        }
    }
}
