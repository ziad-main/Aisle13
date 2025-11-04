using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Aim : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;
    
    [Header("UI References")]
    public TextMeshProUGUI promptText;
    public Image crosshair;

    private float xRotation = 0f;
    private IInteractable currentInteractable;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if (promptText != null)
        {
            promptText.text = "";
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleInteraction();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleInteraction()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Check if we're looking at something interactable
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // Update current interactable
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                    UpdatePrompt(interactable.GetPromptMessage());
                    ChangeCrosshairColor(Color.yellow);
                }

                // Handle interaction input
                if (Input.GetKeyDown(interactKey))
                {
                    interactable.Interact();
                    UpdatePrompt(interactable.GetPromptMessage());
                }
            }
        }
        else
        {
            // Clear prompt when not looking at anything
            if (currentInteractable != null)
            {
                currentInteractable = null;
                ClearPrompt();
                ChangeCrosshairColor(Color.white);
            }
        }
    }

    void UpdatePrompt(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }
    }

    void ClearPrompt()
    {
        if (promptText != null)
        {
            promptText.text = "";
        }
    }

    void ChangeCrosshairColor(Color color)
    {
        if (crosshair != null)
        {
            crosshair.color = color;
        }
    }
}