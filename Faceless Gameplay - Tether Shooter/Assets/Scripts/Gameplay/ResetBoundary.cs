using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBoundary : MonoBehaviour
{
    public string pl = "Team1";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(pl))
        {
            other.gameObject.GetComponent<MovementBehaviour>().ResetPos();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(pl))
        {
            collision.gameObject.GetComponent<MovementBehaviour>().ResetPos();
        }
    }
}
