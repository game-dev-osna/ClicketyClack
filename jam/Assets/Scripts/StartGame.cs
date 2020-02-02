using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

[Serializable]
public struct PlayerGameObjects
{
    public Player player;
    public List<GameObject> playerStuff;
}
public class StartGame : MonoBehaviour
{
    public GameObject gameName;
    public GameObject credits;
    public GameObject pressSpaceToStart;
    public Camera camera;
    public List<PlayerGameObjects> playerStuff = new List<PlayerGameObjects>();
    private List<Tweener> tweeners = new List<Tweener>();

    public bool inSetup;

    KeyboardInput keyInputs;
    Timer timer;
    EnemySpawner enemySpawner;

    void Awake()
    {
        keyInputs = GameObject.FindObjectOfType<KeyboardInput>();
        timer = GameObject.FindObjectOfType<Timer>();
        enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
    }

    private void Start()
    {
        var cameraTargetPos = camera.transform.position;
        var cameraTargetRot = camera.transform.rotation;

        foreach(var key in keyInputs.keys)
        {
            key.gameObject.SetActive(false);
        }

        Reset(false);

        Sequence sequence = DOTween.Sequence();
        camera.transform.position -= Vector3.forward * 74f;
        camera.transform.position += Vector3.up * 20f;
        camera.transform.rotation = Quaternion.identity;

        var first = camera.transform.DOMove(cameraTargetPos, 3f).Pause();
        var second = camera.transform.DORotateQuaternion(cameraTargetRot, 3f).Pause();

        first.OnComplete(() => {
            float maxTime = 1.5f;
            credits.SetActive(false);
            gameName.SetActive(false);
            foreach(var key in keyInputs.keys)
            {

                key.gameObject.SetActive(true);
                key.transform.position += Vector3.up * UnityEngine.Random.Range(7.2f, 10.3f);
                key.transform.DOMove(key.originalPos, UnityEngine.Random.Range(0.2f, maxTime));
            }
        });

        sequence.Append(first).Join(second).Play();

        sequence.OnComplete(() => {
                foreach(var p in playerStuff)
                {
                    var keyScript = keyInputs.codeToScript[p.player.ControllKey];
                    var rend = keyScript.GetComponent<MeshRenderer>();
                    if(rend)
                    {
                        var mat =  rend.material;
                        var activeColor = mat.color;
                        var inactiveColor = Color.white;
                        rend.material = new Material(mat);

                        var keyColorTween = DOTween.To(x => rend.material.SetColor("_BaseColor", Color.Lerp(inactiveColor, activeColor, x)), 0, 1, UnityEngine.Random.Range(0.45f,0.69f)).SetLoops(-1, LoopType.Yoyo);
                        keyColorTween.OnKill(() => {
                            rend.material.SetColor("_BaseColor", activeColor);
                        });
                        tweeners.Add(keyColorTween);
                    }
                }
        });
        StartCoroutine(DelayInteraction());
    }

    private void Update()
    {
        if(!inSetup) return;
        foreach(var p in playerStuff)
        {
            if(Input.GetKeyDown(p.player.ControllKey))
            {
                var active = p.player.gameObject.activeInHierarchy;
                p.player.gameObject.SetActive(!active);
                foreach(var go in p.playerStuff)
                {
                    go.SetActive(!active);
                }
                pressSpaceToStart.SetActive(playerStuff.Select(x => x.player.gameObject).Any(g => g.activeInHierarchy));
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(playerStuff.Select(p => p.player.gameObject).Any(gameObject => gameObject.activeInHierarchy))
            {
                StartIt();
            }
        }
    }

    private void StartIt()
    {
        timer.StartTimer();
        keyInputs.Shuffle();
        enemySpawner.ShouldSpawn = true;
        inSetup = false;
        foreach(var tween in tweeners)
        {
            tween.Kill();
        }
        tweeners.Clear();

        foreach(var p in playerStuff)
        {
            p.player.SetToStart();
        }
        pressSpaceToStart.SetActive(false);
    }

    private void Reset(bool doInSetup = true)
    {
        timer.Reset();
        enemySpawner.ShouldSpawn = false;

        foreach(var p in playerStuff)
        {
            p.player.gameObject.SetActive(false);
            foreach(var go in p.playerStuff)
            {
                go.SetActive(false);
            }
        }
        inSetup = doInSetup;
    }

    private IEnumerator DelayInteraction()
    {
        yield return new WaitForSeconds(4.5f);
        inSetup = true;
    }
}
