using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingBehavior : MonoBehaviour
{
    [SerializeField] private Transform ghostBody;
    private List<Transform> plates;

    [SerializeField] private GameObject prefab;

    [SerializeField] private int plateNumber = 6;

    [SerializeField] private float pulsationAmplitude = 0.1f;

    [SerializeField] private float pulsationSpeed = 0.1f;

    [SerializeField] private float rotationSpeed = 1.0f;

    [SerializeField] private float elongationAmplitude = 0.3f;

    [SerializeField] private float elongationSpeed = 2.0f;

    [SerializeField] private float radius = 1.0f;

    void Start()
    {
        plates = new List<Transform>();

        for (int i = 0; i < plateNumber; i++)
        {
            plates.Add(Instantiate(prefab).transform);
            plates[i].transform.parent = ghostBody;
        }
    }

    void Update()
    {
        Pulse();
    }

    void Pulse()
    {
        for (int i = 0; i < plateNumber; i++)
        {
            plates[i].localPosition = new Vector3(
                                     Mathf.Sin((Time.time + ((float)(i) / plateNumber) * Mathf.PI * 2) * rotationSpeed) * radius *
                                     (1 + Mathf.Sin((Time.time * pulsationSpeed * Mathf.PI * 2) * pulsationSpeed) * pulsationAmplitude),
                                     Mathf.Cos((Time.time + ((float)(i) / plateNumber) * Mathf.PI * 2) * rotationSpeed) * radius *
                                     (1 + Mathf.Sin((Time.time * pulsationSpeed * Mathf.PI * 2) * pulsationSpeed) * pulsationAmplitude),
                                     0);

            plates[i].localScale = new Vector3(0.2f, 0.2f, 1 + Mathf.Sin((Time.time + ((float)(i) / plateNumber) * Mathf.PI * 2) * elongationSpeed) * elongationAmplitude);
        }
    }

    void PulseWithRotation()
    {
        
    }
}