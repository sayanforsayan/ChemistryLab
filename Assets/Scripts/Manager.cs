using DG.Tweening;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    [SerializeField] private CharacterController charcater;
    [SerializeField] private string[] messages;
    void Awake() => Instance = this;

    void Start()
    {
        CameraController.Instance.ChangeCamera(0);
        SendMessage(0);
        SendMessage(6, 2f);
    }

    // Pass message 
    public void SendMessage(int index, float delay = 0)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            charcater.PlayAnimation(AnimationType.Excited);
            charcater.ShowNarration(messages[index]);
        });
    }
}
