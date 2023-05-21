using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobHandler : MonoBehaviour
{
    public static JobHandler Instance;
    public List<Creature> availableCreatures = new List<Creature>();
    public List<CreatueJob> availableJobs = new List<CreatueJob>();
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(availableCreatures.Count > 0 && availableJobs.Count > 0)
        {
            CreatueJob job = null;
            Creature creature = null;
            float shortestDistance = float.MaxValue;
            foreach (var creat in availableCreatures)
            {
                foreach (var jobbo in availableJobs)
                {
                    float distance = Vector3.Distance(jobbo.transform.position, creat.transform.position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        job = jobbo;
                        creature = creat;
                    }
                }
            }

            availableJobs.Remove(job);
            availableCreatures.Remove(creature);

            job.ChangeColor(Color.blue);
            creature.AssingJob(job);

        }
    }

    public void AssingJob(CreatueJob job)
    {
        if (!availableJobs.Contains(job))
            availableJobs.Add(job);
    }
    public void UnAssingJob(CreatueJob job)
    {
        if (availableJobs.Contains(job))
            availableJobs.Remove(job);
    }
    public void AssignCreature(Creature creature)
    {
        if (!availableCreatures.Contains(creature))
            availableCreatures.Add(creature);
    }
}
