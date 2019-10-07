using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private Transform customTransform;

    private CharmManager charmManager;

    private PeonBehavior currentPeon;
    [SerializeField] private float strengthLaunch;

    [SerializeField] private Transform firePoint;

    private bool isArmed = false;

    void Start()
    {
        customTransform = transform;
        charmManager = FindObjectOfType<CharmManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (!isArmed)
            {
                if(!charmManager.IsAmmo())
                    return;

                currentPeon = charmManager.LoadPeon();


                currentPeon.SetLauncher(firePoint);
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

    private void Reload()
    {
        currentPeon.statePeon = PeonBehavior.StatePeon.LOADING;
    }

    private void Launch()
    {
        isArmed = false;
        currentPeon.GetComponent<Rigidbody>().AddForce((customTransform.TransformPoint(0, 0, 1) - customTransform.position).normalized * strengthLaunch);
    }
}
