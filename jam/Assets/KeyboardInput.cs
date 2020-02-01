using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputEvent : UnityEvent<List<KeyCode>>{}
public class KeyboardInput : MonoBehaviour
{
    public GameObject keyboardParent;
    public List<KeyCode> controllKeys = new List<KeyCode>();
    public List<KeyCode> targetKeys = new List<KeyCode>();

    public Dictionary<KeyCode, KeyScript> codeToScript = new Dictionary<KeyCode, KeyScript>();
    public KeyScript[] keys;
    public InputEvent InputEvent = new InputEvent();
    // Start is called before the first frame update
    private void Awake()
    {
        keys = keyboardParent.GetComponentsInChildren<KeyScript>();
        foreach(var keyScript in keys)
        {
            if(keyScript.keyCode != KeyCode.None)
            {
                codeToScript.Add(keyScript.keyCode, keyScript);
            }
        }
        //97 - 122 alphabet
        //48 -57 number
        // umlauts and symbols already in targetKeys

        targetKeys = targetKeys.Where(c => c != KeyCode.None).ToList();
        for(int i = 97; i <= 122; i++)
        {
            targetKeys.Add((KeyCode)i);
        }
        targetKeys.Add((KeyCode)48);
        for(int i = 50; i <= 57; i++)
        {
            targetKeys.Add((KeyCode)i);
        }
    }

    List<KeyCode> currentKeys = new List<KeyCode>();
    // Update is called once per frame
    void Update()
    {
        currentKeys.Clear();

        foreach(var keyCode in targetKeys)
        {
            if(Input.GetKeyDown(keyCode))
            {
                currentKeys.Add(keyCode);
            }
        }
        InputEvent.Invoke(this.currentKeys);
    }
}
