using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    public List<KeyCode> controllKeys = new List<KeyCode>();
    List<KeyCode> allKeycodes = new List<KeyCode>();
    // Start is called before the first frame update
    void Start()
    {
        var allkeycodes = Enum.GetValues(typeof(KeyCode));
        foreach(var keycode in allkeycodes)
        {
            allKeycodes.Add((KeyCode)keycode);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //97 - 122 alphabet
        //48 -57 number
        // umlauts
        //(Input.GetKeyDown(KeyCode.))
        foreach(var keyCode in allKeycodes)
        {
            if(Input.GetKey(keyCode))
            {
                Debug.Log(keyCode +" frame: "+Time.frameCount);
            }
        }
    }
}
