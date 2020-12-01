using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    Vector2 mapSize = Vector2.zero;
    Vector2 topRight = Vector2.zero;  
    Vector2 bottomLeft = Vector2.zero;
    Vector2 center = Vector2.zero;

    [SerializeField] Transform map;

    [SerializeField] float margin = 10.0f;
    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        Transform[] allElements = map.GetComponentsInChildren<Transform>();
        List<Transform> elements = new List<Transform>();

        foreach (Transform element in allElements)
        {
            if (element.gameObject.CompareTag("Area"))
            {
                elements.Add(element);
            }
        }
        foreach(Transform element in elements)
        {
            if(element.position.x < bottomLeft.x)
            {
                bottomLeft.x = element.position.x;
            }
            else if (element.position.z < bottomLeft.y)
            {
                bottomLeft.y = element.position.z;
            }
            else if (element.position.x > topRight.x)
            {
                topRight.x = element.position.x;
            }
            else if (element.position.z > topRight.y)
            {
                topRight.y = element.position.z;
            }
        }
        topRight += new Vector2(margin, margin);
        topRight = new Vector2(Mathf.RoundToInt(topRight.x), Mathf.RoundToInt(topRight.y));
        bottomLeft -= new Vector2(margin, margin);
        bottomLeft = new Vector2(Mathf.RoundToInt(bottomLeft.x), Mathf.RoundToInt(bottomLeft.y));

        mapSize = topRight - bottomLeft;
        center = topRight + bottomLeft;
        center /= 2;
    }

    void CalculateBoundaries()
    {

    }

    void DebugResetMap()
    {
        mapSize = Vector2.zero;
        topRight = Vector2.zero;
        bottomLeft = Vector2.zero;
        center = Vector2.zero;
    }

     void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(new Vector3(center.x, 1.0f, center.y), new Vector3(mapSize.x, 2.0f, mapSize.y));

        Transform[] allElements = map.GetComponentsInChildren<Transform>();
        List<Transform> elements = new List<Transform>();

        foreach (Transform element in allElements)
        {
            if (element.gameObject.CompareTag("Area"))
            {
                elements.Add(element);
            }
        }

        Gizmos.color = Color.magenta;
        foreach (Transform element in elements)
        {
            Gizmos.DrawSphere(element.position + Vector3.up, 0.5f);
        }
    }
}
