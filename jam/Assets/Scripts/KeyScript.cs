﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyScript : MonoBehaviour
{
    public KeyCode keyCode;
    private float originalY;
    public float speed = 1;
    private KeyboardSoundPlayer keyboardSoundPlayer;

    public bool isOnRightSpot;
    public bool IsOnRightSpot => isOnRightSpot;

    public GameObject particleEffectPlop;

    public bool IsBroken;
    public bool IsRemoved => removed;
    private bool removed;
    private MeshRenderer renderer;
    private Canvas canvas;
    public Vector3 originalPos;
    public Vector3 beforeDepotPos;
    private bool isPlayerOnKey;

    static public UnityEvent KeyPutEvent = new UnityEvent();

    private void Awake()
    {
        originalPos = transform.position;
        originalY = transform.position.y;
        canvas = GetComponentInChildren<Canvas>();
        renderer = GetComponent<MeshRenderer>();
        keyboardSoundPlayer = GameObject.FindObjectOfType<KeyboardSoundPlayer>();
        isPlayerOnKey = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyCode))
        {
            if(keyCode == KeyCode.Space)
            {
                StartCoroutine(keyboardSoundPlayer.PlaySpacebarAudio());
            }
            else
            {
                StartCoroutine(keyboardSoundPlayer.PlayKeyAudioasd());
            }
            StartCoroutine(PressMove(originalY - 0.06f));
        }
        if(Input.GetKeyUp(keyCode))
        {
            if (isPlayerOnKey)
            {
                StartCoroutine(PressMove(originalY - 0.04f));
            }
            else
            {
                StartCoroutine(PressMove(originalY));
            }
        }
        CheckIfPlayerAboveKey();
    }

    private void CheckIfPlayerAboveKey()
    {
        if(Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitInfo, 2.0f))
        {
            var hitPlayer = hitInfo.transform.GetComponent<Player>();
            if (hitPlayer && !isPlayerOnKey)
            {
                isPlayerOnKey = true;
                StartCoroutine(keyboardSoundPlayer.PlayKeyAudioasd());
                StartCoroutine(PressMove(originalY - 0.04f));
            }
        }
        else if (isPlayerOnKey)
        {
            isPlayerOnKey = false;
            StartCoroutine(PressMove(originalY));
        }
    }

    public void ToDepot()
    {
        Debug.Log("ToDepot");
        beforeDepotPos = transform.position;
        IsBroken = true;
        GetComponent<BoxCollider>().enabled = false;
        renderer.enabled = true;
        canvas.gameObject.SetActive(true);
        removed = false;
    }

    public void FromDepot()
    {
        GetComponent<BoxCollider>().enabled = true;
        IsBroken = false;
        transform.position = beforeDepotPos;
    }

    public KeyCode Take()
    {
        renderer.enabled = false;
        canvas.gameObject.SetActive(false);
        removed = true;
        isOnRightSpot = false;
        KeyPutEvent.Invoke();
        return keyCode;
    }

    public void Put(Vector3? newPos = null)
    {
        if(newPos != null)
        {
            transform.position = newPos.Value;
        }
        canvas.gameObject.SetActive(true);
        removed = false;
        renderer.enabled = true;
        DoRightSpotCheck();
        //Debug.Log("Distance - " + Vector3.Distance(transform.position, originalPos));
        //Debug.Log(isOnRightSpot);
        SpawnParticleEffect(transform.position);
        KeyPutEvent.Invoke();
    }

    public void DoRightSpotCheck()
    {
        isOnRightSpot = Vector3.Distance(transform.position, originalPos) < 0.05f;
    }

    public IEnumerator PressMove(float targetY)
    {
        float startY = transform.position.y;
        float dir = targetY < transform.position.y ? -1 : 1;

        if(targetY < transform.position.y )
        {
            while(transform.position.y > targetY)
            {
                transform.position += transform.up * Time.deltaTime * speed * dir;
            }
        }
        else
        {
            while(transform.position.y < targetY)
            {
                transform.position += transform.up * Time.deltaTime * speed * dir;
            }
        }
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);

        yield return null;
    }

    private void SpawnParticleEffect(Vector3 pos)
    {
        var effect = Instantiate(particleEffectPlop, pos, Quaternion.identity);
        effect.GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(DestroyAfterDelay(effect));
    }

    private IEnumerator DestroyAfterDelay(GameObject particleToDestroy)
    {
        Debug.Log("test1");
        yield return new WaitForSeconds(1.5f);
        Destroy(particleToDestroy);
        Debug.Log("test2");
    }
}
