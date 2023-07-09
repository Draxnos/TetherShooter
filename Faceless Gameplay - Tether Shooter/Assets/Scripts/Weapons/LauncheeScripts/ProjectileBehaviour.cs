using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public string enemyTeam;
    public Vector3 lastPos;
    public RaycastHit hit;
    public float damage;

    private void FixedUpdate()
    {
        if (lastPos != null)
        {
            if (Physics.Raycast(transform.position, lastPos, out hit, Vector3.Distance(transform.position, lastPos)))
            {
                Hit(hit.collider.gameObject);
            }
        }

        transform.position = lastPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hit(collision.collider.gameObject);
    }

    void Hit(GameObject target)
    {
        if (target.tag == enemyTeam)
        {
            target.GetComponent<CharacterBehaviour>().ReceiveHit(damage);
        }

        Destroy(gameObject);
    }
}
