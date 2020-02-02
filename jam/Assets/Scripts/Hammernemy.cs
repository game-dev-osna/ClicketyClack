using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hammernemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(-1.23f, 2.38f), 0.43f,3.41f);
        //-49.029

        Sequence sequence = DOTween.Sequence();
        var first = transform.DORotateQuaternion(Quaternion.AngleAxis(-58, Vector3.right), 4f).Pause();
        var second = transform.DORotateQuaternion(Quaternion.identity, 0.6f).Pause();
        var third = transform.DORotateQuaternion(Quaternion.AngleAxis(-80, Vector3.right), 0.1f).SetEase(Ease.Linear).Pause();
        var fourth = transform.DORotateQuaternion(Quaternion.AngleAxis(58, Vector3.right), 5f).SetEase(Ease.Linear).Pause();
        fourth.OnComplete(() => {
            StartCoroutine(DestroyAfterDelay());
        });
        
        sequence.Append(first).Append(second).Append(third).Append(fourth).Play();
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
