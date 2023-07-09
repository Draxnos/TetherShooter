using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerManager : MonoBehaviour
{
    public GameObject patrollerPrefab;

    public List<Route> patrolMarkers;
    public List<PatrollerBehaviour> patrollers;

    public bool anotherOne = false;
    public bool bloodSucking;

    private void Awake()
    {
        for (int x = 0; x < patrollers.Count; x++)
        {
            patrollers[x].patrolLoop = patrolMarkers[x];

            patrollers[x].StartPatrol();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (anotherOne)
        {
            PlusOne();

            anotherOne = false;
        }
    }

    void PlusOne()
    {
        patrollers.Add(Instantiate(patrollerPrefab, patrolMarkers[patrollers.Count - 1].route[0].transform.position, Quaternion.identity).GetComponent<PatrollerBehaviour>());
        patrollers[patrollers.Count - 1].patrolLoop = patrolMarkers[patrollers.Count - 1];
        patrollers[patrollers.Count - 1].StartPatrol();
    }

    void AddRoute(GameObject[] markers)
    {
        patrolMarkers.Add(new Route(markers));
    }

    public IEnumerator InvokeOneDown(int who)
    {
        yield return new WaitForSeconds(1);

        OneDown(who);
    }

    public void OneDown(int who)
    {
        patrollers.Insert(who, Instantiate(patrollerPrefab, patrolMarkers[who].route[0].transform.position, Quaternion.identity).GetComponent<PatrollerBehaviour>());
        patrollers[who].patrolLoop = patrolMarkers[patrollers.Count - 1];
        patrollers[who].StartPatrol();
    }
}
