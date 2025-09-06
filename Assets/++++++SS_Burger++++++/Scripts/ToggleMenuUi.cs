using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenuUi : MonoBehaviour
{
    [SerializeField] private Animator toggleMenuAnim;

    private bool isOpen = false;

    private void Awake()
    {
        toggleMenuAnim = GetComponent<Animator>();
        isOpen = true;
        toggleMenuAnim.SetBool("isOpen", isOpen);
    }

    public void ToggleMenuBtn()
    {
        isOpen = !isOpen;

        toggleMenuAnim.SetBool("isOpen", isOpen);

    }
}
