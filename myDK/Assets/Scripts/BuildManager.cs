using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static bool isBuilding;
    public static bool isSelling;

    private void Update()
    {
        if ( Input.GetMouseButtonDown(1))
        {
            isBuilding = false;
            isSelling = false;
        }
    }
    public void StartBuilding()
    {
        isBuilding = true;
        isSelling = false;
    }
    public void StartSelling()
    {
        isSelling = true;
        isBuilding = false;
    }
}
