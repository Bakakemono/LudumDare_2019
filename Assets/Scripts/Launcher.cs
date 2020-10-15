using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private Transform customTransform;
    [SerializeField] private Transform ghostBody;
    [SerializeField] MeshRenderer ghostMeshRenderer;

    private PeonBehavior currentPeon;

    [SerializeField] private float strengthLaunch;


    private bool isArmed = false;

    [SerializeField] private PeonDetector peonDetector;

    [SerializeField] List<PeonBehavior> charmedPeons;

    [SerializeField] float expandFactor = 3.0f;
    [SerializeField] float transparancyFactor = 0.1f;
    [SerializeField] float transformationSpeed = 10.0f;

    void Start() {
        peonDetector = GetComponentInChildren<PeonDetector>();
        customTransform = transform;
    }

    void Update() {
        float size = 0;
        float transparancy = 0;
        if (isArmed) {
            size = Mathf.Lerp(ghostBody.localScale.x, expandFactor, transformationSpeed * Time.deltaTime);
            transparancy = Mathf.Lerp(ghostMeshRenderer.material.color.a, transparancyFactor, transformationSpeed * Time.deltaTime);
        } else {
            size = Mathf.Lerp(ghostBody.localScale.x, 1, transformationSpeed * Time.deltaTime);
            transparancy = Mathf.Lerp(ghostMeshRenderer.material.color.a, 1, transformationSpeed * Time.deltaTime);
        }
        ghostBody.localScale = new Vector3(size, size, size);
        ghostMeshRenderer.material.color = new Color(1, 1, 1, transparancy);

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
        currentPeon.GetComponent<Rigidbody>().AddForce((customTransform.TransformPoint(0, 0, 1) - customTransform.position).normalized * strengthLaunch);
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
}
