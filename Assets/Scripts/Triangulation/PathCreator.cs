using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

struct Node
{
    public List<int> neighborsIndexes;
    public Node(int x = 0)
    {
        neighborsIndexes = new List<int>();
    }
}
struct ForbiddenPair {
    private const int pairSize = 2;
    public int[] pair;

    public ForbiddenPair(int index1, int index2)
    {
        pair = new int[pairSize];
        pair[0] = index1;
        pair[1] = index2;
    }

    public bool IsSamePair(int point1, int point2)
    {
        return (pair[0] == point1 && pair[1] == point2) ||
               (pair[1] == point1 && pair[0] == point2);
    }
}
public class PathCreator : MonoBehaviour
{
    [SerializeField] Triangulation triangulationManager;
    List<Transform> points = new List<Transform>();
    List<ForbiddenPair> forbiddenPairs = new List<ForbiddenPair>();
    List<Triangle> triangles;
    Node[] nodes = new Node[0];

    [SerializeField] int indexToCheck = 0;

    List<int> debugNonLoopingPoints = new List<int>();

    void Start()
    {
        points = triangulationManager.GetPoints();
        triangles = triangulationManager.GetTriangles();
        nodes = new Node[points.Count];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new Node();
            nodes[i].neighborsIndexes = new List<int>();
        }

        FindForbiddenPairs();
        CreateMinimumSpanningTree();
        RemoveLoop();
    }

    void FindForbiddenPairs()
    {
        for(int i = 0; i < triangles.Count; i++)
        {
            float[] length = new float[Triangle.trianglePointNumber];

            for (int j = 0; j < Triangle.trianglePointNumber; j++)
            {
                length[j] = 
                    (triangles[i].points[j].position -
                     triangles[i].points[(j + 1) % Triangle.trianglePointNumber].position).magnitude;
            }

            for (int j = 0; j < Triangle.trianglePointNumber; j++)
            {
                if (length[j] > length[(j + 1) % Triangle.trianglePointNumber] && length[j] > length[(j + 2) % Triangle.trianglePointNumber])
                {
                    forbiddenPairs.Add
                        (new ForbiddenPair(FindIndex(triangles[i].points[j]), FindIndex(triangles[i].points[(j + 1) % Triangle.trianglePointNumber])));
                }
            }
        }
    }

    void CreateMinimumSpanningTree() {
        for (int i = 0; i < points.Count; i++) {
            for (int j = 0; j < triangles.Count; j++) {
                if (triangles[j].points.Contains(points[i])) {
                    for (int k = 0; k < Triangle.trianglePointNumber; k++) {
                        if (triangles[j].points[k] != points[i]) {
                            bool isForbiddenPair = false;
                            foreach (ForbiddenPair forbiddenPair in forbiddenPairs) {
                                if (forbiddenPair.IsSamePair(FindIndex(triangles[j].points[k]), i)) {
                                    isForbiddenPair = true;
                                    break;
                                }
                            }
                            if(isForbiddenPair)
                                break;

                            nodes[i].neighborsIndexes.Add(FindIndex(triangles[j].points[k]));
                        }
                    }
                }
            }
        }
    }

    void RemoveLoop()
    {
        List<int> nonLoopingPoint = new List<int>();

        for (int i = 0; i < nodes.Length; i++)
        {
            if(nodes[i].neighborsIndexes.Count == 1)
            {
                nonLoopingPoint.Add(i);
            }
        }

        //for (int i = 0; i < nodes.Length; i++)
        //{
        //    if (nonLoopingPoint.Contains(i))
        //        continue;

        //    int neighborsLength = nodes[i].neighborsIndexes.Count;

        //    if (neighborsLength > 2)
        //        continue;

        //    foreach (int pointIndex in nodes[i].neighborsIndexes)
        //    {
        //        if (nonLoopingPoint.Contains(pointIndex))
        //        {
        //            nonLoopingPoint.Add(i);
        //        }
        //    }
        //}
        debugNonLoopingPoints = nonLoopingPoint;
        Debug.Log(nonLoopingPoint.Count);
    }

    int FindIndex(Transform point) {
        for (int i = 0; i < points.Count; i++) {
            if (points[i] == point) {
                return i;
            }
        }

        return -1;
    }

    void OnDrawGizmos() {
        for (int i = 0; i < points.Count; i++)
        {
            if (debugNonLoopingPoints.Contains(i))
            {
                Gizmos.color = Color.cyan;
            }
            else
            {
                Gizmos.color = Color.white;
            }
            Gizmos.DrawSphere(points[i].position, 0.2f);
        }
        Gizmos.color = Color.white;
        for (int i = 0; i < points.Count; i++)
        {
            if (debugNonLoopingPoints.Contains(i))
                continue;
            for (int j = 0; j < nodes[i].neighborsIndexes.Count; j++)
            {
                if(debugNonLoopingPoints.Contains(nodes[i].neighborsIndexes[j]))
                    continue;
                Gizmos.DrawLine(points[i].position, points[nodes[i].neighborsIndexes[j]].position);
            }
        }

        Gizmos.color = Color.red * 0.4f;
        for (int i = 0; i < debugNonLoopingPoints.Count; i++)
        {
            for (int j = 0; j < nodes[debugNonLoopingPoints[i]].neighborsIndexes.Count; j++)
            {
                Gizmos.DrawLine(points[debugNonLoopingPoints[i]].position, points[nodes[debugNonLoopingPoints[i]].neighborsIndexes[j]].position);
            }
        }

        //Gizmos.color = Color.red * 0.2f;
        //for (int i = 0; i < forbiddenPairs.Count; i++)
        //{
        //    Gizmos.DrawLine(points[forbiddenPairs[i].pair[0]].position, points[forbiddenPairs[i].pair[1]].position);

        //}
    }
}
