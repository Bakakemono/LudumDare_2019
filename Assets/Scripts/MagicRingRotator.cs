using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MagicRingRotator : MonoBehaviour
{

    public enum AxisRotation
    {
        X,
        Y
    }

    [SerializeField] private AxisRotation axisRotation = AxisRotation.X;

    private const float baseRotationValue = 360;

    [SerializeField] private float turnPerSecond = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Rotate();
    }

    void Rotate()
    {
        switch (axisRotation)
        {
            case AxisRotation.X:
                transform.Rotate(Vector3.up, baseRotationValue * Time.deltaTime * turnPerSecond);
                break;
            case AxisRotation.Y:
                transform.Rotate(Vector3.right, baseRotationValue * Time.deltaTime * turnPerSecond);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
