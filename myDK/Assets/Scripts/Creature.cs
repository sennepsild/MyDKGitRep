using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Creature : MonoBehaviour
{

    public Animator anim;

    NavMeshAgent agent;


    CreatueJob targetJob;

    public float DecisionTimeMax, DecisionTimeMin;

    bool WalkingToJob = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        JobHandler.Instance.AssignCreature(this);
    }
    private void Update()
    {
        if (WalkingToJob)
        {
            if(agent.remainingDistance == 0)
            {
                WalkingToJob = false;
                if(targetJob.jobType == CreatueJob.JobType.Excavating)
                    StartCoroutine(DecideToDigBlock());
                else if (targetJob.jobType == CreatueJob.JobType.Capture)
                    StartCoroutine(DecideToCaptureTile());
            }
        }
        
    }

    IEnumerator DecideToCaptureTile()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(DecisionTimeMin, DecisionTimeMax));
        StartCoroutine(CaptureTile());
    }

    IEnumerator CaptureTile()
    {

        anim.SetTrigger("Work");
        yield return new WaitForSeconds(2);
        TileMap.instance.CaptureTileAt(targetJob.TargetTile.GetMapPos());
        targetJob.RemoveJob();
    }

    IEnumerator AttackBlock()
    {
        while (targetJob != null)
        {
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.83333f);
            targetJob.TargetTile.TakeDamage(35);
            yield return new WaitForSeconds(1.2f);
        }
    }
    IEnumerator DecideToWalkToJob()
    {
        yield return new WaitForSeconds(Random.Range(DecisionTimeMin,DecisionTimeMax));
        agent.isStopped = false;
        agent.SetDestination(targetJob.transform.position);
        WalkingToJob = true;
    }
    IEnumerator DecideToDigBlock()
    {
        agent.isStopped = true;
        transform.LookAt(targetJob.TargetTile.transform);
        yield return new WaitForSeconds(Random.Range(DecisionTimeMin, DecisionTimeMax));
        StartCoroutine(AttackBlock());
    }


    public void StopCurrentJob()
    {
        anim.SetTrigger("Stop");
        targetJob = null;
        WalkingToJob = false;
        agent.isStopped = true;
        StopAllCoroutines();
        JobHandler.Instance.AssignCreature(this);
        
    }

    public void AssingJob(CreatueJob job)
    {
        targetJob = job;
        targetJob.AssingedCreature = this;
        StartCoroutine(DecideToWalkToJob());
    }
}
