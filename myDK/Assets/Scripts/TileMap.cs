using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TileMap : MonoBehaviour
{
    public static TileMap instance;
    public Dictionary<Vector2, Tile> Tiles = new Dictionary<Vector2, Tile>();

    public List<Tile> CapturedTiles = new List<Tile>();

    public NavMeshSurface navSurface;

    public GameObject ExeJobPrefab;
    public GameObject CapJobPrefab;
    public GameObject CapWallJobPrefab;
    public GameObject CreatureToSpawn;

    Vector2[] directions =
        {
            Vector2.left,
            Vector2.up,
            Vector2.right,
            Vector2.down
        };
    private void Awake()
    {
        instance = this;
    }

    public void SpawnCreatureAt(Vector2 pos)
    {
        Instantiate(CreatureToSpawn, Tiles[pos].transform.position, Quaternion.identity);
    }
    public void DestroyDirtAt(Vector2 pos)
    {
        
        Tiles[pos].HasBlock = false;
        Tiles[pos].Block.SetActive ( false);
        GenerateDiggingJobAt(pos);
        GenerateCaptureJobAt(pos);
        RemoveDigginJobsAtNeigbour(pos);
        RemoveReinforceJobsAtNeigbour(pos);
        StartCoroutine(UpdateNavMeshWithDelay(navSurface));
    }

    public void ReinForceDirtAt(Vector2 pos, bool CornerCall = false)
    {
        if (!Tiles[pos].HasBlock) return;
        Tiles[pos].DirtBlock.Capture();
        RemoveReinforceJobsAtNeigbour(pos);
        if(!CornerCall)
        ReinforceCorner(pos);
    }

    public void ReinforceCorner(Vector2 pos)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if(Tiles.ContainsKey(pos + directions[i]))
            {
                if (!TileIsAccessable(pos + directions[i]) && TileIsHas2ReinforecedNeigbours(pos + directions[i]) && TileHasAccessDiagonal(pos + directions[i]))
                {
                    Tiles[pos + directions[i]].Reinforced = true;
                    Tiles[pos + directions[i]].Health = Tiles[pos + directions[i]].ReinForcedHealth;
                    ReinForceDirtAt(pos + directions[i],true);
                }
            }
        }
    }
    public bool TileIsHas2ReinforecedNeigbours(Vector2 pos)
    {
        int amount = 0;
        for (int i = 0; i < directions.Length; i++)
        {
            if (Tiles.ContainsKey(pos + directions[i]) && Tiles[pos + directions[i]].Reinforced)
            {
                amount++;
                if (amount == 2)
                    return true;
            }
        }
        
        return false;
    }

    public bool TileHasAccessDiagonal(Vector2 pos)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (Tiles.ContainsKey(pos + directions[i] + directions[(i+1)%4]) && !Tiles[pos + directions[i] + directions[(i + 1) % 4]].HasBlock)
            {
                return true;
            }
        }
        return false;
    }

    public bool TileIsAccessable(Vector2 pos)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (Tiles.ContainsKey(pos + directions[i]) && !Tiles[pos+directions[i]].HasBlock)
            {
                return true;
            }
        }
        return false;
    }

    public Tile GetRandomCapturedTile()
    {
        return CapturedTiles[Random.Range(0, CapturedTiles.Count)];
    }

    public void CaptureTileAt(Vector2 pos)
    {
        if (Tiles[pos].Captured) return;
        Tiles[pos].Captured = true;
        Tiles[pos].Floor.Capture();
        CapturedTiles.Add(Tiles[pos]);
        GenerateCaptureJobsAtNeigbour(pos);
        GenerateReinforceJobAt(pos);
    }

    public void GenerateDigginJobsAtNeigbour(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        for (int i = 0; i < directions.Length; i++)
        {
            GenerateDiggingJobAt(pos + directions[i]);
        }
    }
    public void GenerateCaptureJobsAtNeigbour(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        for (int i = 0; i < directions.Length; i++)
        {
            GenerateCaptureJobAt(pos + directions[i]);
        }
    }
    public void RemoveDigginJobsAtNeigbour(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        for (int i = 0; i < directions.Length; i++)
        {
            RemoveDigginJobsAt(pos + directions[i]);
        }
    }
    public void RemoveReinforceJobsAtNeigbour(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        for (int i = 0; i < directions.Length; i++)
        {
            RemoveReinforceJobsAt(pos + directions[i]);
        }
    }
    public void GenerateReinforceJobsAtNeigbour(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        for (int i = 0; i < directions.Length; i++)
        {
            GenerateReinforceJobAt(pos + directions[i]);
        }
    }

    public void RemoveDigginJobsAt(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        for (int i = 0; i < directions.Length; i++)
        {
            if (Tiles.ContainsKey(pos + directions[i]))
            {
                if (!Tiles[pos + directions[i]].Marked || !Tiles[pos + directions[i]].HasBlock)
                {
                    int compare = Tiles[pos].Jobs.Count;
                    for (int j = 0; j < compare; j++)
                    {
                        if (Tiles[pos].Jobs[j].TargetTile == Tiles[pos + directions[i]] && Tiles[pos].Jobs[j].jobType == CreatueJob.JobType.Excavating)
                        {
                            CreatueJob obj = Tiles[pos].Jobs[j];
                            Tiles[pos].Jobs.RemoveAt(j);
                            j--;
                            compare--;
                            obj.RemoveJob();
                        }
                    }

                }
            }
        }
    }

    public void RemoveReinforceJobsAt(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        for (int i = 0; i < directions.Length; i++)
        {
            if (Tiles.ContainsKey(pos + directions[i]))
            {
                if (Tiles[pos + directions[i]].Marked || !Tiles[pos + directions[i]].HasBlock || Tiles[pos + directions[i]].Reinforced)
                {
                    int compare = Tiles[pos].Jobs.Count;
                    for (int j = 0; j < compare; j++)
                    {
                        if (Tiles[pos].Jobs[j].TargetTile == Tiles[pos + directions[i]] && Tiles[pos].Jobs[j].jobType == CreatueJob.JobType.ReinforceWall)
                        {
                            CreatueJob obj = Tiles[pos].Jobs[j];
                            Tiles[pos].Jobs.RemoveAt(j);
                            j--;
                            compare--;
                            obj.RemoveJob();
                        }
                    }

                }
            }
        }
    }

    public void RemoveCaptureJobAt(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        for (int i = 0; i < Tiles[pos].Jobs.Count; i++)
        {
            if(Tiles[pos].Jobs[i].jobType == CreatueJob.JobType.Capture)
            {
                CreatueJob obj = Tiles[pos].Jobs[i];
                Tiles[pos].Jobs.RemoveAt(i);
                obj.RemoveJob();
                return;
            }

        }
    }
    public void GenerateCaptureJobAt(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        if (Tiles[pos].Captured) return;
        if (Tiles[pos].HasBlock) return;


        for (int i = 0; i < Tiles[pos].Jobs.Count; i++)
        {
            if (Tiles[pos].Jobs[i].jobType == CreatueJob.JobType.Capture)
            {
                return;
            }
        }

        for (int i = 0; i < directions.Length; i++)
        {
            if (Tiles.ContainsKey(pos + directions[i]))
            {
                if (Tiles[pos + directions[i]].Captured)
                {
                    GameObject job = Instantiate(CapJobPrefab, Tiles[pos].transform.position , Quaternion.identity);
                    job.GetComponent<CreatueJob>().TargetTile = Tiles[pos];
                    Tiles[pos].Jobs.Add(job.GetComponent<CreatueJob>());
                    return;
                }
            }
        }
    }

    public void GenerateDiggingJobAt(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        if (Tiles[pos].HasBlock) return;
        for (int i = 0; i < directions.Length; i++)
        {
            if (Tiles.ContainsKey(pos + directions[i]))
            {
                if (Tiles[pos + directions[i]].Marked && Tiles[pos + directions[i]].HasBlock)
                {
                    Vector3 posOffset = new Vector3(directions[i].x, 0, directions[i].y);
                    posOffset *= 0.3f;
                    Vector3 posOffsetSideways = Quaternion.Euler(0, -90, 0) * posOffset;

                    for (int j = 0; j < 3; j++)
                    {
                        GameObject job = Instantiate(ExeJobPrefab, Tiles[pos].transform.position + posOffset + (posOffsetSideways*j + posOffsetSideways * -1), Quaternion.identity);
                        job.GetComponent<CreatueJob>().TargetTile = Tiles[pos + directions[i]];
                        Tiles[pos].Jobs.Add(job.GetComponent<CreatueJob>());
                    }
                    
                }
            }
        }
    }

    public void GenerateReinforceJobAt(Vector2 pos)
    {
        if (!Tiles.ContainsKey(pos)) return;
        if (Tiles[pos].HasBlock) return;
        if (!Tiles[pos].Captured) return;
        for (int i = 0; i < directions.Length; i++)
        {
            if (Tiles.ContainsKey(pos + directions[i]))
            {
                if (!Tiles[pos + directions[i]].Marked && Tiles[pos + directions[i]].HasBlock && !Tiles[pos + directions[i]].Reinforced)
                {
                    Vector3 posOffset = new Vector3(directions[i].x, 0, directions[i].y);
                    posOffset *= 0.3f;

                    GameObject job = Instantiate(CapWallJobPrefab, Tiles[pos].transform.position + posOffset, Quaternion.identity);
                    job.GetComponent<CreatueJob>().TargetTile = Tiles[pos + directions[i]];
                    Tiles[pos].Jobs.Add(job.GetComponent<CreatueJob>());
                }
            }
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyDirtAt(new Vector2(15, 10));
            DestroyDirtAt(new Vector2(14, 10));
            DestroyDirtAt(new Vector2(13, 10));
            DestroyDirtAt(new Vector2(14, 9));
            DestroyDirtAt(new Vector2(15, 9));
            CaptureTileAt(new Vector2(14, 10));
            DestroyDirtAt(new Vector2(13, 9));
            DestroyDirtAt(new Vector2(14, 11));
            DestroyDirtAt(new Vector2(15, 11));
            DestroyDirtAt(new Vector2(13, 11));
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnCreatureAt(new Vector2(15, 10));
        }
        
    }

    private IEnumerator UpdateNavMeshWithDelay(NavMeshSurface surf)
    {
        yield return new WaitForSeconds(0.1f); // Delay of 1 second

        surf.UpdateNavMesh(surf.navMeshData);
    }

}
