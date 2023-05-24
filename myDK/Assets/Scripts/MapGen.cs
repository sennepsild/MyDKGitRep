using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGen : MonoBehaviour
{
    [SerializeField]
    GameObject dirtBlock;
    [SerializeField]
    int width, height;
    int carveSize = 3;

    public GameObject defaultCreat;
    public GameObject ExeJobPrefab;
    public GameObject CapJobPrefab;
    public GameObject CapWallJobPrefab;

    NavMeshSurface surface;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject map = new GameObject();
        map.name = "map";
        map.AddComponent<TileMap>();
        surface = map.AddComponent<NavMeshSurface>();
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.collectObjects = CollectObjects.Children;
        surface.BuildNavMesh();
        TileMap.instance.navSurface = surface;
        TileMap.instance.CreatureToSpawn = defaultCreat;
        TileMap.instance.ExeJobPrefab = ExeJobPrefab;
        TileMap.instance.CapJobPrefab = CapJobPrefab;
        TileMap.instance.CapWallJobPrefab = CapWallJobPrefab;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tempBlock = Instantiate(dirtBlock);
                tempBlock.transform.position = new Vector3(x, 0, y);
                tempBlock.transform.parent = map.transform;
            }
        }

    }


    
}
