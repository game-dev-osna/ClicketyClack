using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public KeyCode ControllKey;
    public AnimationCurve speedCurve;
    public Animator animator;
    public float speed = 1;
    public float rampDist;
    public float showMe;
    public float stunDuration;
    public Color inactiveColor;
    public Color activeColor;
    public SkinnedMeshRenderer meshRenderer;
    private KeyboardInput keyInputs;

    public Vector3 heightOffset = Vector3.zero;

    private bool isMoving;

    private float startDist;
    private Vector3 start;
    private Vector3 target;

    private bool isStunned;
    private KeyCode carryingKey = KeyCode.None;
    private GameObject carriedCopy;

    private void Start()
    {
        keyInputs = GameObject.FindObjectOfType<KeyboardInput>();
        keyInputs.TargetInput.AddListener(OnTarget);
        keyInputs.ActionInput.AddListener(OnAction);

        transform.position = keyInputs.codeToScript[ControllKey].transform.position + heightOffset;
        meshRenderer.material = new Material(meshRenderer.material);
        meshRenderer.material.SetColor("_BaseColor", inactiveColor);
    }

    private void Update() {
        if(Input.GetKeyDown(ControllKey))
        {
            DOTween.To(x => meshRenderer.material.SetColor("_BaseColor", Color.Lerp(inactiveColor, activeColor, x)), 0, 1, 0.25f);
        }
        if(Input.GetKeyUp(ControllKey))
        {
            DOTween.To(x => meshRenderer.material.SetColor("_BaseColor", Color.Lerp(activeColor, inactiveColor, x)), 0, 1, 0.25f);
        }
    }

    private void StartMoveToTarget(Vector3 target)
    {
        if(isStunned) return;
        start = transform.position;
        this.target = new Vector3(target.x, transform.position.y, target.z);
        startDist = Vector3.Distance(start, this.target);
        isMoving = true;
        transform.LookAt(this.target);

        //TODO with sequence to lerp up then steady then down
        var moveTween = transform.DOMove(this.target, startDist / speed);
        moveTween.OnUpdate(() => {
            var t =(Mathf.Sin(moveTween.ElapsedPercentage().Remap(0,1,-Mathf.PI+Mathf.PI/2,Mathf.PI+Mathf.PI/2)) + 1) / 2.0f;
            animator.SetFloat("Speed",t);

            Debug.Log("0: "+t);
        });
        moveTween.OnComplete(() => {
            animator.SetFloat("Speed", 0);
            isMoving = false;
        });
    }

    private void OnTarget(KeyCode target)
    {
        if(Input.GetKey(ControllKey))
        {
            //handle target
            StartMoveToTarget(keyInputs.codeToScript[target].transform.position + heightOffset);
        }
    }

    private void OnAction(KeyCode action)
    {
        if(Input.GetKey(ControllKey))
        {
            if(action == KeyCode.Space && !isMoving && !isStunned)
            {
                Debug.Log("Shitty shit");
                if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo ,0.4f))
                {
                    Debug.Log("hit");
                    var hitKey = hitInfo.transform.GetComponent<KeyScript>();
                    if(hitKey && keyInputs.targetKeys.Contains(hitKey.keyCode))
                    {
                        Debug.Log("containing and bla");
                        if(hitKey.IsRemoved && carryingKey == hitKey.keyCode)
                        {
                            hitKey.Put();
                            carryingKey = KeyCode.None;
                            Destroy(carriedCopy);
                        }
                        else if(carryingKey == KeyCode.None)
                        {
                            var keyCopy = Instantiate(hitKey, transform);
                            carriedCopy = keyCopy.gameObject;
                            Destroy(keyCopy);
                            carriedCopy.transform.position = transform.position + Vector3.up * 0.3f;
                            carryingKey = hitKey.Take();
                        }
                        else if(carryingKey != KeyCode.None)
                        {
                            var original = keyInputs.codeToScript[carryingKey];
                            original.Put();
                            Destroy(carriedCopy);


                            var temp = original.transform.position;
                            original.transform.position = hitKey.transform.position;
                            hitKey.transform.position = temp;
                            var keyCopy = Instantiate(hitKey, transform);
                            carriedCopy = keyCopy.gameObject;
                            Destroy(keyCopy);
                            carriedCopy.transform.position = transform.position + Vector3.up * 0.3f;
                            carryingKey = hitKey.Take();
                        }
                    }
                }
            }
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
        animator.SetFloat("Speed", 0);
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
