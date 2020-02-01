using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class PlayerUI : MonoBehaviour
{
    public List<Transform> UITransforms;

    private KeyboardInput keyInputs;

    public void SetWinKeys(List<KeyCode> keys)
    {
        keyInputs = GameObject.FindObjectOfType<KeyboardInput>();
        int index = 3;
        int size = UITransforms.Count;
        Debug.Log("1");
        foreach(var key in keys)
        {
            Debug.Log("2");
            var keyScript = keyInputs.codeToScript[key];
            var copyKey = Instantiate(keyScript, UITransforms[index%size]);
            copyKey.transform.localPosition = Vector3.zero;
            copyKey.transform.localRotation = Quaternion.identity;
            Destroy(copyKey.GetComponent<KeyScript>());
            UITransforms[index%size].gameObject.GetComponent<MeshRenderer>().enabled = false;
            index++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
