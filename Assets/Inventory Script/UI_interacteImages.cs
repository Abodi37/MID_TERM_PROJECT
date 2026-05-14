using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InteracteImages : MonoBehaviour
{
    public InventoryManager inventoryManager;

    [Header("Interactables Images")]
    [SerializeField] private RawImage[] interactableImages;
    [SerializeField] private string[] itemNames;

    void Update()
    {
        for (int i = 0; i < interactableImages.Length; i++)
        {
            UpdateInventoryImage(interactableImages[i], itemNames[i]);
        }
    }

    private void UpdateInventoryImage(RawImage inventoryImage, string itemName)
    {
        bool hasItem = inventoryManager.HasItem(itemName);

        inventoryImage.enabled = hasItem;
    }
}