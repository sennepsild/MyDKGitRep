using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CreatueJob : MonoBehaviour
{
    public Creature AssingedCreature;
    public Tile TargetTile;
    public float Priority;
    Renderer rend;

    public enum JobType { Excavating, Capture, ReinforceWall};

    public JobType jobType;
   
    private void Start()
    {
        rend = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        JobHandler.Instance.AssingJob(this);
    }
    public void ChangeColor(Color color)
    {
        rend.material.color = color;
    }

    public void RemoveJob()
    {
        JobHandler.Instance.UnAssingJob(this);
        if (AssingedCreature != null)
        {
            AssingedCreature.StopCurrentJob();
        }
        Destroy(gameObject);
    }
}
