using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputEvent : UnityEvent<KeyCode>{}
public class KeyboardInput : MonoBehaviour
{
    public GameObject keyboardParent;
    public List<KeyCode> controllKeys = new List<KeyCode>();
    public List<KeyCode> targetKeys = new List<KeyCode>();
    public GameObject winCanvas;

    private StartGame startGame;

    public Dictionary<KeyCode, KeyScript> codeToScript = new Dictionary<KeyCode, KeyScript>();
    public KeyScript[] keys;
    public InputEvent TargetInput = new InputEvent();
    public InputEvent ActionInput = new InputEvent();
    // Start is called before the first frame update
    private void Awake()
    {
        startGame = FindObjectOfType<StartGame>();
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

    public int shuffleAmount;
    public void Shuffle()
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        var shuffled = targetKeys.OrderBy(x => UnityEngine.Random.value).Take(7).ToArray();    
        int[] dernged = Derange(shuffled.Length);

        var vecs = dernged.Select(i => codeToScript[shuffled[i]].transform.position).ToArray();
        for(int i = 0; i < shuffled.Length; i++)
        {
            codeToScript[shuffled[i]].transform.position = vecs[i];
        }
        targetKeys.Add((KeyCode)280);

        foreach(var key in targetKeys)
        {
            codeToScript[key].DoRightSpotCheck();
        }
        KeyScript.KeyPutEvent.AddListener(WinConCheck);
    }

    private void WinConCheck()
    {
        bool allTrue = true;
        foreach(var key in targetKeys)
        {
            if(!codeToScript[key].IsOnRightSpot)
            {
                allTrue = false;
                break;
            }
        }
        if(allTrue)
        {
            //win:
            Debug.Log("win");
            winCanvas.SetActive(true);
            StartCoroutine(RestartGame());
        }
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(3.0f);
        winCanvas.SetActive(false);
        startGame.Reset();
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
        if(currentKeys.Count > 0)
        {
            TargetInput.Invoke(this.currentKeys[0]);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ActionInput.Invoke(KeyCode.Space);
        }
    }

    private int[] Derange(int n)
    {
        while(true)
        {
            bool didBreak = false;
            int[] v = Enumerable.Range(0, n).ToArray();
            for(int j = n-1; j >= 0 ; j--)
            {
                int p = UnityEngine.Random.Range(0, j+1);
                if(v[p] == j)
                {
                    didBreak = true;
                    break;
                }
                else
                {
                    var temp = v[j];
                    v[j] = v[p];
                    v[p] = temp;
                }
            }
            if(!didBreak)
            {
                if(v[0] != 0)
                {
                    return v;
                }
            }
        }
    }
}
