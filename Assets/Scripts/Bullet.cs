using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Bullet : MonoBehaviour
{
    
    public void BulletDisactive()
    {
        DOVirtual.DelayedCall(0.5f, () => gameObject.SetActive(false));
    }
}
