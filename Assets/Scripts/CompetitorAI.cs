using UnityEngine;
using UnityEngine.AI;

public class CompetitorAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private TeamLeader myLeaderData;
    private float scanTimer;

    [Header("AI Settings")]
    public float scanInterval = 0.3f;
    public float detectionRadius = 40f;
    public float fleeRadius = 15f;
    public float attackMargin = 2f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myLeaderData = GetComponent<TeamLeader>();
    }

    void Update()
    {
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)
        {
            DecideBehavior();
            scanTimer = 0;
        }
    }

    void DecideBehavior()
    {
        TeamLeader threat = FindNearestThreat();

        if (threat != null)
        {
            FleeFrom(threat.transform.position);
            return;
        }

        TeamLeader target = FindTargetLeader();

        if (target != null)
        {
            agent.SetDestination(target.transform.position);
            agent.stoppingDistance = 0f;
            return;
        }

        FindNearestNeutral();
    }

    TeamLeader FindNearestThreat()
    {
        TeamLeader[] allLeaders = Object.FindObjectsByType<TeamLeader>(FindObjectsSortMode.None);
        TeamLeader biggestThreat = null;
        float closestDist = Mathf.Infinity;

        foreach (TeamLeader other in allLeaders)
        {
            if (other == myLeaderData) continue;

            if (other.followers.Count > myLeaderData.followers.Count)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (dist < fleeRadius && dist < closestDist)
                {
                    closestDist = dist;
                    biggestThreat = other;
                }
            }
        }
        return biggestThreat;
    }

    void FleeFrom(Vector3 threatPosition)
    {
        Vector3 runDirection = (transform.position - threatPosition).normalized;
        Vector3 runTarget = transform.position + runDirection * 10f;

        if (NavMesh.SamplePosition(runTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    TeamLeader FindTargetLeader()
    {
        TeamLeader[] allLeaders = Object.FindObjectsByType<TeamLeader>(FindObjectsSortMode.None);
        TeamLeader bestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (TeamLeader other in allLeaders)
        {
            if (other == myLeaderData) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            bool isWeaker = (myLeaderData.followers.Count > other.followers.Count + attackMargin);

            if (isWeaker && dist <= detectionRadius)
            {
                if (dist < closestDist)
                {
                    closestDist = dist;
                    bestTarget = other;
                }
            }
        }
        return bestTarget;
    }

    void FindNearestNeutral()
    {
        MinionUnit[] allMinions = Object.FindObjectsByType<MinionUnit>(FindObjectsSortMode.None);
        MinionUnit closest = null;
        float closestDist = Mathf.Infinity;

        foreach (MinionUnit unit in allMinions)
        {
            if (unit.currentTeam == MinionUnit.Team.Neutral)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist < closestDist && dist <= detectionRadius)
                {
                    closestDist = dist;
                    closest = unit;
                }
            }
        }

        if (closest != null) agent.SetDestination(closest.transform.position);
        else if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 roamPos = transform.position + Random.insideUnitSphere * 15f;
            if (NavMesh.SamplePosition(roamPos, out NavMeshHit hit, 15f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }
    }
}