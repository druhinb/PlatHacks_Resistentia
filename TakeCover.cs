using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

 
public class TakeCover : MonoBehaviour {
 
    public float coverEfficiency = 0.5f;
    public bool atCover = false;
    private Transform target;
    private NavMeshAgent agent;
    List<Vector3> coverPositions = new List<Vector3>();
    List<float> coverDistances = new List<float>();
    private Vector3 targetCover;
    private float timer;
 
    // Use this for initialization
    void OnEnable () 
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1;
        for (var i = 0; i < 50; i++)
        {
            Vector3 nearUnit = this.transform.position + Random.insideUnitSphere * 20;
            NavMeshHit hit;
            if (NavMesh.FindClosestEdge(nearUnit, out hit, NavMesh.AllAreas))
            {
                if (Vector3.Dot(hit.normal, (this.GetComponent<BotStateController>().aggroTargetObject.transform.position - this.transform.position)) < coverEfficiency)
                {
                    coverPositions.Add(hit.position);
                    coverDistances.Add(Vector3.Distance(this.transform.position, hit.position));
                }
            }
        }
        targetCover = coverPositions[coverDistances.IndexOf(coverDistances.Max())];
        Debug.DrawLine(this.transform.position, targetCover, Color.green, 200);
    }

    // Update is called once per frame
    void Update () 
    {
        agent.SetDestination(targetCover);

        if (Vector3.Distance(this.transform.position, targetCover) < 1.5)
            {
                print (atCover);
                atCover = true;
            }

            
    }

    
 
}   