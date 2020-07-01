using UnityEngine;
using System.Collections;
using UnityEngine.AI;
 
public class Wander : MonoBehaviour {
 
    public float wanderRadius = 5.0f;
    public float wanderTimer = 5.0f;
 
    private Transform target;
    private NavMeshAgent agentW;
    private float timer = 5.0F;
 
    // Use this for initialization
    void OnEnable () {
        print("hello there");
        agentW = GetComponent<NavMeshAgent> ();
        timer = wanderTimer;
        agentW.updateRotation = true;
    }
 
    // Update is called once per frame
    public void Update () {
        timer += Time.deltaTime;
 
        if (timer >= wanderTimer) {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agentW.SetDestination(newPos);
            timer = 0;
        }
    }
 
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;
 
        randDirection += origin;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
 
        return navHit.position;
    }
}