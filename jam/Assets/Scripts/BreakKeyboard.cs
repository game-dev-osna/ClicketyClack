using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakKeyboard : MonoBehaviour
{
    public KeyScript[] keys;
    // Start is called before the first frame update
    void Awake()
    {
        keys = GetComponentsInChildren<KeyScript>();
        Random.InitState(123);
        var shuffled = keys.OrderBy(x => Random.value).ToArray();    
    }
}
