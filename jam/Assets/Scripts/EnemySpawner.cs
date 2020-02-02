using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();


    public float interval;
    public float intervalSigma;
    private float timer;

    void Start()
    {
        
    }

    void Update()
    {
        if(timer <= 0)
        {
            //enemyPrefabs.
        }
    }
}
