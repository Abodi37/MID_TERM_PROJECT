using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UHFPS.Runtime;

// Our inventory (the blood panel, top right) is the one the player sees.
// The UHFPS inventory stays hidden in the background because its puzzles (fusebox, locked doors...)
// read from it. This script keeps both in sync for the linked items, so:
// - anything picked up by UHFPS shows up in our inventory (and the other way around)
// - when the fusebox uses fuses, or our flashlight uses a battery, it's removed from both
// - Tab only opens our inventory; the UHFPS one only opens when a puzzle asks to pick an item
public class InventoryBridge : MonoBehaviour
{
    [Serializable]
    public class ItemLink
    {
        [Tooltip("Item name in our inventory")]
        public string ourItem;
        [Tooltip("Item title in the UHFPS inventory database")]
        public string uhfpsTitle;
        [Tooltip("Link every UHFPS item whose title starts with the text above (e.g. all keys)")]
        public bool matchTitleStart;
        [Tooltip("Off = only UHFPS changes are copied to our inventory (for items our scripts never use)")]
        public bool syncBothWays = true;

        [NonSerialized] public string[] guids;
        [NonSerialized] public int lastOurs;
        [NonSerialized] public int lastUhfps;
    }

    public InventoryManager inventoryManager;

    public List<ItemLink> links = new List<ItemLink>
    {
        new ItemLink { ourItem = "Battery", uhfpsTitle = "Alkaline Battery" },
        new ItemLink { ourItem = "Fuse", uhfpsTitle = "Fuse" },
        new ItemLink { ourItem = "Key", uhfpsTitle = "Key", matchTitleStart = true, syncBothWays = false },
    };

    [Tooltip("Tab opens only our inventory, never the UHFPS one")]
    public bool blockUhfpsInventoryKey = true;

    private Inventory uhfps;
    private bool initialized;
    private FieldInfo itemSelectorField;

    void Start()
    {
        if (inventoryManager == null) inventoryManager = GetComponent<InventoryManager>();
        if (inventoryManager == null) inventoryManager = FindFirstObjectByType<InventoryManager>();

        itemSelectorField = typeof(Inventory).GetField("itemSelector", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void LateUpdate()
    {
        if (inventoryManager == null) return;

        if (!initialized)
        {
            uhfps = Inventory.Instance;
            if (uhfps == null || uhfps.items == null) return;
            Initialize();
        }

        foreach (ItemLink link in links)
        {
            if (link.guids.Length > 0) Sync(link);
        }

        if (blockUhfpsInventoryKey) BlockUhfpsInventory();
    }

    void Initialize()
    {
        foreach (ItemLink link in links)
        {
            link.guids = uhfps.items
                .Where(pair => link.matchTitleStart ? pair.Value.Title.StartsWith(link.uhfpsTitle) : pair.Value.Title == link.uhfpsTitle)
                .Select(pair => pair.Key)
                .ToArray();

            if (link.guids.Length == 0)
            {
                Debug.LogWarning($"InventoryBridge: no UHFPS item called '{link.uhfpsTitle}'");
                continue;
            }

            // Items carried over from the previous level (UHFPS) + anything already in ours
            int ours = inventoryManager.GetItemCount(link.ourItem);
            int theirs = UhfpsCount(link);
            int total = ours + theirs;

            inventoryManager.SetItemCount(link.ourItem, total);
            if (link.syncBothWays) SetUhfpsCount(link, total);

            link.lastOurs = inventoryManager.GetItemCount(link.ourItem);
            link.lastUhfps = UhfpsCount(link);
        }

        initialized = true;
    }

    // Apply what changed on each side since last frame to both sides
    void Sync(ItemLink link)
    {
        int ours = inventoryManager.GetItemCount(link.ourItem);
        int theirs = UhfpsCount(link);
        int oursChange = ours - link.lastOurs;
        int theirsChange = theirs - link.lastUhfps;

        if (oursChange == 0 && theirsChange == 0) return;

        int total = Mathf.Max(0, link.lastOurs + oursChange + theirsChange);
        inventoryManager.SetItemCount(link.ourItem, total);
        if (link.syncBothWays) SetUhfpsCount(link, total);

        link.lastOurs = inventoryManager.GetItemCount(link.ourItem);
        link.lastUhfps = UhfpsCount(link);
    }

    int UhfpsCount(ItemLink link)
    {
        int count = 0;
        foreach (string guid in link.guids) count += uhfps.GetAllItemsQuantity(guid);
        return count;
    }

    void SetUhfpsCount(ItemLink link, int target)
    {
        string guid = link.guids[0];
        int difference = target - UhfpsCount(link);

        if (difference > 0) uhfps.AddItem(guid, (ushort)difference, new ItemCustomData());
        else if (difference < 0) uhfps.RemoveItemQuantityMany(guid, (ushort)(-difference));
    }

    // Tab opened the UHFPS inventory: close it again (our InventoryToggle handles Tab).
    // A puzzle asking the player to choose an item (VCR tape, keycard...) still opens it normally.
    void BlockUhfpsInventory()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null || !gameManager.IsInventoryShown) return;

        bool openedByPuzzle = itemSelectorField != null && (bool)itemSelectorField.GetValue(uhfps);
        if (!openedByPuzzle) gameManager.ShowInventoryPanel(false);
    }
}
