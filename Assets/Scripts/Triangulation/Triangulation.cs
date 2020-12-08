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
            (points[0].GetInstanceID() == triangle.points[0].GetInstanceID() &&
             points[1].GetInstanceID() == triangle.points[1].GetInstanceID() &&
             points[2].GetInstanceID() == triangle.points[2].GetInstanceID()) ||

            (points[0].GetInstanceID() == triangle.points[0].GetInstanceID() &&
             points[1].GetInstanceID() == triangle.points[2].GetInstanceID() &&
             points[2].GetInstanceID() == triangle.points[1].GetInstanceID()) ||

            (points[0].GetInstanceID() == triangle.points[1].GetInstanceID() && 
             points[1].GetInstanceID() == triangle.points[0].GetInstanceID() && 
             points[2].GetInstanceID() == triangle.points[2].GetInstanceID()) ||

            (points[0].GetInstanceID() == triangle.points[1].GetInstanceID() &&
             points[1].GetInstanceID() == triangle.points[2].GetInstanceID() &&
             points[2].GetInstanceID() == triangle.points[0].GetInstanceID()) ||

            (points[0].GetInstanceID() == triangle.points[2].GetInstanceID() &&
             points[1].GetInstanceID() == triangle.points[0].GetInstanceID() &&
             points[2].GetInstanceID() == triangle.points[1].GetInstanceID()) ||

            (points[0].GetInstanceID() == triangle.points[2].GetInstanceID() &&
             points[1].GetInstanceID() == triangle.points[1].GetInstanceID() &&
             points[2].GetInstanceID() == triangle.points[0].GetInstanceID());
    }
}

struct Polygone
{
    public List<Transform> points;
    
    public Polygone(List<Transform> originPoints, List<Triangle> originTriangles)
    {
        points = new List<Transform>();

        if(originPoints.Count == 0)
        {
            return;
        }

        List<Transform> trashedPoints = new List<Transform>();

        for (int i = 0; i < originPoints.Count; i++)
        {
            if (!trashedPoints.Contains(originPoints[i]))
            {
                trashedPoints.Add(originPoints[i]);
            }
        }

        int currentIndex = 0;
        points.Add(trashedPoints[currentIndex]);

        Vector2 center = new Vector2();

        List<int> checkedIndex = new List<int>();
        checkedIndex.Add(0);

        for(int i = 0; i < trashedPoints.Count; i++)
        {
            center += (Vector2)trashedPoints[i].position;
        }
        center /= trashedPoints.Count;

        float bestAngle = 0.0f;
        int bestIndex = 0;
        for (int i = 1; i < trashedPoints.Count; i++)
        {
            bool isInSameTriangle = false;
            for (int j = 0; j < originTriangles.Count; j++)
            {
                if(originTriangles[j].AreInTriangle(points[currentIndex], trashedPoints[i]))
                {
                    isInSameTriangle = true;
                    break;
                }
            }

            if (!isInSameTriangle)
                continue;

            float newAngle = Vector2.Angle(center - (Vector2)points[currentIndex].position, trashedPoints[i].position - points[currentIndex].position);
            if (newAngle > bestAngle)
            {
                bestAngle = newAngle;
                bestIndex = i;
            }
        }

        points.Add(trashedPoints[bestIndex]);
        checkedIndex.Add(bestIndex);
        currentIndex++;

        while (true)
        {
            if(checkedIndex.Count == trashedPoints.Count)
            {
                break;
            }
            bestAngle = 0.0f;

            for (int i = 0; i < trashedPoints.Count; i++)
            {
                if (checkedIndex.Contains(i))
                    continue;

                bool isInSameTriangle = false;
                for (int j = 0; j < originTriangles.Count; j++)
                {
                    if (originTriangles[j].AreInTriangle(points[currentIndex], trashedPoints[i]))
                    {
                        isInSameTriangle = true;
                        break;
                    }
                }

                if (!isInSameTriangle)
                    continue;

                float newAngle = 
                    Vector2.Angle(
                        points[currentIndex - 1].position - points[currentIndex].position,
                        trashedPoints[i].position - points[currentIndex].position);

                if (newAngle > bestAngle)
                {
                    bestAngle = newAngle;
                    bestIndex = i;
                }
            }

            points.Add(trashedPoints[bestIndex]);
            checkedIndex.Add(bestIndex);
            currentIndex++;
        }
    }
}

public class Triangulation : MonoBehaviour
{
    int pointNumber = 4;

    [SerializeField] Transform[] suroundingTriangle;
    [SerializeField] List<Transform> points = new List<Transform>();

    [SerializeField] List<Triangle> triangles = new List<Triangle>();

    [Header("Debug Value")]
    [SerializeField] int triangleToRender = 1;
    [SerializeField] GameObject debugGameObject;
    [SerializeField] float delayBetweenEachPoint = 0.5f;
    [SerializeField] float delayTriangleCreation = 0.0f;

    [Header("Spawn points")]
    [SerializeField] int objectNumber = 30;
    [SerializeField] int randomSeed = 0;
    [SerializeField] float areaRadius = 15.0f;
    [SerializeField] bool renderArea = false;


    List<Triangle> debugTrashedTriangles = new List<Triangle>();

    Transform[] debugCurrentCheckedPoints = new Transform[3];

    private void Awake()
    {
        if(randomSeed == 0)
        {
            randomSeed = Random.Range(1000, 1000);
            Random.InitState(randomSeed);
        }
        else
        {
            Random.InitState(randomSeed);
        }
        GeneratePoints();

        InstantiateSurroundingTriangle();

        ProcessPoints();

        EraseSuperTriangle();
    }

    private void Update()
    {
    }

    void GeneratePoints()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject point = 
                Instantiate(
                    debugGameObject,
                    new Vector3(Random.Range(-areaRadius, areaRadius), Random.Range(-areaRadius, areaRadius)),
                    Quaternion.identity);
            points.Add(point.transform);
        }
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

            for (int j = 0; j < triangleToRemove.Count; j++)
            {
                triangles.Remove(triangleToRemove[j]);
            }

            Polygone polygone = new Polygone(trashedPoints, triangleToRemove);

            for (int j = 0; j < polygone.points.Count; j++)
            {
                triangles.Add(new Triangle(points[i], polygone.points[j], polygone.points[(j + 1) % polygone.points.Count]));
            }
        }

    }

    IEnumerator ProcessPointsIE()
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

            for (int j = 0; j < triangleToRemove.Count; j++)
            {
                triangles.Remove(triangleToRemove[j]);
            }

            Polygone polygone = new Polygone(trashedPoints, triangleToRemove);

            for (int j = 0; j < polygone.points.Count; j++)
            {
                triangles.Add(new Triangle(points[i], polygone.points[j], polygone.points[(j + 1) % polygone.points.Count]));
                yield return new WaitForSeconds(delayTriangleCreation);
            }

            yield return new WaitForSeconds(delayBetweenEachPoint);
        }

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

    public List<Transform> GetPoints()
    {
        return points;
    }

    public List<Triangle> GetTriangles()
    {
        return triangles;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan * new Color(1.0f, 1.0f, 1.0f, 1.0f);

        for (int i = 0; i < suroundingTriangle.Length; i++)
        {
            Gizmos.DrawSphere(suroundingTriangle[i].position, 0.3f);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i].position, 0.2f);
        }
        Color lightRed = Color.red * new Color(1, 1, 1, 0.5f);

        bool first = false;
        bool second = false;
        for (int i = 0; i < triangles.Count; i++)
        {
            float lengthA = Vector3.SqrMagnitude(triangles[i].points[0].position - triangles[i].points[1].position);
            float lengthB = Vector3.SqrMagnitude(triangles[i].points[1].position - triangles[i].points[2].position);
            float lengthC = Vector3.SqrMagnitude(triangles[i].points[2].position - triangles[i].points[0].position);

            bool isALonger = lengthA > lengthB && lengthA > lengthC;
            bool isBLonger = lengthB > lengthA && lengthB > lengthC;
            bool isCLonger = lengthC > lengthB && lengthC > lengthA;

            if (first)
                Gizmos.color = Color.white;
            else
                Gizmos.color = lightRed;
            Gizmos.DrawLine(triangles[i].points[0].position, triangles[i].points[1].position);
            if (second)
                Gizmos.color = Color.white;
            else
                Gizmos.color = lightRed;
            Gizmos.DrawLine(triangles[i].points[1].position, triangles[i].points[2].position);
            if(!first && !second)
                Gizmos.color = Color.white;
            else
                Gizmos.color = lightRed;
            Gizmos.DrawLine(triangles[i].points[2].position, triangles[i].points[0].position);
        }

        //Gizmos.color = Color.white;
        //Gizmos.DrawLine(triangles[triangleToRender].points[0].position, triangles[triangleToRender].points[1].position);
        //Gizmos.DrawLine(triangles[triangleToRender].points[1].position, triangles[triangleToRender].points[2].position);
        //Gizmos.DrawLine(triangles[triangleToRender].points[2].position, triangles[triangleToRender].points[0].position);
    }
}
