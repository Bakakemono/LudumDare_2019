using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLauncher : MonoBehaviour
{
    Vector3 fireDirection = new Vector3(0.0f, 0.0f, 1.0f);
    float fireAngle = 45.0f;
    float initialVelocity = 10.0f;

    int stepDefinition = 10;
    float totalTime = 5.0f;

    Transform bullet;

    bool physicalSimultation = false;
    bool positionSimultation = false;

    float startingTime;

    private void Update()
    {
        if (Input.GetKeyDown("y") && !physicalSimultation && !positionSimultation)
        {
            physicalSimultation = true;
            startingTime = Time.time;
        }
        else if(Input.GetKeyDown("x") && !physicalSimultation && !positionSimultation)
        {
            positionSimultation = true;
            startingTime = Time.time;
        }

        if (physicalSimultation)
        {
        }
        else if (positionSimultation)
        {

        }
    }


    private void OnDrawGizmos()
    {
        Vector3 fireDirectionNormalized = fireDirection.normalized;

        Vector3 newFireDirection =
            new Vector3(
                fireDirection.x,
                fireDirection.magnitude * Mathf.Tan(Mathf.Deg2Rad * fireAngle),
                fireDirection.z).normalized;

        float gravity = Physics.gravity.y;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + fireDirection);
        Gizmos.DrawSphere(transform.position + fireDirection, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + newFireDirection);
        Gizmos.DrawSphere(transform.position + newFireDirection, 0.1f);

        Gizmos.color = Color.red;
        for (int i = 0; i < stepDefinition; i++)
        {
            float currentTime = totalTime / stepDefinition * i;
            Vector3 currentPositionXZ =
                transform.position + (fireDirectionNormalized * initialVelocity * currentTime);

            float currentHeight =
                transform.position.y +
                initialVelocity * newFireDirection.y * currentTime +
                (0.5f * gravity * currentTime * currentTime);

            float nextTime = totalTime / stepDefinition * (i + 1);
            Vector3 nextPositionXZ =
                transform.position + (fireDirectionNormalized * initialVelocity * nextTime);

            float nextHeight =
                transform.position.y +
                initialVelocity * newFireDirection.y * nextTime +
                (0.5f * gravity * nextTime * nextTime);

            Gizmos.DrawLine(
                new Vector3(currentPositionXZ.x, currentHeight, currentPositionXZ.z),
                new Vector3(nextPositionXZ.x, nextHeight, nextPositionXZ.z)
                );
        }
    }
}
