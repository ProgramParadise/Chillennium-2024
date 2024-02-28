using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    public float spawnInterval = 3f;
    public float maxSpawnDistance = 2f;
    int currentCount = 0;
    public int maxCount = 5;
    bool isSpawning = false;

    // Update is called once per frame
    void Update()
    {
        // Spawn infintely if maxCount is 0
        if (!isSpawning && (currentCount < maxCount || maxCount == 0))
        {
            StartCoroutine(SpawnPrefab());
        }
    }

    IEnumerator SpawnPrefab()
    {
        isSpawning = true;
        currentCount++;
        yield return new WaitForSeconds(spawnInterval);
        Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-maxSpawnDistance, maxSpawnDistance), 0, Random.Range(-maxSpawnDistance, maxSpawnDistance));
        Instantiate(prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)], spawnPosition, Quaternion.identity, transform);
        isSpawning = false;
    }
}
