using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public DoorLock doorLock;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public string doorLabel = "Door";
    public bool finalDoor = false;

    [Header("Unlock Message")]
    public string unlockMessage = "Door Unlocked";
    public float unlockMessageDuration = 2f;
    public Camera mainCamera;
    
    private bool isOpen = false;
    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Aim aimScript;
    

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0,0 , openAngle);

        // Find the Aim script from the Camera
        
        if (mainCamera != null)
        {
            aimScript = mainCamera.GetComponent<Aim>();
            if (aimScript == null)
            {
                Debug.LogWarning("Aim script not found on Camera!");
            }
        }
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

    private IEnumerator ShowUnlockMessage()
    {
        if (aimScript != null && aimScript.promptText != null)
        {
            aimScript.headerText.text = unlockMessage;
            yield return new WaitForSeconds(unlockMessageDuration);
            aimScript.headerText.text = "";
        }
    }

    public string GetPromptMessage()
    {
        // If door has a lock and it's not unlocked
        if (doorLock != null && !doorLock.IsUnlocked())
        {
            if (doorLabel == "Main Door")
            {
                return "Weird... This main door is closed. There must be another way out.";
            }
            if (Inventory.Instance.HasItem(doorLock.requiredKeyName))
            {
                return "Press [E] to unlock door";
            }
            return "Door is locked. Find a key.";
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
                StartCoroutine(ShowUnlockMessage());
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
        if(isOpen && finalDoor)
        {
            Debug.Log("Final door opened. You escaped!");
            SceneManager.LoadScene("Game Over");

        }
    }
}