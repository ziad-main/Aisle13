using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public string keyName = "DoorKey";
    private bool isCollected = false;

    public string GetPromptMessage()
    {
        if (!isCollected)
        {
            return "Press [E] to pick up key";
        }
        return "";
    }

    public void Interact()
    {
        if (!isCollected)
        {
            // Add to inventory
            Inventory.Instance.AddItem(keyName);
            isCollected = true;

            // Hide or destroy the key
            gameObject.SetActive(false);
        }
    }
}