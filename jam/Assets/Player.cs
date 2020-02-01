using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public KeyCode ControllKey;
    public AnimationCurve speedCurve;
    public float speed = 1;
    public float rampDist;
    public float showMe;
    public float stunDuration;
    private KeyboardInput keyInputs;

    private Vector3 heightOffset =  Vector3.up * 0.36f;

    private bool isMoving;

    private float startDist;
    private Vector3 start;
    private Vector3 target;

    private bool isStunned;

    private void Start()
    {
        keyInputs = GameObject.FindObjectOfType<KeyboardInput>();
        keyInputs.InputEvent.AddListener(OnInput);

        transform.position = keyInputs.codeToScript[ControllKey].transform.position + heightOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            var dir = target - transform.position;
            float dist = dir.magnitude;

            float speedMult  = 1;
            /*
            if(dist > rampDist && speed < 1)
            {
                speed += 
            }
            */


            if(startDist - dist < rampDist)
            {
                speedMult = speedCurve.Evaluate((startDist - dist).Remap(0,rampDist,0,1));
            }
            else if( dist < rampDist)
            {
                speedMult = speedCurve.Evaluate(dist.Remap(0,rampDist,0,1));
            }
            speedMult = Mathf.Clamp(speedMult, 0.6f, 1);

            if(dist > 0.02f)
            {
                showMe = speed * speedMult;
                transform.position += dir.normalized * Time.deltaTime * showMe;
            }
            else
            {
                transform.position = target;
                isMoving = false;
            }
        }
    }

    private void StartMoveToTarget(Vector3 target)
    {
        if(isStunned) return;
        start = transform.position;
        this.target = target;
        startDist = Vector3.Distance(target, start);
        isMoving = true;
        transform.LookAt(target);
    }

    private void OnInput(List<KeyCode> targets)
    {
        if(Input.GetKey(ControllKey) && targets.Count > 0)
        {
            //handle target
            StartMoveToTarget(keyInputs.codeToScript[targets[0]].transform.position + heightOffset);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherPlayer = other.gameObject.GetComponent<Player>();
        if(otherPlayer)   
        {
            Debug.Log("COLLISONS "+Time.frameCount);
            Stun(other.transform);
            otherPlayer.Stun(transform);
        }
    }

    public void Stun(Transform other)
    {
        if(isStunned) return;
        transform.LookAt(other.transform);
        isStunned = true;
        isMoving = false;
        //collision animatiuon?!
        transform.position += transform.forward * -1 * 0.04f;
        StartCoroutine(Unstun());
    }

    private IEnumerator Unstun()
    {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }
}
