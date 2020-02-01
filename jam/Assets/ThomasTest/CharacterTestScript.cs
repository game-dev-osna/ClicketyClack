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
            animator.SetBool("IsCarrying", false);
        }
        else
        {
            IsPickedUp = true;
            animator.SetBool("IsCarrying", true);
        }
    }
    private void Stun()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("IsStunned", true);
    }
    private void EndStun()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("IsStunned", false);
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            Stun();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            EndStun();
        }
    }
}
