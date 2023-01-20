using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] team1;
    public GameObject[] team2;
    public Transform[] attackSpawn;
    public Transform[] defenseSpawn;
    public float time1 = 300f;
    public float time2 = 300f;

    public bool defense = false;
    public int round = 1;

    public bool capturing = false;
    public bool contested = false;
    public float capture = 0;
    public float speedPerPlayer;
    public int players = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (defense == false)
        {
            if (other.tag == "Team1")
            {
                capturing = true;
                players += 1;
            }


            if (other.tag == "Team2")
            {
                contested = true;
            }
        }
        else
        {
            if (other.tag == "Team1")
            {
                capturing = true;
                players += 1;
            }


            if (other.tag == "Team2")
            {
                contested = true;
            }
        }
    }
}
