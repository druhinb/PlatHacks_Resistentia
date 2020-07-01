using UnityEngine;
using System.Collections;
using UnityEngine.AI;
 
public class Seek : MonoBehaviour {
 
    private Transform target;
    private NavMeshAgent agentS;
    public bool switchTime = false;
    public GameObject cube;

    [HideInInspector] 
    public Vector3 aggroTarget;
    private float timer;
 
    // Use this for initialization
    void OnEnable () {
        agentS = GetComponent<NavMeshAgent> ();
        agentS.isStopped = false;
        agentS.stoppingDistance = 0.3f;
        agentS.updateRotation = true;
        agentS.speed = 5;
        switchTime = false;
    }
 
    // Update is called once per frame
    void Update () 
    {
        agentS.SetDestination(aggroTarget);

        if (Vector3.Distance(this.transform.position, aggroTarget) < 2)
            {
                StartCoroutine(waitThenSwitch());
            }

    }

    private IEnumerator waitThenSwitch()
    {
        yield return new WaitForSeconds(Random.Range(2,4));
        switchTime = true;
    }

    
}   