using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public KeyCode keyCode;
    private float originalY;
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyCode))
        {
            StartCoroutine(PressMove(originalY-0.06f));
        }
        if(Input.GetKeyUp(keyCode))
        {
            StartCoroutine(PressMove(originalY));
        }
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

        yield return null;
    }
}
