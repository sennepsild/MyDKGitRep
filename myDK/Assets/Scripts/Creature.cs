using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Creature : MonoBehaviour
{
    public bool CanWork;
    public Animator anim;

    NavMeshAgent agent;


    CreatueJob targetJob;

    public float DecisionTimeMax, DecisionTimeMin;

    bool WalkingToJob = false;

    bool walkingSomewhere = false;

    Coroutine DecidingToWalkSomewhere;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(CanWork)
            JobHandler.Instance.AssignCreature(this);

    }
    private void Update()
    {
        if (WalkingToJob && !walkingSomewhere)
        {
            if(agent.remainingDistance == 0)
            {
                WalkingToJob = false;
                if(targetJob.jobType == CreatueJob.JobType.Excavating)
                    StartCoroutine(DecideToDigBlock());
                else if (targetJob.jobType == CreatueJob.JobType.Capture)
                    StartCoroutine(DecideToCaptureTile());
                else if (targetJob.jobType == CreatueJob.JobType.ReinforceWall)
                    StartCoroutine(DecideToReinforceWall());
            }
        }
        anim.SetBool("Running", agent.velocity.magnitude != 0);
    }
    void LateUpdate()
    {
        if (targetJob == null && !walkingSomewhere)
        {
            DecidingToWalkSomewhere = StartCoroutine(DecideToWalkSomewhere());
        }
    }



    IEnumerator DecideToReinforceWall()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(DecisionTimeMin, DecisionTimeMax));
        StartCoroutine(ReinforceWall());
    }

    IEnumerator DecideToCaptureTile()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(DecisionTimeMin, DecisionTimeMax));
        StartCoroutine(CaptureTile());
    }
    IEnumerator ReinforceWall()
    {
        transform.LookAt(targetJob.TargetTile.transform);
        anim.SetTrigger("Reinforce");
        while (targetJob != null )
        {
            yield return new WaitForSeconds(.5f);
            targetJob.TargetTile.GetHealth(50);
        }
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
        walkingSomewhere = false;
        yield return new WaitForSeconds(Random.Range(DecisionTimeMin,DecisionTimeMax));
        agent.isStopped = false;
        agent.SetDestination(targetJob.transform.position);
        WalkingToJob = true;
    }

    IEnumerator DecideToWalkSomewhere()
    {
        walkingSomewhere = true;
        while (walkingSomewhere)
        {
            yield return new WaitForSeconds(Random.Range(DecisionTimeMax * 5, DecisionTimeMax * 20));
            agent.isStopped = false;
            Vector3 somewhere = TileMap.instance.GetRandomCapturedTile().transform.position;
            somewhere.x += Random.Range(-.5f, .5f);
            somewhere.z += Random.Range(-.5f, .5f);
            agent.SetDestination(somewhere);
            yield return new WaitForSeconds(1f);
            while (agent.remainingDistance != 0)
            {
                yield return new WaitForSeconds(1f);
            }
        }
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
        walkingSomewhere = false;
        agent.isStopped = true;
        StopAllCoroutines();
        if (CanWork)
            JobHandler.Instance.AssignCreature(this);
    }

    public void AssingJob(CreatueJob job)
    {
        walkingSomewhere = false;
        StopCoroutine(DecidingToWalkSomewhere);
        targetJob = job;
        targetJob.AssingedCreature = this;
        StartCoroutine(DecideToWalkToJob());
    }
}
