using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{

    public LayerMask mask;

    Vector3 CamStart;
    Vector3 starDrag;
    void Update()
    {
        Camera.main.transform.position += Camera.main.transform.forward * Input.mouseScrollDelta.y;

        if (Input.GetMouseButtonDown(2))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 20, mask))
            {
                starDrag = hit.point;
                CamStart = Camera.main.transform.position;
            }
        }
        if (Input.GetMouseButton(2))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 20, mask))
            {
                Vector3 diff =  starDrag - hit.point;
                diff.y = 0;
                Camera.main.transform.position += diff;
            }
        }
    }
}
