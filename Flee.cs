using UnityEngine;
using System.Collections;
using UnityEngine.AI;
 
public class Flee : MonoBehaviour {
 
    public bool fled = false;
    private Transform target;
    private NavMeshAgent agent;
    private Vector3 direction;
    private Vector3 targetPoint;
    private float timer;
 
    // Use this for initialization
    void OnEnable () {
        fled = false;
        agent = GetComponent<NavMeshAgent> ();
        agent.stoppingDistance = 2 ;
        agent.speed = 5;

        direction = this.transform.position - this.GetComponent<BotStateController>().aggroTargetObject.transform.position;
        targetPoint = transform.position + direction * Random.Range(1,3);
    }

    // Update is called once per frame
    void Update () 
    {
        agent.destination = (targetPoint);
        agent.updateRotation = true;

        if (Vector3.Distance(this.transform.position, targetPoint) <= agent.stoppingDistance)
        {
            StartCoroutine(waitThenSwitch());
        }
    }

    private IEnumerator waitThenSwitch()
    {
        yield return new WaitForSeconds(Random.Range(2,4));
        fled = true;
        
    }

    
 
}   