using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBehaviour : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public GameObject healthBarObj;
    public Slider healthBar;
    public GameObject canvas;

    public string player;
    public Transform players;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.maxValue = maxHealth;
        health = maxHealth;
        players = GameObject.Find("Player (Split)").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (health != maxHealth)
        {
            healthBarObj.SetActive(true);
            canvas.transform.LookAt(players);
        }
        else
        {
            healthBarObj.SetActive(false);
        }

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReceiveHit(float damage)
    {
        if (damage < health)
        {
            health -= damage;
        }
        else
        {
            health -= health;
        }

        healthBar.value = health;
    }
}
