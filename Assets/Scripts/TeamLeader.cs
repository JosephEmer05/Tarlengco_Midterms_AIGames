using UnityEngine;
using System.Collections.Generic;

public class TeamLeader : MonoBehaviour
{
    public MinionUnit.Team teamType;
    public List<MinionUnit> followers = new List<MinionUnit>();

    private void OnTriggerEnter(Collider other)
    {
        MinionUnit unit = other.GetComponentInParent<MinionUnit>();

        if (unit != null)
        {
            if (unit.currentTeam == MinionUnit.Team.Neutral)
            {
                AddUnit(unit);
            }
        }

        TeamLeader otherLeader = other.GetComponentInParent<TeamLeader>();
        if (otherLeader != null && otherLeader != this)
        {
            if (otherLeader.teamType != this.teamType && this.followers.Count > otherLeader.followers.Count)
            {
                this.AbsorbTeam(otherLeader);
            }
        }
    }

    public void AddUnit(MinionUnit unit)
    {
        if (!followers.Contains(unit))
        {
            unit.JoinTeam(teamType, transform);
            followers.Add(unit);
        }
    }

    public void AbsorbTeam(TeamLeader loser)
    {
        foreach (MinionUnit unit in loser.followers)
        {
            unit.JoinTeam(teamType, transform);
            followers.Add(unit);
        }
        loser.followers.Clear();

        if (loser.teamType == MinionUnit.Team.Player)
        {
            GameManager.Instance.TriggerGameOver();
        }

        Destroy(loser.gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        SphereCollider sc = GetComponent<SphereCollider>();
        if (sc != null) Gizmos.DrawSphere(transform.position, sc.radius);
    }
}