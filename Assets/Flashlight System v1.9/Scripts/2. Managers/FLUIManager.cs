using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlashlightSystem
{
    public class FLUIManager : MonoBehaviour
    {
        [Header("UI Canvas")]
        [SerializeField] private CanvasGroup flashlightCanvas = null; // Controls visibility of the flashlight UI

        [Header("UI Battery")]
        [SerializeField] private Slider batteryLevelSlider = null;     // Visual battery drain bar
        [SerializeField] private TMP_Text batteryCountUI = null;       // Text display for battery count

        [Header("UI Indicator")]
        [SerializeField] private Image flashlightIndicatorUI = null;   // Flashlight on/off status indicator
        [SerializeField] private Color flashlightIndicatorOn = Color.green; // Color when flashlight is on
        [SerializeField] private Color flashlightIndicatorOff = Color.white; // Color when flashlight is off

        [Header("Radial Indicator")]
        [SerializeField] private Image radialIndicatorUI = null;       // Radial reload indicator

        [Header("Crosshair")]
        [SerializeField] private Image crosshairUI = null;             // Interaction crosshair

        [Header("Should persist?")]
        [SerializeField] private bool persistAcrossScenes = true;      // Optional persistence between scenes

        private bool showUI;                                           // Tracks inventory UI visibility

        public static FLUIManager instance;

        private void Awake()
        {
            // Singleton setup
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                if (persistAcrossScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }

            FieldNullCheck(); // Verify required references
            flashlightCanvas.alpha = 0; // Start hidden
        }

        // Updates battery count display text
        public void UpdateBatteryUI(int batteryCount)
        {
            batteryCountUI.text = batteryCount.ToString("0");
        }

        // Lowers the battery level bar by a drain amount
        public void UpdateBatteryLevelUI(float drainAmount)
        {
            batteryLevelSlider.value -= drainAmount;
        }

        // Resets battery bar to full when replacing battery
        public void MaximumBatteryLevel(float maxIntensity)
        {
            batteryLevelSlider.value = maxIntensity;
        }

        // Enables or disables the radial reload indicator
        public void ToggleRadialIndicator(bool on)
        {
            radialIndicatorUI.enabled = on;
        }

        // Sets fill level of radial reload indicator
        public void UpdateRadialIndicatorUI(float indicatorAmount)
        {
            radialIndicatorUI.fillAmount = indicatorAmount;
        }

        // Toggles visibility of the flashlight inventory UI
        public void ToggleFlashlightInventory()
        {
            showUI = !showUI;
            flashlightCanvas.alpha = showUI ? 1 : 0;
        }

        // Changes the flashlight status indicator color
        public void FlashlightIndicatorColor(bool on)
        {
            flashlightIndicatorUI.color = on ? flashlightIndicatorOn : flashlightIndicatorOff;
        }

        // Changes crosshair color based on hover state
        public void HighlightCrosshair(bool on)
        {
            crosshairUI.color = on ? Color.red : Color.white;
        }

        // Checks required UI references and logs errors for any missing fields
        void FieldNullCheck()
        {
            CheckField(batteryLevelSlider, "BatteryLevelUI");
            CheckField(batteryCountUI, "BatteryCountUI");
            CheckField(flashlightIndicatorUI, "FlashlightIndicatorUI");
            CheckField(flashlightCanvas, "FlashlightCanvas");
            CheckField(radialIndicatorUI, "RadialIndicatorUI");
            CheckField(crosshairUI, "CrosshairUI");
        }

        // Logs a message if a serialized UI field is not set
        void CheckField(Object field, string fieldName)
        {
            if (field == null)
            {
                Debug.LogError(gameObject + " " + $"FieldNullCheck: {fieldName} is not set in the inspector!");
            }
        }
    }
}
