using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFighterSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float lastSpawnTime = 0f;
    public float spawnPeriod = 5f;

    void Update()
    {
        if (Time.time > lastSpawnTime + spawnPeriod)
        {
            Vector2 movementDirection = new Vector2(
                Random.Range(-1000000, 1000000), Random.Range(-1000000, 1000000)).normalized;
            Vector2 position = -17.5f * movementDirection + new Vector2(0, Random.Range(-10, 10));
            EnemyControllerFighter.SpawnAndInstantiate(prefab, position, movementDirection);
            lastSpawnTime = Time.time;
        }
    }
}
