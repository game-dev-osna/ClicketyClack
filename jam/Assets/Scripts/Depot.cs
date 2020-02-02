using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depot : MonoBehaviour
{
    public KeyScript depotKey;
    public Queue<KeyScript> keys = new Queue<KeyScript>();
    public GameObject plopEffect;

    private KeyScript toPickup;
    private float timer;
    private KeyboardInput keyInputs;

    private void Awake()
    {
        keyInputs = GameObject.FindObjectOfType<KeyboardInput>();
        depotKey.GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        if(keys.Count > 0 && toPickup == null)
        {
            DequeKey();
        }
    }

    private void DequeKey()
    {
        toPickup = keys.Dequeue();
        toPickup.gameObject.SetActive(true);
        toPickup.transform.position = depotKey.transform.position;
        UpdateQueueVisualisation(null, toPickup);
    }

    public void PlaceInDepot(KeyScript keyScript)
    {
        keyScript.ToDepot();
        //keyScript.gameObject.SetActive(false);
        keys.Enqueue(keyScript);
        if (toPickup)
        {
            UpdateQueueVisualisation(keyScript);
        }
    }

    public KeyScript TakeFromDepot()
    {
        var temp = toPickup;
        toPickup = null;
        if(temp) {
            temp.FromDepot();
        }
        return temp;
    }

    public void UpdateQueueVisualisation(KeyScript newKey = null, KeyScript dequeueKey = null)
    {
        int index = 1;
        foreach (var key in keys)
        {
            key.gameObject.SetActive(true);
            key.transform.position = depotKey.transform.position + (Vector3.right * (0.45f * index));
            index++;
        }
        if (newKey)
        {
            Debug.Log(newKey.transform.position);
            SpawnParticleEffect(newKey.transform.position);
        }
        if (dequeueKey)
        {
            Debug.Log(dequeueKey.transform.position);
            SpawnParticleEffect(dequeueKey.transform.position);
        }
    }
    private void SpawnParticleEffect(Vector3 pos)
    {
        var effect = Instantiate(plopEffect, pos, Quaternion.identity);
        effect.GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(DestroyAfterDelay(effect));
    }

    private IEnumerator DestroyAfterDelay(GameObject particleToDestroy)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(particleToDestroy);
    }
}
