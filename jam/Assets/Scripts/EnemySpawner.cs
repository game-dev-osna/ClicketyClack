using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();


    public bool ShouldSpawn;
    public float interval;
    public float intervalSigma;
    private float timer;

    void Start()
    {
        timer = 0;
    }

    void Update()
    {
        if(!ShouldSpawn) return;
        if(timer <= 0)
        {
            var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Instantiate(prefab);
            timer = interval + Random.Range(-intervalSigma, intervalSigma);
        }
        else
        {
            timer -= Time.deltaTime; 
        }
    }
}
