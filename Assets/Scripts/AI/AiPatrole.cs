using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPatrole : MonoBehaviour
{
    public enum State
    {
        PATROLING,
        CHASING,
        WAITING,
        BACK_PATROLING
    }

    private State state = State.PATROLING;

    [SerializeField] List<Transform> patrolePoints;
    int closestPoint = 0;
    Rigidbody rigidbody;
    float speed = 10.0f;
    float backSpeed = 6.0f;
    float minimalDistance = 0.1f;

    [SerializeField] Transform target;

    [SerializeField] float detectionRadius = 2;
    float chasingRadius;
    [SerializeField] float chasingFactor = 1.5f;
    [SerializeField] float killigRadius = 0.5f;

    const float waitingTime = 4.0f;
    float currentWaitingTime = 0.0f;

    float customTime = 0.0f;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        FindClosestPoint();

        chasingRadius = detectionRadius * chasingFactor;
    }

    private void Update()
    {
        switch (state)
        {
            case State.PATROLING:
                if(Vector3.Distance(patrolePoints[closestPoint].position, transform.position) < minimalDistance)
                {
                    closestPoint++;
                    closestPoint = closestPoint % patrolePoints.Count;
                    
                }
                // Set velocity to the current point targeted
                rigidbody.velocity =
                        (patrolePoints[closestPoint].position - transform.position).normalized * speed;
                

                // Check if the target is close or not
                if(Vector3.Distance(transform.position, target.position) < detectionRadius)
                {
                    state = State.CHASING;
                }
                break;
            case State.CHASING:
                rigidbody.velocity =
                    (target.position - transform.position).normalized * speed;

                // Check if the target is out of range
                if (Vector3.Distance(transform.position, target.position) > chasingRadius)
                {
                    state = State.WAITING;
                }

                // Check if the target is close enough to be killed
                if (Vector3.Distance(transform.position, target.position) < killigRadius)
                {
                    Debug.Log("Kill");
                    FindClosestPoint();
                    state = State.BACK_PATROLING;
                }


                break;
            // if the target is out of range, the patroler will wait a certain amount of time before returning to his duty
            case State.WAITING:
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, Vector3.zero, 0.4f * Time.deltaTime);

                if (Vector3.Distance(transform.position, target.position) > chasingRadius)
                {
                    currentWaitingTime += Time.deltaTime;
                    if (currentWaitingTime > waitingTime)
                    {
                        currentWaitingTime = 0.0f;
                        FindClosestPoint();
                        state = State.BACK_PATROLING;
                    }
                }
                else
                {
                    currentWaitingTime = 0.0f;
                    state = State.CHASING;
                }
                break;
            case State.BACK_PATROLING:

                if (Vector3.SqrMagnitude(patrolePoints[closestPoint].position - transform.position) <
                    Mathf.Pow(1 + minimalDistance, 2) - 1)
                {
                    Mathf.Clamp(closestPoint, 0, patrolePoints.Count);
                    state = State.PATROLING;
                }

                rigidbody.velocity =
                        (patrolePoints[closestPoint].position - transform.position).normalized * backSpeed;
                break;
            default:
                break;
        }

        customTime += Time.deltaTime;
    }

    void FindClosestPoint()
    {
        int closestPointIndex = 0;
        Vector3 pos = transform.position;
        for (int i = 1; i < patrolePoints.Count; i++)
        {
            if( Vector3.SqrMagnitude(pos - patrolePoints[i].position) <
                Vector3.SqrMagnitude(pos - patrolePoints[closestPointIndex].position))
            {
                closestPointIndex = i;
            }
        }
        closestPoint = closestPointIndex;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chasingRadius);
        for (int i = 0; i < patrolePoints.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(patrolePoints[i].position, 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(patrolePoints[i].position, patrolePoints[(i + 1) % (patrolePoints.Count)].position);
        }
    }
}
