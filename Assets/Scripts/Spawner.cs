using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    public GameObject minionPrefab;
    public int totalNeutralUnits = 20;
    public float spawnRadius = 30f;

    void Start()
    {
        for (int i = 0; i < totalNeutralUnits; i++)
        {
            SpawnUnit();
        }
    }

    void SpawnUnit()
    {
        Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
        randomPos += transform.position;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
        {
            Instantiate(minionPrefab, hit.position, Quaternion.identity);
        }
    }
}