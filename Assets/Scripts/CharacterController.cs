using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CharacterController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private TMP_Text dialogueText;

    public void PlayAnimation(AnimationType clipName)
    {
        if (anim != null)
            anim.Play(clipName.ToString());
    }

    void OnMouseDown()
    {
        Manager.Instance.SendMessage(0);
    }

    public void ShowNarration(string message)
    {
        if (dialogueText != null)
            dialogueText.text = "" + message;
    }
}

public enum AnimationType
{
    Idle, Excited, Irritated
}