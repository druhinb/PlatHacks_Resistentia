using UnityEngine;
using System.Collections;
using UnityEngine.AI;
 
public class Follow : MonoBehaviour {
 
    private Transform target;
    private NavMeshAgent agent;

    public GameObject Player;
    private float timer;
 
    // Use this for initialization
    void OnEnable () {
        agent = GetComponent<NavMeshAgent> ();
        agent.stoppingDistance = 7;
        agent.speed = 5;
    }

    // Update is called once per frame
    void Update () 
    {
        agent.destination = (Player.transform.position);
        agent.updateRotation = true;
            
    }

    

    
 
}   