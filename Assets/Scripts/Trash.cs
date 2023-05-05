using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Trash : MonoBehaviour
{
    void Start()
    {
        DOVirtual.DelayedCall(4f, () => 
        {
            if (GetComponent<Collider>())
            {
                GetComponent<Collider>().isTrigger = true;
            }
        });
        DOVirtual.DelayedCall(6f, () =>
        {
            Destroy(gameObject);
        });
    }

}
