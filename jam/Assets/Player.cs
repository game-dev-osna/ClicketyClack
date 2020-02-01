using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

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
    public Transform keyAttachPoint;
    public SkinnedMeshRenderer meshRenderer;
    private KeyboardInput keyInputs;

    public Vector3 heightOffset = Vector3.zero;

    private bool isMoving;

    private float startDist;
    private Vector3 start;
    private Vector3 target;

    private bool isStunned;

    public KeyCode CarryingKey => carryingKey;
    private KeyCode carryingKey = KeyCode.None;

    public GameObject CarriedCopy => carriedCopy;
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

    TweenerCore<Vector3, Vector3, VectorOptions> moveTween;
    private void StartMoveToTarget(Vector3 target)
    {
        if(isStunned) return;
        start = transform.position;
        this.target = new Vector3(target.x, transform.position.y, target.z);
        startDist = Vector3.Distance(start, this.target);
        isMoving = true;
        transform.LookAt(this.target);

        //TODO with sequence to lerp up then steady then down
        if(moveTween != null && moveTween.IsPlaying())
        {
            moveTween.SetTarget(this.target);
            moveTween.ChangeValues(start, this.target, startDist/speed);
        }
        else
        {
            moveTween = transform.DOMove(this.target, startDist / speed);
            moveTween.OnUpdate(() => {
                var t =(Mathf.Sin(moveTween.ElapsedPercentage().Remap(0,1,-Mathf.PI+Mathf.PI/2,Mathf.PI+Mathf.PI/2)) + 1) / 2.0f;
                animator.SetFloat("Speed",t);

                if(!isMoving || isStunned)
                {
                    moveTween.Kill();
                }
            });
            moveTween.OnComplete(() => {
                animator.SetFloat("Speed", 0);
                isMoving = false;
            });
            moveTween.OnKill(() => {
                animator.SetFloat("Speed", 0);
                isMoving = false;
            });
        }
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
        if(!Input.GetKey(ControllKey)) return;
        
        if(action == KeyCode.Space && !isMoving && !isStunned)
        {
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo ,0.4f))
            {
                var hitKey = hitInfo.transform.GetComponent<KeyScript>();
                if(hitKey && keyInputs.targetKeys.Contains(hitKey.keyCode))
                {
                    if(hitKey.IsRemoved && carryingKey == hitKey.keyCode)
                    {
                        PutDown(hitKey);
                    }
                    else if(carryingKey == KeyCode.None)
                    {
                        PickUp(hitKey);
                    }
                    else if(carryingKey != KeyCode.None)
                    {
                        var original = keyInputs.codeToScript[carryingKey];
                        original.Put();
                        Destroy(carriedCopy);


                        var temp = original.transform.position;
                        original.transform.position = hitKey.transform.position;
                        hitKey.transform.position = temp;

                        PickUp(hitKey);
                    }
                }
            }
        }
        
    }

    private void PutDown(KeyScript toPut)
    {
        toPut.Put();
        carryingKey = KeyCode.None;
        Destroy(carriedCopy);
        animator.SetBool("IsCarrying", false);
    }

    private void PickUp(KeyScript toTake)
    {
        var keyCopy = Instantiate(toTake, keyAttachPoint);
        carriedCopy = keyCopy.gameObject;
        Destroy(keyCopy);
        carriedCopy.transform.position = keyAttachPoint.transform.position;
        carriedCopy.transform.localScale = Vector3.one * 0.9f;
        carryingKey = toTake.Take();
        
        animator.SetBool("IsCarrying", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherPlayer = other.gameObject.GetComponent<Player>();
        if(otherPlayer)   
        {
            Stun(otherPlayer);
            otherPlayer.Stun(this, true);

        }
    }

    public void Stun(Player other, bool swap = false)
    {
        if(isStunned) return;
        transform.LookAt(other.transform);
        isStunned = true;
        isMoving = false;
        animator.SetFloat("Speed", 0);
        //collision animatiuon?!
        transform.position += transform.forward * -1 * 0.1f;
        animator.SetBool("IsStunned", true);
        if(swap)
        {
            KeySwap(other);
        }
        StartCoroutine(Unstun());
    }

    private void KeySwap(Player other)
    {
        var tempMine = carryingKey;
        var tempOther = other.CarryingKey;

        if(carryingKey != KeyCode.None)
        {
            PutDown(keyInputs.codeToScript[carryingKey]);
        }

        if(other.CarryingKey !=  KeyCode.None)
        {
            other.PutDown(keyInputs.codeToScript[other.CarryingKey]);
        }

        if(tempMine != KeyCode.None)
        {
            other.PickUp(keyInputs.codeToScript[tempMine]);
        }

        if(tempOther != KeyCode.None)
        {
            PickUp(keyInputs.codeToScript[tempOther]);
        }
    }

    private IEnumerator Unstun()
    {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        animator.SetBool("IsStunned", false);
    }
}
