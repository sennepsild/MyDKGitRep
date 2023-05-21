using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dirt : MonoBehaviour
{
    Tile parentTile;
    Renderer rend;
    void Start()
    {
        parentTile = transform.parent.GetComponent<Tile>();
        Vector3 pos = transform.position;
        
        rend = GetComponent<Renderer>();
    }

    

    private void OnMouseEnter()
    {
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
        if (Input.GetMouseButtonDown(0))
        {


            parentTile.Marked = !parentTile.Marked;
            if (parentTile.Marked)
            {
                TileMap.instance.GenerateDigginJobsAtNeigbour(parentTile.GetMapPos());
                rend.material.color = Color.Lerp(Color.yellow, Color.blue, .5f);
            }
            else
            {
                TileMap.instance.RemoveDigginJobsAtNeigbour(parentTile.GetMapPos());
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
