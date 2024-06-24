using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialControls : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private bool isPressed;

    private void Update()
    {
        if (Input.anyKeyDown && !isPressed)
        {
            isPressed = true;
            anim.Play("Pressed");
        }
    }
}
