using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sawnemey : MonoBehaviour
{
    public List<Vector3> startPoints = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPoints[Random.Range(0,startPoints.Count)];
        //-2.779
        var target = new Vector3(-2.779f, transform.position.y, transform.position.z); 
        var targetEnd = new Vector3(8.779f, transform.position.y, transform.position.z); 
        Sequence sequence = DOTween.Sequence();
        var first = transform.DOMove(target, 3).Pause();
        var second = DOTween.To(x => {
            transform.Rotate(transform.forward, -360 * Time.deltaTime);
        }, 0, 1, 2f).Pause();
        second.OnComplete(() => {
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        });
        var third = DOTween.To(x => {
            transform.Rotate(transform.forward, -360 * Time.deltaTime);
        }, 0, 1, 4f).Pause();

        var fourth = transform.DOMove(targetEnd, 4f).Pause();;
        fourth.OnComplete(() => {
            StartCoroutine(DestroyAfterDelay());
        });

        sequence.Append(first).Append(second).Append(fourth).Join(third).Play();
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
