using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    void LateUpdate()
    {
        transform.LookAt(target.position);
    }
}
