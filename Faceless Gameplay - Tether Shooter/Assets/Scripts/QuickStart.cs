using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStart : MonoBehaviour
{
    public GameObject gc;

    // Start is called before the first frame update
    void Awake()
    {
        Instantiate(gc);
    }
}
