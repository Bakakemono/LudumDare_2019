using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmManager : MonoBehaviour
{

    private float charmDistance = 2;

    private Transform playerTransform;

    private List<PeonBehavior> dumbPeons;

    private List<PeonBehavior> charmedLinkList;

    void Start()
    {
        PeonBehavior[] peons = FindObjectsOfType<PeonBehavior>();
        dumbPeons = new List<PeonBehavior>();
        foreach (PeonBehavior peon in peons)
        {
            dumbPeons.Add(peon);
        }

        playerTransform = FindObjectOfType<SoulController>().transform;

        charmedLinkList = new List<PeonBehavior>();
    }



    public bool CharmClosestPeon(Vector3 playerPosition)
    {
        PeonBehavior peon;
        int index = 0;
        float closestDist = 10000;
        bool isPeonToCharm = false;
        for (int i = 0; i < dumbPeons.Count; i++)
        {
            float dist = Vector3.Distance(dumbPeons[i].transform.position, playerPosition);
            if (dist < charmDistance && dist < closestDist)
            {
                index = i;
                closestDist = dist;
                isPeonToCharm = true;
                peon = dumbPeons[i];
            }
        }

        if (isPeonToCharm)
        {
            AddNewCharmedPeon(dumbPeons[index]);
            dumbPeons.RemoveAt(index);
        }


        return isPeonToCharm;
    }


    void AddNewCharmedPeon(PeonBehavior peon)
    {
        charmedLinkList.Add(peon);
        if (charmedLinkList.Count > 1)
        {
            charmedLinkList[charmedLinkList.Count - 1].UpdateFollower(charmedLinkList[charmedLinkList.Count - 2].transform);
        }
        else
        {
            charmedLinkList[charmedLinkList.Count - 1].UpdateFollower(playerTransform);
        }

        charmedLinkList[charmedLinkList.Count - 1].statePeon = PeonBehavior.StatePeon.IN_LOVE;
    }

    public bool IsAmmo()
    {
        Debug.Log(charmedLinkList.Count);
        return charmedLinkList.Count > 0;
    }

    public PeonBehavior LoadPeon()
    {
        PeonBehavior peon = charmedLinkList[0];

        charmedLinkList.RemoveAt(0);
        if(charmedLinkList.Count > 0)
            charmedLinkList[0].UpdateFollower(playerTransform);

        return peon;
    }
}
