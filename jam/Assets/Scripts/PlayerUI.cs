using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public List<Transform> UITransforms;
    public Material correctMaterial;
    public Material wrongMaterial;
    public Image fillbar;

    private KeyboardInput keyInputs;
    private bool isInitialized = false;
    private Dictionary<KeyCode, GameObject> keyScripts = new Dictionary<KeyCode, GameObject>();

    public void AddWinKeys(List<KeyCode> keys)
    {
        isInitialized = true;
        int index = 3;
        int size = UITransforms.Count;
        foreach(var tra in UITransforms)
        {
            tra.GetComponent<MeshRenderer>().material = wrongMaterial;
        }
        foreach (var key in keys)
        {
            var keyScript = keyInputs.codeToScript[key];
            var copyKey = Instantiate(keyScript, UITransforms[index % size]);
            copyKey.transform.localPosition = Vector3.zero;
            copyKey.transform.localRotation = Quaternion.identity;
            Destroy(copyKey.GetComponent<KeyScript>());
            UITransforms[index % size].gameObject.GetComponent<MeshRenderer>().enabled = false;
            keyScripts.Add(key, copyKey.gameObject);
            index++;
        }
    }

    public void UpdateMeterUI(float percent)
    {
        fillbar.fillAmount = percent;
    }

    public void updateKeyUi(List<KeyCode> keys)
    {
        if (!isInitialized) AddWinKeys(keys);
        foreach (var key in keys)
        {
            var keyScript = keyInputs.codeToScript[key];
            if (keyScript.IsOnRightSpot)
            {
                keyScripts[key].GetComponent<MeshRenderer>().material = correctMaterial;
            }
            else
            {
                keyScripts[key].GetComponent<MeshRenderer>().material = wrongMaterial;
            }
        }
    }

    private void Awake()
    {
        keyInputs = GameObject.FindObjectOfType<KeyboardInput>();
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
