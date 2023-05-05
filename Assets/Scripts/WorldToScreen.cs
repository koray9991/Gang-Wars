using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldToScreen : MonoBehaviour
{
    public Camera cam;

    public Transform target;

    public RectTransform PointerRectTransform;

    public TextMeshProUGUI countText;

    public Ai ai;

    public float index;

    void Update()
    {
        if (target != null)
        {
            float depth = cam.transform.InverseTransformPoint(target.position).z;

            if (depth > 0)
            {
                PointerRectTransform.position = new Vector3(cam.WorldToScreenPoint(target.position).x, cam.WorldToScreenPoint(target.position).y, 0);
            }
            else
            {
                PointerRectTransform.position = new Vector3(cam.WorldToScreenPoint(target.position).x, cam.WorldToScreenPoint(target.position).y, 0) * -10000;
            }


            PointerRectTransform.gameObject.SetActive(false);

            if (PointerRectTransform.position.x >= Screen.width - index)
            {
                PointerRectTransform.position = new Vector3(Screen.width - index, PointerRectTransform.position.y, PointerRectTransform.position.z);
                PointerRectTransform.gameObject.SetActive(true);
            }
            if (PointerRectTransform.position.x <= index)
            {
                PointerRectTransform.position = new Vector3(index, PointerRectTransform.position.y, PointerRectTransform.position.z);
                PointerRectTransform.gameObject.SetActive(true);
            }
            if (PointerRectTransform.position.y <= index)
            {
                PointerRectTransform.position = new Vector3(PointerRectTransform.position.x, index, PointerRectTransform.position.z);
                PointerRectTransform.gameObject.SetActive(true);
            }
            if (PointerRectTransform.position.y >= Screen.height - index)
            {
                PointerRectTransform.position = new Vector3(PointerRectTransform.position.x, Screen.height - index, PointerRectTransform.position.z);
                PointerRectTransform.gameObject.SetActive(true);
            }
            countText.text = ai.power.ToString();
        }
      
    }
}
