using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileLauncherBehaviour : MonoBehaviour
{
    public MenuBehaviour mb;
    public Camera cam;
    public TrailRenderer bt;
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

    public int maxAmmo;
    public int ammo;

    public float fireRate;
    public float reloadRate;

    public float damage;
    public float maxDamage;

    public string enemyTeam;

    public Text mode;

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
                        if (Input.GetKeyDown(mb.keys[11]))
                        {
                            Fire(maxProjectileForce);

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;

                    //Auto
                    case 1:

                        while (Input.GetKey(mb.keys[11]))
                        {
                            Fire(maxProjectileForce);

                            yield return new WaitForSeconds(fireRate);
                        }

                        break;

                    //Burst
                    case 2:
                        if (Input.GetKeyDown(mb.keys[11]))
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
                        while (Input.GetKey(mb.keys[11]) && charge < maxCharge)
                        {
                            charge += Time.deltaTime * chargeRate;

                            if (charge > maxCharge)
                            {
                                charge = maxCharge;
                            }
                        }

                        if (Input.GetKeyUp(mb.keys[11]))
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
                        if (Input.GetKeyDown(mb.keys[12]))
                        {
                            ammo = 0;

                            yield return new WaitForSeconds(reloadRate);

                            ammo = maxAmmo;
                            clipHUD.text = ammo + " / " + maxAmmo;
                        }
                        
                        break;

                    //Per Shot
                    case 1:

                        if (Input.GetKey(mb.keys[12]))
                        {
                            while (ammo < maxAmmo)
                            {
                                yield return new WaitForSeconds(reloadRate);

                                ammo++;
                                clipHUD.text = ammo + " / " + maxAmmo;
                            }
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
