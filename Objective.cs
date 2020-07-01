using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Objective : MonoBehaviour
{
    public AudioSource audioSource;
    private AudioClip clip;
    private bool isNearPlayer = false;
    private int counter = 0;
    public AudioClip[] voices;
    public GameObject player;
    public GameObject objective;
    public bool ObjectiveCompleted = false;
    Coroutine dist;
    // Start is called before the first frame update
    void Start()
    {
        clip = voices[0];
        dist  = StartCoroutine(checkForPlayerDist());


    }

    // Update is called once per frame
    void Update()
    {
        if (isNearPlayer == true)
        {
            print ("jude");
            StartCoroutine(playClip());
        }
    }

    private IEnumerator checkForPlayerDist()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            isNearPlayer = Vector3.Distance(this.transform.position, player.transform.position) < 5;
        }
    }

    private IEnumerator playClip()
    {
        isNearPlayer = false;
        StopCoroutine(dist);
        player.GetComponent<FirstPersonAIO>().playerCanMove = false;
        clip = voices[counter];
        audioSource.clip = clip;
        audioSource.Play();
        
        if (ObjectiveCompleted)
        {
            counter = voices.Length-1;
        }
        
        yield return new WaitForSeconds(clip.length);

        if (counter == 0)
        {
            objective.SetActive(true);
        }
        
        if (counter < voices.Length  - 2)
        {
            counter += 1;
        }
        
       
        player.GetComponent<FirstPersonAIO>().playerCanMove = true;
        yield return new WaitForSeconds(10);
        dist = StartCoroutine(checkForPlayerDist());

        
    }
}

