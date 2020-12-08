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
    int[] pair;

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
    List<Transform> points;
    List<ForbiddenPair> forbiddenPairs = new List<ForbiddenPair>();
    List<Triangle> triangles;
    List<Node> nodes;


    private void Start()
    {
        points = triangulationManager.GetPoints();
        triangles = triangulationManager.GetTriangles();
        nodes = new List<Node>();
        foreach (Transform point in points) {
            nodes.Add(new Node());
        }

        FindForbiddenPairs();
        CreateMinimumSpanningTree();
    }

    void FindForbiddenPairs()
    {
        for(int i = 0; i < triangles.Count; i++)
        {
            float[] length = new float[triangles[i].points.Length];
            for (int j = 0; j < triangles[i].points.Length; j++)
            {
                length[j] = (triangles[i].points[j].position - triangles[i].points[(j + 1) & Triangle.trianglePointNumber].position).magnitude;
            }

            for (int j = 0; j < triangles[i].points.Length; j++)
            {
                if(length[j] > length[(i + 1) % triangles[i].points.Length] && length[i] > length[(i + 2) % triangles[i].points.Length])
                {
                    forbiddenPairs.Add(new ForbiddenPair(FindIndex(triangles[i].points[j]), FindIndex(triangles[i].points[(j + 1) & Triangle.trianglePointNumber])));
                }
            }
        }
    }

    void CreateMinimumSpanningTree() {
        for (int i = 0; i < points.Count; i++) {
            for (int j = 0; j < triangles.Count; j++) {
                if (triangles[j].points.Contains(points[i])) {
                    for (int k = 0; k < triangles[j].points.Length; k++) {
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

    int FindIndex(Transform point) {
        for (int i = 0; i < points.Count; i++) {
            if (points[i] == point) {
                return i;
            }
        }

        return -1;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        for (int i = 0; i < nodes.Count; i++) {
            for (int j = 0; j < nodes[i].neighborsIndexes.Count; j++) {
                Gizmos.DrawLine(points[i].position, points[nodes[i].neighborsIndexes[j]].position);
            }
        }
    }
}
