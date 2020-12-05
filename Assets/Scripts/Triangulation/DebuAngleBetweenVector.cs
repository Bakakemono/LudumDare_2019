using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuAngleBetweenVector : MonoBehaviour
{
    [SerializeField] Transform obj1;
    [SerializeField] Transform obj2;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, obj1.position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, obj2.position);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(obj1.position, 0.1f);
        Gizmos.DrawSphere(obj2.position, 0.1f);

        Vector2 one = obj1.position - transform.position;
        Vector2 two = obj2.position - transform.position;
        Debug.Log(Mathf.Acos((one.x * two.x + one.y * two.y) / (Mathf.Sqrt(one.x * one.x + one.y * one.y) * Mathf.Sqrt(two.x * two.x + two.y * two.y))));
    }
}
