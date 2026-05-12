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

    public bool HasItem(string itemName)
    {
        InventoryItem existingItem = inventory.FirstOrDefault(item => item.itemName == itemName);

        return existingItem != null && existingItem.quantity > 0;
    }
}