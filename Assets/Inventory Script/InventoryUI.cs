using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventoryManager inventoryManager;

    [Header("Battery UI")]
    public TextMeshProUGUI batteryText;
    public GameObject batteryImage;

    [Header("Pill UI")]
    public TextMeshProUGUI pillText;
    public GameObject pillImage;

    [Header("Key UI")]
    public TextMeshProUGUI keyText;
    public GameObject keyImage;

    void Update()
    {
        // Battery
        int batteryCount = inventoryManager.GetItemCount("Battery");

        batteryText.text = batteryCount.ToString();

        batteryText.gameObject.SetActive(batteryCount > 0);
        batteryImage.SetActive(batteryCount > 0);

        // Pill
        int pillCount = inventoryManager.GetItemCount("Pill");

        pillText.text = pillCount.ToString();

        pillText.gameObject.SetActive(pillCount > 0);
        pillImage.SetActive(pillCount > 0);

        // Key
        int keyCount = inventoryManager.GetItemCount("Key");

        keyText.text = keyCount.ToString();

        keyText.gameObject.SetActive(keyCount > 0);
        keyImage.SetActive(keyCount > 0);
    }
}