using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tile : MonoBehaviour
{
    public bool HasBlock = true;
    public GameObject Block;

    public bool Marked;
    public bool Captured;
    public bool Reinforced;

    public bool HasBuilding;

    public Ground Floor;
    public Dirt DirtBlock;

    public float ReinForcedHealth = 400;

    public List<CreatueJob> Jobs = new List<CreatueJob>();

    public float Health = 100;

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if(Health <= 0)
        {
            TileMap.instance.DestroyDirtAt(GetMapPos());
        }
    }

    public void GetHealth(float amount)
    {
        Health += amount;

        if (Health >= ReinForcedHealth)
        {
            Reinforced = true;
            Health = ReinForcedHealth;
            TileMap.instance.ReinForceDirtAt(GetMapPos());
        }

    }

    private void Start()
    {
        TileMap.instance.Tiles.Add(GetMapPos(), this);
    }

    public Vector2 GetMapPos()
    {
        return new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.z));
    }
}
