using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PatrollerBehaviour : MonoBehaviour
{
    private NavMeshAgent nma;
    private GameController gc;
    private GameObject player;
    private GameObject eyes;
    private PatrollerManager pm;
    private CharacterBehaviour cb;
    private MovementBehaviour mb;

    public Route patrolLoop;
    private int loopPos;
    public float[] detectRange;
    public float precision;
    public float sightRange;
    public float detectAngle;

    public bool canShoot = false;
    public float shootingRange;
    private bool distracted = false;
    private bool playerSeen;
    private bool playerHeard;
    private Vector3 checkSpot = new Vector3(0, -100, 0);
    public bool aggression;
    private Vector3 playerPos;
    private bool startPatrol;

    public float catchChance;
    public float dayMultiplier;

    public List<GameObject> beenThere;
    public float wanderChance;
    public float wanderDetectRange;
    public float wanderRadius;
    public float wanderTime;
    public bool canWander;
    private bool wandered;

    public bool isStunned;
    public float stunTime;
    private Vector3 lastPos;
    public bool locked;
    public float lockedRange;

    public float maxAttackRange;

    // Start is called before the first frame update
    void Awake()
    {
        nma = GetComponent<NavMeshAgent>();
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        player = GameObject.Find("Player");
        cb = GetComponent<CharacterBehaviour>();
        eyes = transform.GetChild(0).gameObject;
        pm = GameObject.Find("PatrollerManager").GetComponent<PatrollerManager>();
        mb = GameObject.Find("Player").GetComponent<MovementBehaviour>();

        if (startPatrol)
        {
            PatrolStart();
        }
    }

    private void Update()
    {
        if (startPatrol)
        {
            PatrolStart();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isStunned && !locked)
        {
            ListenForPlayer();
            TheEyes();

            if (playerHeard && playerSeen && !aggression)
            {
                WokeUpAndChoseAnger();
            }

            if (aggression == true && Vector3.Distance(player.transform.position, gameObject.transform.position) < detectRange[mb.crouch ? 1 : 0] && playerSeen)
            {
                FightingWords();
            }
            else
            {
                PatrolLoop();
            }

            Wander();
        }
        else if (locked)
        {
            Collider[] arr = Physics.OverlapSphere(transform.position, lockedRange, LayerMask.GetMask("Player"));

            if (arr.Length == 0)
            {
                locked = false;
            }
        }
    }

    void TheEyes()
    {
        RaycastHit hit;

        if (Vector3.Angle(player.transform.position - eyes.transform.position, eyes.transform.forward) <= detectAngle && 
            Physics.Raycast(eyes.transform.position, player.transform.position - eyes.transform.position, out hit, sightRange) && hit.collider.gameObject == player)
        {
            playerSeen = true;

            if (pm.bloodSucking)
            {
                WokeUpAndChoseAnger();
            }
        }
        else
        {
            playerSeen = false;
        }
    }

    void FightingWords()
    {
        transform.LookAt(transform.position - Vector3.ProjectOnPlane(transform.position - player.transform.position, Vector3.up));

        if (Vector3.Distance(player.transform.position, transform.position) > .5f)
        {
            nma.destination = player.transform.position;
        }
    }

    void PatrolLoop()
    {
        if (Vector3.Distance(nma.destination, transform.position) <= 1.2f)
        {
            if (!wandered)
            {
                playerHeard = false;
                loopPos++;
                canWander = true;

                if (loopPos >= patrolLoop.route.Length)
                {
                    loopPos = 0;
                }
            }
            else
            {
                wandered = false;
            }

            nma.destination = patrolLoop.route[loopPos].transform.position;
        }
    }

    public void StartPatrol()
    {
        startPatrol = true;

        StartCoroutine("Wander");
    }

    void PatrolStart()
    {
        nma.destination = patrolLoop.route[0].transform.position;
        startPatrol = false;
    }

    void ListenForPlayer()
    {
        Collider[] arr = Physics.OverlapSphere(transform.position, detectRange[mb.crouch ? 1 : 0], LayerMask.GetMask("Player"));

        for (int x = 0; x < arr.Length; x++)
        {
            if (arr[x].gameObject == player)
            {
                if (aggression && mb.moveDir != Vector3.zero)
                {
                    playerHeard = true;
                    checkSpot = player.transform.position + Random.insideUnitSphere * precision;
                }
            }
            else
            {
                playerHeard = false;
            }
        }

        Collider[] arr_ = Physics.OverlapSphere(transform.position, detectRange[0], LayerMask.GetMask("Player"));

        for (int x = 0; x < arr_.Length; x++)
        {
            if (arr_[x].gameObject == player)
            {
                if (pm.bloodSucking)
                {
                    playerHeard = true;
                    checkSpot = player.transform.position + Random.insideUnitSphere * precision;
                }
            }
            else
            {
                playerHeard = false;
            }
        }

        if (arr.Length == 0 && arr_.Length == 0)
        {
            playerHeard = false;
        }
    }

    IEnumerator Wander()
    {
        while (true)
        {
            if (canWander && !playerHeard && !playerSeen)
            {
                if (Random.Range(1f, 100f) < wanderChance)
                {
                    Collider[] arr = Physics.OverlapSphere(transform.position, wanderDetectRange, LayerMask.GetMask("WanderPoints"));

                    if (arr.Length > 0)
                    {
                        Vector3 destination = (Random.insideUnitSphere * wanderRadius) + arr[0].gameObject.transform.position;
                        NavMeshHit hit;
                        NavMesh.SamplePosition(destination, out hit, wanderRadius, 1);
                        nma.destination = hit.position;
                        canWander = false;
                        wandered = true;
                    }
                }
            }

            yield return new WaitForSeconds(wanderTime);
        }
    }

    public void WokeUpAndChoseAnger()
    {
        aggression = true;

        StartCoroutine("Anger");
    }

    IEnumerator Anger()
    {
        while (aggression)
        {
            if (canShoot)
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= detectRange[mb.crouch ? 1 : 0])
                {
                    if (playerSeen)
                    {
                        //ab.attack[1] = true;
                    }

                    if (Vector3.Distance(player.transform.position, transform.position) <= shootingRange && playerSeen)
                    {
                        nma.isStopped = true;
                    }
                    else
                    {
                        nma.isStopped = false;
                    }
                }
                else
                {
                    nma.isStopped = false;
                    //ab.attack[1] = false;
                }
            }
            else
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= maxAttackRange)
                {
                    nma.isStopped = true;
                    //ab.attack[0] = true;
                }
                else
                {
                    nma.isStopped = false;
                    //ab.attack[0] = false;
                }
            }

            yield return null;
        }
    }
}
