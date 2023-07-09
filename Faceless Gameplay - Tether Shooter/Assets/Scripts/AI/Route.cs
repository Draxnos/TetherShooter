using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    public GameObject[] route;

    public Route()
    {
        route = new GameObject[0];
    }

    public Route(GameObject[] markers)
    {
        route = markers;
    }
}
