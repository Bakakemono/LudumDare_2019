using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class Example : MonoBehaviour
{

}

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(Example))]
public class DebugTriangulation : Editor
{
    // Custom in-scene UI for when ExampleScript
    // component is selected.
    public void OnSceneGUI()
    {
        return;
        //var t = target as Triangulation;
        //var tr = t.points;
        //// display an orange disc where the object is
        //var color = new Color(1, 0.8f, 0.4f, 1);
        //Handles.color = color;

        //for (int i = 0; i < tr.Count; i++)
        //{
        //    Handles.Label(tr[i].position + new Vector3(0.5f, 0.5f), i.ToString());
        //}
    }
}
