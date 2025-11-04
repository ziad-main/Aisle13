using UnityEngine;

public class DoorLock : MonoBehaviour
{
    public string requiredKeyName = "DoorKey";
    public GameObject lockVisual; // The padlock model

    private bool isUnlocked = false;

    public bool IsUnlocked()
    {
        return isUnlocked;
    }

    public bool TryUnlock()
    {
        if (Inventory.Instance.HasItem(requiredKeyName))
        {
            isUnlocked = true;
            
            // Remove lock visual
            if (lockVisual != null)
            {
                Destroy(lockVisual);
            }

            // Optional: Use the key (remove from inventory)
            // Inventory.Instance.RemoveItem(requiredKeyName);

            return true;
        }
        return false;
    }
}