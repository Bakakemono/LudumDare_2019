using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWave : MonoBehaviour
{
    [SerializeField] private Transform ghostBody;

    [SerializeField] private AnimationCurve floatingWave;

    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float amplitude = 0.2f;

    private Vector3 startingPos;

    void Start()
    {
        startingPos = ghostBody.position;
    }

    void Update()
    {
        ghostBody.position = 
            new Vector3(
                ghostBody.position.x,
                startingPos.y + floatingWave.Evaluate(Time.time * speed) * amplitude,
                ghostBody.position.z
                );
    }
}
