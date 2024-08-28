using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;

public class MobSpawnerHandler : MonoBehaviour
{
    [SerializeField, Range(0, 1000)] float closeSpawnRange;
    [SerializeField] List<SpawnRegion> regions = new();
    [Header("Add Collection")]
    [SerializeField] bool addCollection;
    [SerializeField] GameObject collection;

    [Space(3), Header("Other")]
    [SerializeField] Transform player;
    [SerializeField] bool debugPointNames;

    [Serializable]
    public class SpawnRegion
    {
        public string name = "*New Region*";
        public Color color = Color.green;
        public MobSpawnpoint[] spawnPoints = new MobSpawnpoint[0];
    }

    GameObject[] nearPoints;
    private void OnValidate()
    {
        if (addCollection)
        {
            addCollection = false;
            MobSpawnpoint[] points = collection.GetComponentsInChildren<MobSpawnpoint>();
            List<SpawnRegion> findRegion = regions.Where(p => p.name == collection.name).ToList();
            Debug.Log(findRegion.Count());
            if (findRegion.Count != 0)
                regions.Find(p => p.name == collection.name).spawnPoints.AddRange(points);
            else
            {
                SpawnRegion region = new();
                region.spawnPoints = points;
                regions.Add(region);
            }
            collection = new();
        }

        if (player == null) return;

        spawnPoints.Clear();
        foreach (var region in regions)
        {
            foreach (var point in region.spawnPoints)
            {
                if (point == null) continue;
                if (!point.TryGetComponent(out MobSpawnpoint ms)) continue;
                ms.color = region.color;
                spawnPoints.Add(ms);
            }
        }
        nearPoints = GetSpawnpoints(player);

        if (!debugPointNames) return;
        foreach (var point in nearPoints)
        {
            float distance = Vector3.Distance(player.position, point.transform.position);
            Debug.Log(point.name + " - " + distance);
        }
    }

    List<MobSpawnpoint> spawnPoints = new();
    public GameObject[] GetSpawnpoints(Transform playerTransform)
    {
        Vector3 playerPosition = playerTransform.position;
        Dictionary<GameObject, float> viableSpawnPoints = new();
        foreach (MobSpawnpoint spawnPoint in spawnPoints)
        {
            float distance = Vector3.Distance(playerPosition, spawnPoint.transform.position);
            if (distance < closeSpawnRange) continue;
            viableSpawnPoints.TryAdd(spawnPoint.gameObject, distance);
        }
        GameObject[] orderedList = viableSpawnPoints.Keys.OrderBy(unit => viableSpawnPoints[unit]).ToArray();
        return orderedList;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Vector3 playerPosition = player.transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerPosition, closeSpawnRange);

        if (nearPoints.Length == 0) return;
        foreach (var point in nearPoints)
        {
            Gizmos.color = point.GetComponent<MobSpawnpoint>().color;
            Gizmos.DrawLine(point.transform.position, player.position);
        }
    }
}
