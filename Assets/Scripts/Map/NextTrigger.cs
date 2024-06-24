using UnityEngine;
using UnityEngine.Events;

public class NextTrigger : MonoBehaviour
{
    [SerializeField] private RoomController roomController;
    [SerializeField] private UnityEvent triggerEvent;
    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            triggerEvent?.Invoke();
            GameManager.instance.NextRoomText();
        }
    }

    public void TriggerNextRoom()
    {
        roomController.NextRoom();
    }
}
