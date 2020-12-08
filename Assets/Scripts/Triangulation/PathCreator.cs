using System.Collections;
using System.Collections.Generic;
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
    Transform[] pair;

    public ForbiddenPair(Transform index1, Transform index2)
    {
        pair = new Transform[pairSize];
        pair[0] = index1;
        pair[1] = index2;
    }

    bool IsSamePair(Transform point1, Transform point2)
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

    private void Start()
    {
        points = triangulationManager.GetPoints();
        triangles = triangulationManager.GetTriangles();
    }

    void FindForbiddenPairs()
    {
        for(int i = 0; i < triangles.Count; i++)
        {
            float[] length = new float[triangles[i].points.Length];
            for (int j = 0; j < triangles[i].points.Length; j++)
            {
                length[j] = (triangles[i].points[j].position - triangles[i].points[(j + 1) & triangles[i].points.Length].position).magnitude;
            }

            for (int j = 0; j < triangles[i].points.Length; j++)
            {
                if(length[j] > length[(i + 1) % triangles[i].points.Length] && length[i] > length[(i + 2) % triangles[i].points.Length])
                {
                    forbiddenPairs.Add(new ForbiddenPair(triangles[i].points[j], triangles[i].points[(j + 1) & triangles[i].points.Length]));
                }
            }
        }
    }
}
