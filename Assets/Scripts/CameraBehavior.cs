using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Transform customTransform;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Vector3 relativePosition = new Vector3(0, 3, -3);
    [SerializeField] private Vector3 relativeLookAim = new Vector3(0, 1, 0.5f);

    void Start()
    {
        customTransform = transform;
    }

    void LateUpdate()
    {
        customTransform.position = playerTransform.position + relativePosition;
        customTransform.LookAt(playerTransform.position + relativeLookAim);
    }
}
