using UnityEngine;
using System.Collections;
using UnityEngine.AI;
 
public class Fire : MonoBehaviour {
 
 
    public GameObject gun;
    public Vector3 targetDirection;
    private Transform target;
    private NavMeshAgent agentF;
    private Vector3 wanderDirection; 
    private int radius = 10;
    private bool moving = false;
    private float timer;
    private Coroutine co;

    private Vector3 finalPosition; 
 
    // Use this for initialization
    void OnEnable () 
    {
        agentF = GetComponent<NavMeshAgent> ();
        agentF.updateRotation = false;
        agentF.stoppingDistance = 0.5f;
        finalPosition = this.transform.position;
        co = StartCoroutine(moveRandom());
    }

    void OnDisable() 
    {
        agentF.isStopped = false;
        moving = false;
        StopCoroutine(co);

        if (Random.Range(0,100) > 0)
        {
            StopCoroutine(co);
            this.gameObject.GetComponent<Seek>().enabled = true;
        }
        else
        {
            this.gameObject.GetComponent<Wander>().enabled = true;
        }
        
                
    }

 
    // Update is called once per frame
    void Update () 
    {   
        targetDirection = this.GetComponent<BotStateController>().aggroTargetObject.transform.position - this.transform.position;
        float singleStep = 5.0f * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(this.transform.forward, targetDirection , singleStep, 0.0f);
        this.transform.rotation = Quaternion.LookRotation(newDirection);
        if (Random.Range(0,100) > 90)
        {
            StartCoroutine(bulletNumber());
        }
        else
        {
            if (moving == false && Random.Range(0,100) > 98)
            {
                agentF.speed = Random.Range(5,8);
                Vector3 randomDirection = (Random.insideUnitSphere * radius);
                randomDirection += transform.position;
                NavMeshHit hit;
                finalPosition = Vector3.zero;
                if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) 
                {
                    finalPosition = hit.position;            
                }
                moving = true;
            }
        }
    }

    private IEnumerator bulletNumber()
    {
         for (var i = 0; i <  Random.Range(1, 2); i++)
            {
                yield return new WaitForSeconds (1 / gun.GetComponent<AutGun>().fireRate);
                gun.GetComponent<AutGun>().Fire();
            } 
        
    }

    private IEnumerator moveRandom()
    {
         while (true)
         {
                //bool updatePath = (Vector3.Distance(this.transform.position, finalPosition) > agent.stoppingDistance + 0.1f) ? false : true;
                
                if (Vector3.Distance(this.transform.position, finalPosition) > 1.5f)
                {
                    moving = true;
                    agentF.SetDestination(finalPosition);
                }
                else
                {  
                    moving = false;
                }
             
             yield return null;
         }
         
    }
    
}
 
