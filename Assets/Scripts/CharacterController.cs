using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// All character activity maintain here
    /// </summary>
    [SerializeField] private Animator anim;
    [SerializeField] private TMP_Text dialogueText;

    // Play Animation according clipname with proper enum name
    public void PlayAnimation(AnimationType clipName)
    {
        if (anim != null)
            anim.Play(clipName.ToString());
    }

    // When touched to character it say introduction
    void OnMouseDown()
    {
        Manager.Instance.SendMessage(0);
    }

    // Show text which character want to speak
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