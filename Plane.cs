using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{

    private Vector3 firstPos;
    public GameObject player;
    
    public Rigidbody rb;
    public AudioSource audioSource;
    public float maxVelocity = 5000f;

    // Start is called before the first frame update
    void Start()
    {
        firstPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 500)
        {
            audioSource.Play();
        }

        if (Vector3.Distance(firstPos, transform.position) < 2000)
        {
            if (rb.velocity.sqrMagnitude > maxVelocity)
            {
                 rb.velocity *= 0.999999f;
            }
            else
            {
                rb.AddRelativeForce(-3000,0,0);
            }
        }
        else
        {
           StartCoroutine(waitRespawn());
        }
    }

    private IEnumerator waitRespawn()
    {
        yield return new WaitForSeconds(10);
        
        this.transform.position = firstPos + new Vector3(Random.Range(-50, 50), Random.Range(-10,10), 0);
    }
}
