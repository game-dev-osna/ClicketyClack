using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class Player : MonoBehaviour
{
    public PlayerUI PlayerUI;

    public List<KeyCode> WinConditionKeys;
    public static int NeedForWin = 3;

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
    public ParticleSystem trail;
    public GameObject collisionEffect;
    public GameObject takeEffect;
    public GameObject checkEffect;
    public ParticleSystem stunEffect;
    private KeyboardInput keyInputs;
    private Depot depot;

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

    public float maxControllTime;
    public float controllTime;

    private void Start()
    {
        keyInputs = GameObject.FindObjectOfType<KeyboardInput>();
        depot = GameObject.FindObjectOfType<Depot>();
        keyInputs.TargetInput.AddListener(OnTarget);
        keyInputs.ActionInput.AddListener(OnAction);

        transform.position = keyInputs.codeToScript[ControllKey].transform.position + heightOffset;
        meshRenderer.material = new Material(meshRenderer.material);
        meshRenderer.material.SetColor("_BaseColor", inactiveColor);
        controllTime = maxControllTime;
        SetMoving(false);
        enemyLayer = LayerMask.NameToLayer("Enemy");

        stunEffect.gameObject.SetActive(false);
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
        if(Input.GetKey(ControllKey))
        {
            controllTime = Mathf.Max(controllTime - Time.deltaTime, 0);
            if(controllTime <= 0)
            {
                Stun(null);
            }
        }
        else
        {
            controllTime = Mathf.Min(controllTime + Time.deltaTime, maxControllTime);
        }
        PlayerUI.UpdateMeterUI(controllTime / maxControllTime);
    }

    TweenerCore<Vector3, Vector3, VectorOptions> moveTween;
    private void StartMoveToTarget(Vector3 target)
    {
        if(isStunned) return;
        start = transform.position;
        this.target = new Vector3(target.x, transform.position.y, target.z);
        startDist = Vector3.Distance(start, this.target);
        SetMoving(true);
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
                SetMoving(false);
            });
            moveTween.OnKill(() => {
                SetMoving(false);
            });
        }
    }

    private void SetMoving(bool moving)
    {
        isMoving = moving;
        if(!moving)
        {
            animator.SetFloat("Speed", 0);
        }
        trail.enableEmission = moving;
    }

    private void OnTarget(KeyCode target)
    {
        if(Input.GetKey(ControllKey))
        {

            if(keyInputs.codeToScript[target].IsBroken) return;
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
                if(hitKey && keyInputs.targetKeys.Contains(hitKey.keyCode) && hitKey.keyCode != KeyCode.PageUp)
                {
                    if(hitKey.IsRemoved && carryingKey == hitKey.keyCode)
                    {
                        PutDown(hitKey);
                        if (hitKey.isOnRightSpot)
                        {
                            Debug.Log("test2");
                            SpawnEffect(checkEffect, transform.position + Vector3.up * 0.52f);
                        }
                    }
                    else if(carryingKey == KeyCode.None && !hitKey.IsRemoved)
                    {
                        PickUp(hitKey);
                        SpawnEffect(takeEffect, transform.position + Vector3.up * 0.52f);
                    }
                    else if(carryingKey != KeyCode.None)
                    {
                        var original = keyInputs.codeToScript[carryingKey];

                        var temp = original.transform.position;
                        original.Put(hitKey.transform.position);
                        Destroy(carriedCopy);


                        hitKey.transform.position = temp;

                        PickUp(hitKey);
                        if (original.isOnRightSpot)
                        {
                            SpawnEffect(checkEffect, transform.position + Vector3.up * 0.52f);
                        }
                        else
                        {
                            SpawnEffect(takeEffect, transform.position + Vector3.up * 0.52f);
                        }
                    }
                }
                var hitDepot = hitInfo.transform.GetComponent<Depot>();
                if(hitDepot != null && carryingKey == KeyCode.None)
                {
                    var depotKey = depot.TakeFromDepot();
                    if(depotKey != null)
                    {
                        PickUp(depotKey);
                        SpawnEffect(takeEffect, transform.position + Vector3.up * 0.52f);
                    }
                }
            }
        }

    }

    private void SpawnEffect(GameObject effectPrefab, Vector3 pos)
    {
        var effect = Instantiate(effectPrefab, pos , Quaternion.identity);
        effect.transform.LookAt(Camera.main.transform);
        effect.transform.forward = effect.transform.forward * -1;
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
        carriedCopy.transform.localScale = Vector3.one * 0.6f;
        carryingKey = toTake.Take();

        animator.SetBool("IsCarrying", true);
    }

    private int enemyLayer;
    private void OnTriggerEnter(Collider other)
    {
        var otherPlayer = other.gameObject.GetComponent<Player>();
        if(otherPlayer)
        {
            Stun(otherPlayer);
            otherPlayer.Stun(this, true);

        }
        else if(other.gameObject.layer == enemyLayer)
        {
            //Effect

            if(carryingKey != KeyCode.None)
            {
                depot.PlaceInDepot(keyInputs.codeToScript[carryingKey]);
                Destroy(carriedCopy);
                animator.SetBool("IsCarrying", false);
                carryingKey = KeyCode.None;
            }
            transform.position = keyInputs.codeToScript[ControllKey].transform.position + heightOffset;
            Stun(null);
        }
    }

    public void Stun(Player other, bool doOnce = false)
    {
        if(isStunned) return;
        if(doOnce)
        {
            KeySwap(other);
            SpawnEffect(collisionEffect, (transform.position + other.transform.position) / 2 + Vector3.up * 0.43f);
        }
        if(other != null)
        {
            transform.LookAt(other.transform);
        }
        isStunned = true;
        SetMoving(false);
        //collision animatiuon?!
        transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * -1 * 0.1f;
        animator.SetBool("IsStunned", true);
        StartCoroutine(Unstun());
    }

    private void KeySwap(Player other)
    {
        var tempMine = carryingKey;
        var tempOther = other.CarryingKey;
        if(carryingKey != KeyCode.None && other.CarryingKey != KeyCode.None) return;

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
        var stunTime = Random.Range(stunDuration-0.75f, stunDuration+0.75f);
        stunEffect.Clear();
        stunEffect.Stop();
        var main = stunEffect.main;
        main.duration = stunTime;
        stunEffect.Play();
        stunEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        animator.SetBool("IsStunned", false);
        stunEffect.gameObject.SetActive(false);
    }
}
