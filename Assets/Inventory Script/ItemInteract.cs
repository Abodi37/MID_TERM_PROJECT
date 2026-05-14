using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteract : MonoBehaviour, IInteractable
{
    public InventoryManager inventoryManager;

    public void Interact()
    {
        Debug.Log("Interacting");

        inventoryManager.AddItem(gameObject.name);

        Destroy(gameObject, 0.1f);
    }
}