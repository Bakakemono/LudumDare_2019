using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeonDetector : MonoBehaviour {
    
    public List<PeonBehavior> closePeons;
    public List<PeonBehavior> charmedPeon;

    [SerializeField] public float detectionRadius = 2.0f;

    void Start() {
        GetComponent<SphereCollider>().radius = detectionRadius;
        closePeons = new List<PeonBehavior>();
    }

    void OnTriggerEnter(Collider collider)
    {
        PeonBehavior peon = collider.GetComponentInParent<PeonBehavior>();
        if (peon != null && !charmedPeon.Contains(peon))
        {
            closePeons.Add(peon);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        PeonBehavior peon = collider.GetComponentInParent<PeonBehavior>();
        if (peon != null && !charmedPeon.Contains(peon))
        {
            closePeons.Remove(peon);
        }
    }
}
