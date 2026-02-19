using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MinionUnit : MonoBehaviour
{
    public enum Team { Neutral, Player, Red, Green }
    public Team currentTeam = Team.Neutral;
    public Transform leader;

    private NavMeshAgent m_Agent;
    private MeshRenderer m_Renderer;
    private Vector3 m_FollowOffset;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Renderer = GetComponent<MeshRenderer>();
        m_FollowOffset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-3f, -1.5f));

        if (currentTeam == Team.Neutral) StartCoroutine(Roam());
    }

    void Update()
    {
        if (currentTeam != Team.Neutral && leader != null)
        {
            Vector3 targetPos = leader.TransformPoint(m_FollowOffset);
            m_Agent.SetDestination(targetPos);
            m_Agent.stoppingDistance = 0.5f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentTeam == Team.Neutral) return;

        if (other.TryGetComponent<MinionUnit>(out MinionUnit otherUnit))
        {
            if (otherUnit.currentTeam == Team.Neutral)
            {
                TeamLeader leaderScript = leader.GetComponent<TeamLeader>();
                if (leaderScript != null)
                {
                    leaderScript.AddUnit(otherUnit);
                }
            }
        }
    }

    IEnumerator Roam()
    {
        while (currentTeam == Team.Neutral)
        {
            Vector3 randomDest = transform.position + Random.insideUnitSphere * 10f;
            if (NavMesh.SamplePosition(randomDest, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                m_Agent.SetDestination(hit.position);
            yield return new WaitForSeconds(Random.Range(3, 6));
        }
    }

    public void JoinTeam(Team newTeam, Transform newLeader)
    {
        currentTeam = newTeam;
        leader = newLeader;
        StopAllCoroutines();

        if (newTeam == Team.Player) m_Renderer.material.color = Color.blue;
        else if (newTeam == Team.Red) m_Renderer.material.color = Color.red;
        else if (newTeam == Team.Green) m_Renderer.material.color = Color.green;
    }
}