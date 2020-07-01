using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BotStateController : MonoBehaviour
{
    public GameObject player;
    public GameObject bot;
    public bool isHighRank = true;
    public float botHealth = 100;
    
    public bool isAllyOfPlayer = false;
    public GameObject aggroTargetObject = null;
    private string [] AITargets = new string[2];
    private BotState _currentState = BotState.Idle;
    private float lastHit;
    private float timeSinceHit;
    private int toggle = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(isAllyOfPlayer)
        {
            AITargets[0] = "EnemyBot";
        }
        else if (!isAllyOfPlayer)
        {
            AITargets[0] = "FriendlyBot"; AITargets[1] = "Player";
        }
        Coroutine co = StartCoroutine(CheckForTarget());
    }

    // Update is called once per frame
    void Update()
    {
        RegenHealth();

        if (botHealth == 0)
        {
            StartCoroutine(WaitThenDie());
        }

        switch (_currentState)
        {
            case BotState.Idle:
            {
                if (aggroTargetObject != null)
                {
                    _currentState = BotState.AttackStates;
                }
                else
                {
                    _currentState = BotState.Wander;
                }
                break;
            }
            case BotState.Wander:
            {
                this.gameObject.GetComponent<Wander>().enabled = true;
                if (aggroTargetObject != null)
                {
                    bot.gameObject.GetComponent<Wander>().enabled = false;
                    _currentState = BotState.Fire;
                }
                break;
            }

            case BotState.Patrol:
            {
                this.gameObject.GetComponent<Patrol>().enabled = true;
                if (aggroTargetObject != null)
                {
                    this.gameObject.GetComponent<Patrol>().enabled = false;
                    _currentState = BotState.Fire;
                }
                break;
            }

            case BotState.Follow:
            {
                break;
            }
            case BotState.AttackStates:
            {
                if (aggroTargetObject != null)
                {
                    _currentState = BotState.Fire;
                    bot.gameObject.GetComponent<Fire>().enabled = true;
                }
                break;
            }

                case BotState.Fire:
                {
                    bot.gameObject.GetComponent<Fire>().enabled = true;
                    if (aggroTargetObject == null)
                    {
                        this.gameObject.GetComponent<Fire>().enabled = false;

                        if (Random.Range(0,100) > 80)
                        {
                            _currentState = BotState.Seek;
                            this.gameObject.GetComponent<Seek>().enabled = true;
                        }
                        else
                        {
                            _currentState = BotState.Wander;
                            this.gameObject.GetComponent<Wander>().enabled = true;
                        }
                    }
                    else if (botHealth < 30)
                    {
                        this.gameObject.GetComponent<Fire>().enabled = false;
                        _currentState = BotState.SelfPreservationStates;
                    }
                    break;
                }

                case BotState.Seek:
                {
                    bot.gameObject.GetComponent<Seek>().enabled = true;
                    if (bot.gameObject.GetComponent<Seek>().switchTime)
                    {
                        bot.gameObject.GetComponent<Seek>().enabled = false;
                        _currentState = BotState.Wander;
                        bot.gameObject.GetComponent<Wander>().enabled = true;
                        
                    }
                    break;
                }
            case BotState.SelfPreservationStates:
            {
                if (isHighRank)
                {
                    _currentState = BotState.TakeCover;
                }
                else
                {
                    _currentState = BotState.Flee;
                }
                break;
            }
                case BotState.TakeCover:
                {
                    bot.gameObject.GetComponent<TakeCover>().enabled = true;
                    if (bot.gameObject.GetComponent<TakeCover>().atCover == true)
                    {
                        bot.gameObject.GetComponent<TakeCover>().enabled = false;
                        _currentState = BotState.Idle;
                    }
                    break;
                }

                case BotState.Flee:
                {
                    bot.gameObject.GetComponent<Flee>().enabled = true;
                    if (bot.gameObject.GetComponent<Flee>().fled == true)
                    {
                        bot.gameObject.GetComponent<Flee>().enabled = false;
                        _currentState = BotState.Idle;
                    }
                    break;
                }

        }

    }
    public enum BotState
    {
        Idle,
        Wander,
        Patrol,
        Follow,
        AttackStates,
        Fire,
        Seek,
        SelfPreservationStates,
        TakeCover,
        Flee
    }

    void FollowCarryout()
    {
            if(Input.GetKeyDown(KeyCode.E) && toggle == 0)
            {
                bot.gameObject.GetComponent<Follow>().enabled = true;
                toggle++;
            }
            else if (Input.GetKeyDown(KeyCode.E) && toggle == 1)
            {
                bot.gameObject.GetComponent<Follow>().enabled = false;
                toggle = 0;
            }
            
    }

    public void OnGetHit(int bulletDamage)
    {
        lastHit = Time.time;
        if (botHealth > 0)
        {
            botHealth -= 10;
        }
    }

    private void RegenHealth()
    {
        if (Time.time - lastHit > 3.0f)
        {
            if (botHealth < 100)
            {
                botHealth += 0.25f;
            }
        }
    }
    

    private IEnumerator CheckForTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            
            Collider[] hitColliders = Physics.OverlapSphere(bot.transform.position, 55);
            List<float> targetValues = new List<float>();
            GameObject tempObj;
            RaycastHit hit;

            //If nothing nearby, then there is no possible targets
            if (hitColliders.Length == 0)
            {
                aggroTargetObject = null;
            }
            else
            {
                //Calculate the "value" for each target
                foreach (Collider collider in hitColliders)
                {
                    if (AITargets.Contains(collider.gameObject.tag))
                    {
                        float tempScore = 0;
                        tempScore += (-(Vector3.Distance(collider.gameObject.transform.position, this.transform.position)));
                        targetValues.Add(tempScore);
                    }
                    else
                    {
                        targetValues.Add(Mathf.NegativeInfinity);
                    }
                }

                //Select the highest target to check if it is within line of sight
                tempObj = hitColliders[targetValues.IndexOf(targetValues.Max())].gameObject;
                Debug.DrawRay(this.transform.position, (tempObj.transform.position - this.transform.position), Color.green, 1);
                if (Physics.Raycast(bot.transform.position, (tempObj.transform.position - this.transform.position), out hit, 100))
                {
                    
                    if(AITargets.Contains(hit.collider.tag))
                    {
                        aggroTargetObject = tempObj;
                    }
                    else
                    {
                        aggroTargetObject = null;
                    }
                }
            }
            
        }

    }

        private IEnumerator WaitThenDie()
        {
            this.gameObject.SetActive(false);
            yield return null;
        }
    }




