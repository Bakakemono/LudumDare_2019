using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PeonBehavior : MonoBehaviour
{
    public enum StatePeon
    {
        STANDBY,
        IN_LOVE,
        LOADING,
        AMMO,
        ARMED,
        LAUNCHED,
        LANDED
    }

    [SerializeField] public StatePeon statePeon = StatePeon.IN_LOVE;

    private Transform customTransform;
    [SerializeField] private Transform peonBody;

    private Transform launcher;

    private bool isLoading = false;

    private int nmbFrameToReload = 25;
    private Vector3 standByPosition;
    private Rigidbody rigidbody;

    private float closestPoint = 2;

    private float speed = 5;


    private Transform follow;
    
    void Start()
    {
        customTransform = transform;
        standByPosition = customTransform.position;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (statePeon)
        {
            case StatePeon.STANDBY:
                break;
            case StatePeon.IN_LOVE:
                FollowTheThing();
                break;
            case StatePeon.LOADING:
                if (!isLoading)
                {
                    rigidbody.velocity = Vector3.zero;
                    isLoading = true;
                    StartCoroutine(Load());
                }
                break;
            case StatePeon.AMMO:
                break;
            case StatePeon.LAUNCHED:
                rigidbody.useGravity = true;
                customTransform.LookAt(rigidbody.velocity + transform.position);
                break;
            case StatePeon.LANDED:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void LateUpdate()
    {
        switch (statePeon)
        {
            case StatePeon.AMMO:
                customTransform.position = launcher.position;
                customTransform.rotation = launcher.rotation;
                break;
        }
    }

    public void UpdateFollower(Transform follower)
    {
        follow = follower;
    }

    public void SetLauncher(Transform currentLauncher)
    {
        launcher = currentLauncher;
    }

    private void FollowTheThing()
    {
        if (Vector3.Distance(customTransform.position, follow.position) > closestPoint)
        {
            LookAtThis(follow.position);
            rigidbody.velocity = (follow.position + new Vector3(0, 0.5f) - customTransform.position).normalized * speed;
        }
        else
        {
            LookAtThis(follow.position);
            rigidbody.velocity = Vector3.zero;
        }
    }

    void LookAtThis(Vector3 thisGuy)
    {
        var lookPos = thisGuy - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
    }

    private IEnumerator Load()
    {
        Vector3 startPos = customTransform.position;
        Quaternion startRot = customTransform.rotation;


        Quaternion startRotPeon = peonBody.localRotation;
        Quaternion goalRotPeon = peonBody.localRotation;
        goalRotPeon = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));

        for (int i = 1; i <= nmbFrameToReload; i++)
        {
            customTransform.position =
                Vector3.Lerp(startPos, launcher.position, (float)i / nmbFrameToReload);
            customTransform.rotation = Quaternion.Lerp(startRot, launcher.rotation, (float)i / nmbFrameToReload);
            peonBody.localRotation = Quaternion.Lerp(startRotPeon, goalRotPeon, (float)i / nmbFrameToReload);
            yield return new WaitForFixedUpdate();
        }

        statePeon = StatePeon.AMMO;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (statePeon == StatePeon.LAUNCHED)
            statePeon = StatePeon.LANDED;
    }
}
