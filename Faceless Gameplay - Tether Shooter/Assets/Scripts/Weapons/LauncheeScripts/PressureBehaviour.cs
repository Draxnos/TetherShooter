using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject initial;

    public string target;
    public float damage;
    public float force;
    public float radius;
    public float minSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude < minSpeed)
        {
            rb.velocity = rb.velocity.normalized * minSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explosion(collision.GetContact(0).point);
    }

    void Explosion(Vector3 point)
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
                    overlap[x].gameObject.GetComponent<Rigidbody>().AddForce(dir * force * ((radius - distance) / radius), ForceMode.Impulse);
                }

                if (overlap[x].gameObject.CompareTag(target))
                {
                    if (overlap[x].gameObject.GetComponent<JumpBehaviour>())
                    {
                        overlap[x].gameObject.GetComponent<JumpBehaviour>().ungroundDouble = false;
                        overlap[x].gameObject.GetComponent<CollisionBehaviour>().frictionSlide = true;
                    }

                    overlap[x].gameObject.GetComponent<CharacterBehaviour>().ReceiveHit(damage * ((radius - distance) / radius));
                }
                else if (overlap[x].gameObject == initial)
                {
                    overlap[x].gameObject.GetComponent<JumpBehaviour>().ungroundDouble = false;
                }
            }
        }

        Destroy(gameObject);
    }
}
