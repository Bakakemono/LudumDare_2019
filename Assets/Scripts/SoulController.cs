using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoulController : MonoBehaviour
{
    private Transform customTransform;

    [Header("movement")]
    [SerializeField] private float speed = 1;

    private Vector2 currentInput = Vector2.zero;
    void Start()
    {
        customTransform = transform;
    }

    void Update()
    {
        currentInput.x = Input.GetAxis("Horizontal");
        currentInput.y = Input.GetAxis("Vertical");

        customTransform.position = customTransform.position + new Vector3(currentInput.x, 0, currentInput.y).normalized * speed * Time.deltaTime;
    }
}
