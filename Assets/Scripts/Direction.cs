using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour
{

    public GameObject target, me;
    public float x, y, z;
    void Start()
    {
        x = transform.localScale.x;
        y = transform.localScale.y;
        z = transform.localScale.z;
    }


    void Update()
    {
        Target();
    }
    void Target()
    {
        float distanceClosestTarget = Mathf.Infinity;
        Destroyable closestTarget = null;
        Destroyable[] allTargets = FindObjectsOfType<Destroyable>();
        if (allTargets.Length != 0)
        {
            foreach (Destroyable currentTarget in allTargets)
            {
                float distanceToTarget = (currentTarget.transform.position - this.transform.position).sqrMagnitude;
                if (distanceToTarget < distanceClosestTarget)
                {
                    distanceClosestTarget = distanceToTarget;
                    closestTarget = currentTarget;
                }
            }
            target = closestTarget.gameObject;

            z = Vector3.Distance(me.transform.position, target.transform.position) / 60;

            if (z < 0.12f)
            {
                z = 0.12f;
            }
            transform.LookAt(target.transform);
        }
    }
}