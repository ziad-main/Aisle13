using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public DoorLock doorLock;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    
    private bool isOpen = false;
    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0,0 , openAngle);
    }

    void Update()
    {
        if (isOpening)
        {
            // Smoothly rotate the door
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                isOpen ? openRotation : closedRotation,
                Time.deltaTime * openSpeed
            );

            // Check if animation is complete
            if (Quaternion.Angle(transform.rotation, isOpen ? openRotation : closedRotation) < 0.1f)
            {
                isOpening = false;
            }
        }
    }

    public string GetPromptMessage()
    {
        // If door has a lock and it's not unlocked
        if (doorLock != null && !doorLock.IsUnlocked())
        {
            if (Inventory.Instance.HasItem(doorLock.requiredKeyName))
            {
                return "Press [E] to unlock door";
            }
            else
            {
                return "Door is locked. Find a key.";
            }
        }

        // If door is unlocked or has no lock
        if (isOpen)
        {
            return "Press [E] to close door";
        }
        else
        {
            return "Press [E] to open door";
        }
    }

    public void Interact()
    {
        // If door has a lock and it's not unlocked yet
        if (doorLock != null && !doorLock.IsUnlocked())
        {
            if (doorLock.TryUnlock())
            {
                Debug.Log("Door unlocked!");
                // Don't open yet, just unlock
                return;
            }
            else
            {
                Debug.Log("Need a key to unlock this door.");
                return;
            }
        }

        // Toggle door open/close
        isOpen = !isOpen;
        isOpening = true;
    }
}