using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour
{
    public Material CapturedMat;
    public Material LairMat;
    Renderer rend;

    public Tile ParentTile;

    Color startColor;
    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    public void Capture()
    {
        rend.material = CapturedMat;
    }
    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
 
        if (ParentTile.Captured && BuildManager.isBuilding && !ParentTile.HasBuilding)
        {
            rend.material.color = Color.yellow;
        }
        if (ParentTile.Captured && BuildManager.isSelling && ParentTile.HasBuilding)
        {
            rend.material.color = Color.red;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rend.material.color = startColor;
        }
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0) && BuildManager.isBuilding && !ParentTile.HasBuilding)
        {
            ParentTile.HasBuilding = true;
            rend.material = LairMat;
            rend.material.color = startColor;
        }
        if (Input.GetMouseButtonDown(0) && BuildManager.isSelling && ParentTile.HasBuilding)
        {
            ParentTile.HasBuilding = false;
            rend.material = CapturedMat;
            rend.material.color = startColor;
        }
    }

    private void OnMouseExit()
    {
        if (ParentTile.Captured)
        {
            rend.material.color = startColor;
        }
    }

}
