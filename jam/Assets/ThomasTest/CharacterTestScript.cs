using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTestScript : MonoBehaviour
{
    private bool IsPickedUp = false;

    private void Test()
    {
        Animator animator = GetComponent<Animator>();
        if (IsPickedUp)
        {
            IsPickedUp = false;
            animator.SetTrigger("PutDown");
        }
        else
        {
            IsPickedUp = true;
            animator.SetTrigger("PickUp");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }
    }
}
