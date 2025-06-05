using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMenu : MonoBehaviour
{
    private Animator animator;
    bool isOpen = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OpenOrCloseSkillMenu()
    {
        isOpen = !isOpen;
        animator.SetBool("Open", isOpen);
    }


}
