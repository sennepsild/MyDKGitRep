using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Dirt : MonoBehaviour
{
    public Material CapturedMat;
    Tile parentTile;
    Renderer rend;
    void Start()
    {
        parentTile = transform.parent.GetComponent<Tile>();
        Vector3 pos = transform.position;
        
        rend = GetComponent<Renderer>();
    }

    public void Capture()
    {
        rend.material = CapturedMat;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (BuildManager.isBuilding) return;
        if (BuildManager.isSelling) return;
        if (!parentTile.Marked)
        {
            rend.material.color = Color.yellow;
        }
        else
        {
            rend.material.color = Color.Lerp( Color.yellow,Color.blue,.5f);
        }
        
    }

    private void OnMouseOver()
    {
        if (BuildManager.isBuilding) return;
        if (BuildManager.isSelling) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))
        {


            parentTile.Marked = !parentTile.Marked;
            if (parentTile.Marked)
            {
                TileMap.instance.GenerateDigginJobsAtNeigbour(parentTile.GetMapPos());
                TileMap.instance.RemoveReinforceJobsAtNeigbour(parentTile.GetMapPos());
                rend.material.color = Color.Lerp(Color.yellow, Color.blue, .5f);
            }
            else
            {
                TileMap.instance.RemoveDigginJobsAtNeigbour(parentTile.GetMapPos());
                TileMap.instance.GenerateReinforceJobsAtNeigbour(parentTile.GetMapPos());
                rend.material.color = Color.yellow;
            }
        }
    }
    private void OnMouseExit()
    {
        if (!parentTile.Marked)
        {
            rend.material.color = Color.white;
        }
        else
        {
            rend.material.color = Color.blue;
        }
    }

}
