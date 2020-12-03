using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct Triangle
{
    public Triangle(Transform a, Transform b, Transform c)
    {
        points = new Transform[3];
        points[0] = a;
        points[1] = b;
        points[2] = c;
        circumCenter = new Vector2();
        radius = 0;
        CalculateCircumcenter();
    }
    public Transform[] points;
    public Vector2 circumCenter;
    public float radius;

    void CalculateCircumcenter()
    {
        Vector2 direction1 = (points[1].position - points[0].position).normalized;
        Vector2 startPos1 = (points[0].position + points[1].position) / 2.0f;
        Vector2 nextPoint1 = startPos1 + new Vector2(direction1.y, -direction1.x);

        Vector3 line1;
        line1.x = nextPoint1.y - startPos1.y;
        line1.y = startPos1.x - nextPoint1.x;
        line1.z = -(line1.x * startPos1.x + line1.y * startPos1.y);

        Vector2 direction2 = (points[2].position - points[1].position).normalized;
        Vector2 startPos2 = (points[1].position + points[2].position) / 2.0f;
        Vector2 nextPoint2 = startPos2 + new Vector2(direction2.y, -direction2.x);

        Vector3 line2;
        line2.x = nextPoint2.y - startPos2.y;
        line2.y = startPos2.x - nextPoint2.x;
        line2.z = -(line2.x * startPos2.x + line2.y * startPos2.y);

        circumCenter = new Vector2(
            (line1.y * line2.z - line2.y * line1.z) / (line1.x * line2.y - line2.x * line1.y),
            (line2.x * line1.z - line1.x * line2.z) / (line1.x * line2.y - line2.x * line1.y)
            );

        radius = ((Vector2)points[0].position - circumCenter).magnitude;
    }

    public bool AreInTriangle(Transform pointOne, Transform pointTwo)
    {
        bool check = false;
        bool doBreak = false;

        for (int i = 0; i < points.Length; i++)
        {
            for (int j = 0; j < points.Length; j++)
            {
                if(i != j && points[i] == pointOne && points[j] == pointTwo)
                {
                    check = true;
                    doBreak = true;
                    break;
                }
            }
            if (doBreak)
                break;
        }

        return check;
    }

    public bool SameTriangle(Triangle triangle)
    {
        return
            (points[0] == triangle.points[0] && points[1] == triangle.points[1] && points[2] == triangle.points[2]) ||
            (points[0] == triangle.points[0] && points[1] == triangle.points[2] && points[2] == triangle.points[1]) ||
            (points[0] == triangle.points[1] && points[1] == triangle.points[0] && points[2] == triangle.points[2]) ||
            (points[0] == triangle.points[1] && points[1] == triangle.points[2] && points[2] == triangle.points[0]) ||
            (points[0] == triangle.points[2] && points[1] == triangle.points[0] && points[2] == triangle.points[1]) ||
            (points[0] == triangle.points[2] && points[1] == triangle.points[1] && points[2] == triangle.points[0]);
    }
}
public class Triangulation : MonoBehaviour
{
    int pointNumber = 4;

    [SerializeField] Transform[] suroundingTriangle;
    [SerializeField] List<Transform> points = new List<Transform>();

    [SerializeField] List<Triangle> triangles = new List<Triangle>();

    int currentPoint = 0;

    List<Transform> debugTrashPoint = new List<Transform>();

    List<Triangle> debugTrashedTriangles = new List<Triangle>();
    private void Start()
    {
        InstantiateSurroundingTriangle();

        //ProcessPoints();

        //EraseSuperTriangle();

        StartCoroutine(ProcessPointsIE());
    }

    void InstantiateSurroundingTriangle()
    {
        for(int i = 0; i < suroundingTriangle.Length; i++)
        {
            triangles.Add(
                new Triangle(
                    points[0],
                    suroundingTriangle[i],
                    suroundingTriangle[(i + 1) % (suroundingTriangle.Length)])
                );
        }
    }

    void ProcessPoints()
    {
        for (int i = 1; i < points.Count; i++)
        {
            List<Transform> trashedPoints = new List<Transform>();
            List<Triangle> triangleToRemove = new List<Triangle>();
            for (int j = 0; j < triangles.Count; j++)
            {
                if ((triangles[j].circumCenter - (Vector2)points[i].position).sqrMagnitude < (triangles[j].radius * triangles[j].radius))
                {
                    for (int k = 0; k < triangles[j].points.Length; k++)
                    {
                        trashedPoints.Add(triangles[j].points[k]);
                    }
                    triangleToRemove.Add(triangles[j]);
                }
            }

            List<Transform> doublePoints = new List<Transform>();

            for (int j = 0; j < trashedPoints.Count; j++)
            {
                for (int k = 0; k < trashedPoints.Count; k++)
                {
                    if (k != j && trashedPoints[j] == trashedPoints[k] && !doublePoints.Contains(trashedPoints[j]))
                    {
                        doublePoints.Add(trashedPoints[j]);
                    }
                }
            }

            for (int j = 0; j < trashedPoints.Count; j++)
            {

                int nextIndex = (j + 1) % trashedPoints.Count;
                if (trashedPoints[j] != trashedPoints[nextIndex])
                {
                    if (!(doublePoints.Contains(trashedPoints[j]) && doublePoints.Contains(trashedPoints[nextIndex])))
                    {
                        triangles.Add(new Triangle(points[i], trashedPoints[j], trashedPoints[nextIndex]));
                    }
                }
            }

            for (int j = 0; j < triangleToRemove.Count; j++)
            {
                triangles.Remove(triangleToRemove[j]);
            }
        }
    }

    IEnumerator ProcessPointsIE()
    {
        for (int i = 1; i < points.Count; i++)
        {
            currentPoint = i;
            List<Transform> trashedPoints = new List<Transform>();
            List<Triangle> triangleToRemove = new List<Triangle>();
            for (int j = 0; j < triangles.Count; j++)
            {
                if ((triangles[j].circumCenter - (Vector2)points[i].position).sqrMagnitude < (triangles[j].radius * triangles[j].radius))
                {
                    

                    for (int k = 0; k < triangles[j].points.Length; k++)
                    {
                        trashedPoints.Add(triangles[j].points[k]);
                    }
                    triangleToRemove.Add(triangles[j]);
                }
            }

            List<Transform> doublePoints = new List<Transform>();

            for (int j = 0; j < trashedPoints.Count; j++)
            {
                for (int k = 0; k < trashedPoints.Count; k++)
                {
                    if (k != j && trashedPoints[j] == trashedPoints[k] && !doublePoints.Contains(trashedPoints[j]))
                    {
                        doublePoints.Add(trashedPoints[j]);
                    }
                }
            }

            if (i == 8)
            {
                //debugTrashedTriangles = triangleToRemove;
                //Debug.Log("Triangle number " + triangleToRemove.Count);

                //for (int j = 0; j < trashedPoints.Count; j++)
                //{
                //    Debug.Log("Trash point " + j + " : " + trashedPoints[j].position);
                //}

                //for (int j = 0; j < doublePoints.Count; j++)
                //{
                //    Debug.Log("Double point " + j + " : " + doublePoints[j].position);
                //}
            }

            for (int j = 0; j < triangleToRemove.Count; j++)
            {
                triangles.Remove(triangleToRemove[j]);
            }

            for (int j = 0; j < trashedPoints.Count; j++)
            {
                for (int k = 0; k < trashedPoints.Count; k++)
                {
                    if (trashedPoints[j] != trashedPoints[k])
                    {
                        Triangle triangle = new Triangle(points[i], trashedPoints[j], trashedPoints[k]);

                        bool sameTriangle = false;

                        for (int f = 0; f < triangles.Count; f++)
                        {
                            if (triangles[f].SameTriangle(triangle))
                            {
                                sameTriangle = true;
                                break;
                            }
                        }

                        if (!sameTriangle && !(doublePoints.Contains(trashedPoints[j]) && doublePoints.Contains(trashedPoints[k])))
                        {
                            bool isSameTriangle = false;
                            for (int f = 0; f < triangleToRemove.Count; f++)
                            {
                                if (triangleToRemove[f].AreInTriangle(trashedPoints[j], trashedPoints[k]))
                                {
                                    isSameTriangle = true;
                                    break;
                                }
                            }
                            if (!isSameTriangle)
                            {
                                continue;
                            }

                            yield return new WaitForSeconds(0.5f);
                            triangles.Add(new Triangle(points[i], trashedPoints[j], trashedPoints[k]));
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        debugTrashedTriangles = triangles;

        for (int j = 0; j < triangles.Count; j++)
        {
            Debug.Log(/*"Triangle CC " + j + " : " + */triangles[j].circumCenter);
        }

        Debug.Log("Total Triangle : " + triangles.Count);
        EraseSuperTriangle();
    }

    void EraseSuperTriangle()
    {
        List<Triangle> triangleToErase = new List<Triangle>();

        foreach (Triangle triangle in triangles)
        {
            bool skip = false;
            for(int i = 0; i < triangle.points.Length; i++)
            {
                for (int j = 0; j < suroundingTriangle.Length; j++)
                {
                    if(suroundingTriangle[j] == triangle.points[i])
                    {
                        triangleToErase.Add(triangle);
                        skip = true;
                    }
                }

                if (skip)
                {
                    break;
                }
            }
        }

        for(int i = 0; i < triangleToErase.Count; i++)
        {
            triangles.Remove(triangleToErase[i]);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan * new Color(1.0f, 1.0f, 1.0f, 1.0f);

        for (int i = 0; i < suroundingTriangle.Length; i++)
        {
            Gizmos.DrawSphere(suroundingTriangle[i].position, 0.3f);
        }

        for (int i = 0; i < points.Count; i++)
        {
            if(i == currentPoint)
            {
                Gizmos.color = Color.white;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(points[i].position, 0.2f);
        }
        Gizmos.color = Color.red;

        for (int i = 0; i < triangles.Count; i++)
        {
            Gizmos.DrawLine(triangles[i].points[0].position, triangles[i].points[1].position);
            Gizmos.DrawLine(triangles[i].points[1].position, triangles[i].points[2].position);
            Gizmos.DrawLine(triangles[i].points[2].position, triangles[i].points[0].position);

            //Gizmos.DrawWireSphere(triangles[i].circumCenter, triangles[i].radius);
        }

        Gizmos.color = Color.white * new Color(1, 1, 1, 0.5f);
        for (int i = 0; i < debugTrashedTriangles.Count; i++)
        {
            for (int j = 0; j < debugTrashedTriangles[i].points.Length; j++)
            {
                Gizmos.DrawLine(debugTrashedTriangles[i].points[j].position, debugTrashedTriangles[i].points[(j +  1) % debugTrashedTriangles[i].points.Length].position);
            }
            Gizmos.color = Color.white * new Color(1, 1, 1, 0.2f);
            //Gizmos.DrawWireSphere(debugTrashedTriangles[i].circumCenter, debugTrashedTriangles[i].radius);
            Gizmos.DrawSphere(debugTrashedTriangles[i].circumCenter, debugTrashedTriangles[i].radius * 0.1f);
        }
    }
}
