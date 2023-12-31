using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public bool isPlayerInAttackRange;
    public NavMeshAgent agent;
    public float maxAngle = 45;
    public float maxDistance = 10;
    public float timer = 1.0f;
    public float visionCheckRate = 1.0f;
    public Transform[] points;
    private int destPoint = 0;
    private Animator animator;
    public LayerMask raycastLayer;

    public GameObject bottle;
    public bool stunned;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        bottle = GameObject.FindWithTag("PickUpLeftHandMetalBottle");
        GotoNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (stunned)
        {
            animator.SetBool("Stunned", true);
            StopMovement();
            Invoke("ResumeMovement", 4.0f);
            stunned = false;
            animator.SetBool("Stunned", false);
        }
        else
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);

            if (SeePlayer())
            {
                //animator.SetBool("Scream", false);
                agent.destination = player.transform.position;
                agent.speed = 5.0f;
                timer = 1.0f;
            }
            else if (SeeMetalBottle())
            {
                agent.destination = bottle.transform.position;
                agent.speed = 2.0f;
                timer = 1.0f;
            }
            else //Patrol
            {
                if (HasReachedDestination())
                {
                    //animator.SetBool("Scream", true);
                    timer -= Time.deltaTime;
                    if (timer <= 0.0f)
                    {
                        //animator.SetBool("Scream", false);
                        GotoNextPoint();
                        timer = 0.1f;
                    }
                }

            }
        }
    }

    void GotoNextPoint()
    {
        ResumeMovement();
        // Returns if no points have been set up
        if (points.Length == 0)
            return;
        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;
        points[destPoint].GetComponent<Renderer>().material.color = Color.red;
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }
    public void ChangeBottle()
    {
        bottle = null;
    }

    public bool SeeMetalBottle()
    {
        if (bottle == null) return false;
        Vector3 vecBottleTurret = bottle.transform.position - transform.position;
        if (vecBottleTurret.magnitude > maxDistance)
        {
            return false;
        }
        Vector3 normVecBottleTurret = Vector3.Normalize(vecBottleTurret);
        float dotProduct = Vector3.Dot(transform.forward, normVecBottleTurret);
        var angle = Mathf.Acos(dotProduct);
        float deg = angle * Mathf.Rad2Deg;
        if (deg < maxAngle)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, vecBottleTurret);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.tag == "PickUpLeftHandMetalBottle")
                {
                    Invoke("ChangeBottle", 5.0f);
                    return true;
                }
            }
        }
        return false;
    }

    public bool SeePlayer()
    {
        if (player == null) return false;
        Vector3 vecPlayerTurret = player.transform.position - transform.position;
        if (vecPlayerTurret.magnitude > maxDistance)
        {
            return false;
        }
        Vector3 normVecPlayerTurret = Vector3.Normalize(vecPlayerTurret);
        float dotProduct = Vector3.Dot(transform.forward, normVecPlayerTurret);
        var angle = Mathf.Acos(dotProduct);
        float deg = angle * Mathf.Rad2Deg;
        if (deg < maxAngle)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, vecPlayerTurret);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.tag == "Player")
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool HasReachedDestination()
    {

        return agent.remainingDistance <= agent.stoppingDistance;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            StopMovement();
            transform.LookAt(player.transform.position);
            isPlayerInAttackRange = true;
            animator.SetTrigger("Attack");
            SceneManager.LoadScene("EndScene");
            Invoke("ResumeMovement", 2.0f);
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            isPlayerInAttackRange = false;
            ResumeMovement();
        }
    }
    public void StopMovement()
    {
        agent.isStopped = true; // was agent.Stop();
        agent.velocity = Vector3.zero;
    }
    public void ResumeMovement()
    {
        agent.isStopped = false; // was agent.Stop();
        agent.speed = 2;
    }
    public void HitPlayer()
    {
        /**if (isPlayerInAttackRange)
         {
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
         }*/
    }
}
