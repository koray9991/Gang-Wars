using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BoxGun : MonoBehaviour
{
    public Transform pos1, pos2;
    public float rotx,roty,rotz;
    private void Start()
    {
        GoPos1();
    }
    void GoPos1()
    {
        transform.DOMove(pos2.position,2).SetEase(Ease.Linear).OnComplete(() => GoPos2());
    }
    void GoPos2()
    {
        transform.DOMove(pos1.position, 2).SetEase(Ease.Linear).OnComplete(() => GoPos1());
    }
    private void FixedUpdate()
    {
        transform.Rotate(rotx,roty,rotz);
    }

}
