using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;


public class Penemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //x: -1.23 - 2.38
        //z: -5.72
        //y: 0.853
        transform.position = new Vector3(Random.Range(-1.23f, 2.38f), 0.853f,-7.18f);
        var target = new Vector3(transform.position.x, transform.position.y, -5.72f);
        var targetEnd = new Vector3(transform.position.x, transform.position.y, 5.72f);

        Sequence sequence = DOTween.Sequence();
        // -5.72f
        var first = transform.DOMove(target, 3).Pause();
        var second = DOTween.To(x => {
            transform.rotation = Quaternion.AngleAxis( Random.Range(-0.5f,0.5f),Vector3.up);
        }, 0, 1, 2).Pause();
        second.OnComplete(() => {
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        });
        var third = transform.DOMove(targetEnd, 1).Pause();
        third.OnComplete(() => {
            StartCoroutine(DestroyAfterDelay());
        });
        sequence.Append(first).Append(second).Append(third).Play();

    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
