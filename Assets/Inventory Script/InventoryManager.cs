using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> inventory = new List<InventoryItem>();

    public void AddItem(string itemName)
    {
        InventoryItem existingItem = inventory.FirstOrDefault(item => item.itemName == itemName);

        if (existingItem != null)
        {
            existingItem.quantity++;
        }
        else
        {
            InventoryItem newItem = new InventoryItem
            {
                itemName = itemName,
                quantity = 1
            };

            inventory.Add(newItem);
        }
    }

    public void RemoveItem(string itemName)
    {
        InventoryItem existingItem = inventory.FirstOrDefault(item => item.itemName == itemName);

        if (existingItem != null)
        {
            existingItem.quantity--;

            if (existingItem.quantity <= 0)
            {
                inventory.Remove(existingItem);
            }
        }
    }

    public int GetItemCount(string itemName)
    {
        InventoryItem existingItem = inventory.FirstOrDefault(item => item.itemName == itemName);

        if (existingItem != null)
        {
            return existingItem.quantity;
        }

        return 0;
    }

    // Used by InventoryBridge to keep this inventory in sync with the UHFPS one
    public void SetItemCount(string itemName, int count)
    {
        InventoryItem existingItem = inventory.FirstOrDefault(item => item.itemName == itemName);

        if (count <= 0)
        {
            if (existingItem != null) inventory.Remove(existingItem);
        }
        else if (existingItem != null)
        {
            existingItem.quantity = count;
        }
        else
        {
            inventory.Add(new InventoryItem { itemName = itemName, quantity = count });
        }
    }

    public bool HasItem(string itemName)
    {
        InventoryItem existingItem = inventory.FirstOrDefault(item => item.itemName == itemName);

        return existingItem != null && existingItem.quantity > 0;
    }
}