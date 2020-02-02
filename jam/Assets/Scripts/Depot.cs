using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depot : MonoBehaviour
{
    public KeyScript depotKey;
    public Queue<KeyScript> keys = new Queue<KeyScript>();

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
    }

    public void PlaceInDepot(KeyScript keyScript)
    {
        keyScript.ToDepot();
        keyScript.gameObject.SetActive(false);
        keys.Enqueue(keyScript);
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

    /*public void UpdateQueueVisualisation()
    {
        int index = 1;
        foreach (var key in keys)
        {
            key.gameObject.SetActive(true);
            key.transform.position = depotKey.transform.position;
            key.transform.position
            index++;
        }
    }*/
}
