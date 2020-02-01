using System.Collections;
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

    public bool IsRemoved => removed;
    private bool removed;
    private MeshRenderer renderer;
    private Canvas canvas;
    private Vector3 originalPos;

    static public UnityEvent KeyPutEvent = new UnityEvent();

    private void Awake() {
        originalPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        renderer = GetComponent<MeshRenderer>();
        originalY = transform.position.y;
        keyboardSoundPlayer = GameObject.FindObjectOfType<KeyboardSoundPlayer>();
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
            StartCoroutine(PressMove(originalY-0.06f));
        }
        if(Input.GetKeyUp(keyCode))
        {
            StartCoroutine(PressMove(originalY));
        }
    }

    public KeyCode Take()
    {
        renderer.enabled = false;
        canvas.gameObject.SetActive(false);
        removed = true;
        isOnRightSpot = false;
        return keyCode;
    }

    public void Put()
    {
        canvas.gameObject.SetActive(true);
        removed = false;
        renderer.enabled = true;

        isOnRightSpot = Vector3.Distance(transform.position, originalPos) < 0.01f;
        KeyPutEvent.Invoke();
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
}
