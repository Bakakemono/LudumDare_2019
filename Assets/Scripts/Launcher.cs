using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Launcher : MonoBehaviour
{
    private Transform customTransform;
    [SerializeField] private Transform ghostBody;
    [SerializeField] MeshRenderer ghostMeshRenderer;
    VisualEffect visualEffect;

    private PeonBehavior currentPeon;

    [SerializeField] private float strengthLaunch;


    private bool isArmed = false;

    [SerializeField] private PeonDetector peonDetector;

    [SerializeField] List<PeonBehavior> charmedPeons;

    [SerializeField] float expandFactor = 3.0f;
    [SerializeField] float transparancyFactor = 0.1f;
    [SerializeField] float transformationSpeed = 10.0f;
    float currentScale = 1.0f;

    Vector3 fireDirection = new Vector3(0.0f, 0.0f, 1.0f);
    [SerializeField] float fireAngle = 45.0f;

    [SerializeField] float mouseSensibility = 1.0f;
    float mouseMovementCount = 0.0f;

    [Header("Debug projectile line")]
    [SerializeField] int d_step = 100;
    [SerializeField] float d_totalTime = 10.0f;
    [SerializeField] Vector3 d_fireDirection = new Vector3(0.0f, 0.0f, 1.0f);
    [SerializeField] float d_fireAngle = 45.0f;
    [SerializeField] float d_initialVelocity = 10.0f;

    bool lockedMouse = true;

    void Start() {
        peonDetector = GetComponentInChildren<PeonDetector>();
        visualEffect = GetComponentInChildren<VisualEffect>();
        customTransform = transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        transform.LookAt(transform.position + d_fireDirection);

        mouseMovementCount += Input.GetAxis("Mouse X") * mouseSensibility;

        d_fireDirection = 
            new Vector3(
                Mathf.Sin(mouseMovementCount), 
                0.0f, 
                Mathf.Cos(mouseMovementCount)).normalized;

        float size = 0;
        float transparancy = 0;
        if (isArmed) {
            size = Mathf.Lerp(currentScale, expandFactor, transformationSpeed * Time.deltaTime);
        } else {
            size = Mathf.Lerp(currentScale, 1, transformationSpeed * Time.deltaTime);
        }
        visualEffect.SetFloat("RadiusFactor", size);
        currentScale = size;

        if (Input.GetKeyDown("c"))
        {
            CharmClosestPeon(customTransform.position);
        }

        if (Input.GetKeyDown("space"))
        {
            if (!isArmed)
            {
                if(!(charmedPeons.Count > 0))
                    return;

                currentPeon = LoadPeon();


                currentPeon.SetLauncher(ghostBody);
                isArmed = true;
            }
            if (currentPeon.statePeon == PeonBehavior.StatePeon.IN_LOVE)
            {
                Reload();
            }
            if (currentPeon.statePeon == PeonBehavior.StatePeon.AMMO)
            {
                currentPeon.statePeon = PeonBehavior.StatePeon.LAUNCHED;
                Launch();
            }
        }
    }

    void Reload()
    {
        currentPeon.statePeon = PeonBehavior.StatePeon.LOADING;
    }

    void Launch()
    {
        isArmed = false;
        //currentPeon.GetComponent<Rigidbody>().AddForce((customTransform.TransformPoint(0, 0, 1) - customTransform.position).normalized * strengthLaunch);


        Vector3 newFireDirection =
            new Vector3(
                d_fireDirection.x,
                d_fireDirection.magnitude * Mathf.Tan(Mathf.Deg2Rad * d_fireAngle),
                d_fireDirection.z).normalized;
        currentPeon.GetComponent<Rigidbody>().velocity = newFireDirection * d_initialVelocity;
    }

    bool CharmClosestPeon(Vector3 playerPosition)
    {
        int index = 0;
        float closestDist = peonDetector.detectionRadius * 2.0f;
        bool isPeonToCharm = false;
        for (int i = 0; i < peonDetector.closePeons.Count; i++)
        {
            float dist = Vector3.Distance(peonDetector.closePeons[i].transform.position, playerPosition);
            if (dist < closestDist)
            {
                index = i;
                closestDist = dist;
                isPeonToCharm = true;
            }
        }

        if (isPeonToCharm)
        {
            AddNewCharmedPeon(peonDetector.closePeons[index]);
            peonDetector.charmedPeon.Add(peonDetector.closePeons[index]);
            peonDetector.closePeons.RemoveAt(index);
        }


        return isPeonToCharm;
    }

    void AddNewCharmedPeon(PeonBehavior peon)
    {
        charmedPeons.Add(peon);
        if (charmedPeons.Count > 1)
        {
            charmedPeons[charmedPeons.Count - 1].UpdateFollower(charmedPeons[charmedPeons.Count - 2].transform);
        }
        else
        {
            charmedPeons[charmedPeons.Count - 1].UpdateFollower(ghostBody);
        }

        charmedPeons[charmedPeons.Count - 1].statePeon = PeonBehavior.StatePeon.IN_LOVE;
    }

    public PeonBehavior LoadPeon()
    {
        PeonBehavior peon = charmedPeons[0];

        charmedPeons.RemoveAt(0);
        if (charmedPeons.Count > 0)
            charmedPeons[0].UpdateFollower(ghostBody);

        return peon;
    }

    private void OnDrawGizmos()
    {
        Vector3 fireDirectionNormalized = d_fireDirection.normalized;

        Vector3 newFireDirection =
            new Vector3(
                d_fireDirection.x,
                d_fireDirection.magnitude * Mathf.Tan(Mathf.Deg2Rad * d_fireAngle),
                d_fireDirection.z).normalized;
        
        float gravity = Physics.gravity.y;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(ghostBody.position, ghostBody.position + d_fireDirection);
        Gizmos.DrawSphere(ghostBody.position + d_fireDirection, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(ghostBody.position, ghostBody.position + newFireDirection);
        Gizmos.DrawSphere(ghostBody.position + newFireDirection, 0.1f);

        Gizmos.color = Color.red;
        for (int i = 0; i < d_step; i++)
        {
            float currentTime = d_totalTime / d_step * i;
            Vector3 currentPositionXZ = 
                ghostBody.position + (newFireDirection * d_initialVelocity * currentTime);

            float currentHeight = 
                ghostBody.position.y +
                d_initialVelocity * newFireDirection.y * currentTime +
                (0.5f * gravity * currentTime * currentTime);

            float nextTime = d_totalTime / d_step * (i + 1);
            Vector3 nextPositionXZ =
                ghostBody.position + (newFireDirection * d_initialVelocity * nextTime);

            float nextHeight =
                ghostBody.position.y +
                d_initialVelocity * newFireDirection.y * nextTime +
                (0.5f * gravity * nextTime * nextTime);

            Gizmos.DrawLine(
                new Vector3(currentPositionXZ.x, currentHeight, currentPositionXZ.z),
                new Vector3(nextPositionXZ.x, nextHeight, nextPositionXZ.z)
                );
        }
    }
}
