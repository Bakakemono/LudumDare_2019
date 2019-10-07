using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LookForward : MonoBehaviour
{

    public enum LaunchingState
    {
        PRE_LAUNCHING,
        LAUNCHED,
        LANDED
    }

    public LaunchingState launchingState = LaunchingState.PRE_LAUNCHING;

    private Transform customTransform;

    void Start()
    {
        customTransform = transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        Launch();
    }

    private void Launch()
    {
        switch (launchingState)
        {
            case LaunchingState.PRE_LAUNCHING:
                break;
            case LaunchingState.LAUNCHED:
                customTransform.LookAt(GetComponent<Rigidbody>().velocity + transform.position);
                break;
            case LaunchingState.LANDED:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        launchingState = LaunchingState.LANDED;
    }

}