using UnityEngine;
using System.Collections;

public class ClearSight : MonoBehaviour
{
    [Header("player transfrom")]
    public Transform transform_player;

    [Header("Detection distance")]
    public float detection_diatance;

    [Header("alpha")]
    public float alpha;

    void Update()
    {
        if (transform_player != null)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(this.transform.position, (this.transform_player.position - transform.position).normalized, detection_diatance);
            Debug.DrawRay(transform.position, (this.transform_player.position - transform.position).normalized * detection_diatance, Color.red);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<Destroyable>() != null)
                {
                    if (hit.transform.GetComponent<Destroyable>().isHome)
                    {
                        Renderer R = hit.collider.transform.GetChild(0).GetComponent<Renderer>();
                        if (R == null)
                            return;
                        AutoTransparent AT = R.GetComponent<AutoTransparent>();

                        if (AT == null)// if no script is attached, attach one
                        {
                            AT = R.gameObject.AddComponent<AutoTransparent>();
                        }
                        AT.set_obj_transparent_and_change_gamelayer(alpha);
                    }

                }


            }
        }
      
    }
}

