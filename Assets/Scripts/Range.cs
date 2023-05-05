using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Range : MonoBehaviour
{
    public Player player;
    public List<GameObject> radars;
    public float delay = 0.5f;
    bool isOpen;
    private void Update()
    {
        if (player.isEnemyInRange)
        {
            if (!isOpen)
            {
                Radar();
                isOpen = true;
            }
        }
    }
    public void Radar()
    {
        radars[0].transform.DOScale(new Vector3(1, 1, 0.2f), 2).OnComplete(() => radars[0].transform.DOScale(new Vector3(0, 0, 0.2f), 0));
        DOVirtual.DelayedCall(delay, () =>
        {
            radars[1].transform.DOScale(new Vector3(1, 1, 0.2f), 2).OnComplete(() => {
                radars[1].transform.DOScale(new Vector3(0, 0, 0.2f), 0);
                });
        });
        DOVirtual.DelayedCall(delay * 2, () =>
        {
            radars[2].transform.DOScale(new Vector3(1, 1, 0.2f), 2).OnComplete(() => {
                radars[2].transform.DOScale(new Vector3(0, 0, 0.2f), 0);
                isOpen = false;
                
                });
        });
    }
}
