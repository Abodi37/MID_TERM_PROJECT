using UnityEngine;

namespace FlashlightSystem
{
    public class FlashlightController : MonoBehaviour
    {
        [Header("Flashlight On Start")]
        [SerializeField] private bool hasFlashlight = false; // If true, player starts with a flashlight

        [Header("Inventory Toggle")]
        [Tooltip("If this is true, allows the user to toggle inventory on / off")]
        [SerializeField] private bool showFlashlightInventory = false; // Allows toggling the UI display for flashlight info

        [Header("Infinite Flashlight")]
        [SerializeField] private bool infiniteFlashlight = false; // If enabled, flashlight never loses power

        [Header("Battery Parameters")]
        [SerializeField] private float batteryDrainAmount = 0.01f; // Battery drain per second
        [SerializeField] private int batteryCount = 1; // Amount of spare batteries
        [SerializeField] private bool drainBatteryWithoutDimming = false; // If true, battery drains but light stays bright

        [Header("Battery Reload Timers")]
        [SerializeField] private float replaceBatteryTimer = 1.0f; // Time required to reload battery
        [SerializeField] private float maxReplaceBatteryTimer = 1.0f; // Max time for the reload timer

        [Header("Flashlight Parameters")]
        [Range(0, 10)][SerializeField] private float maxFlashlightIntensity = 1.0f; // Max beam intensity
        [Range(1, 10)][SerializeField] private int flashlightRotationSpeed = 2; // Flashlight rotation smoothing speed

        [Header("Flashlight Controller Inputs")]
        [SerializeField] private KeyCode flashlightSwitch = KeyCode.F; // Turn flashlight on/off
        [SerializeField] private KeyCode reloadBattery = KeyCode.R; // Reload battery input
        [SerializeField] private KeyCode toggleFlashlightInv = KeyCode.Tab; // Toggle flashlight UI

        [Header("Main Flashlight References")]
        [SerializeField] private Light flashlightSpot = null; // Main light component for flashlight
        [SerializeField] private FlashlightMovement flashlightMovement = null; // Handles following the camera

        [Header("Flashlight Sound Names")]
        [SerializeField] private Sound flashlightPickup = null; // Sound for picking up flashlight/battery
        [SerializeField] private Sound flashlightClick = null; // Flashlight toggle click sound
        [SerializeField] private Sound flashlightReload = null; // Sound for reloading battery

        [Header("Should persist?")]
        [SerializeField] private bool persistAcrossScenes = true; // Keeps object when changing scenes

        private bool shouldUpdate = false; // Used for battery reload cooldown
        private bool isFlashlightOn; // Current on/off state of flashlight
        private float currentBatteryLevel = 1f; // Current battery charge (1 = full, 0 = empty)

        public static FlashlightController instance;

        private void Awake()
        {
            // Singleton: ensures only one instance exists
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
        }

        void Start()
        {
            flashlightSpot.intensity = maxFlashlightIntensity; // Start with full beam
            flashlightMovement.speed = flashlightRotationSpeed; // Set rotation follow speed
            maxReplaceBatteryTimer = replaceBatteryTimer; // Store default reload value
            currentBatteryLevel = 1f; // Start with full charge
            FLUIManager.instance.UpdateBatteryUI(batteryCount); // Update UI battery count

            // Warn if UI is disabled but batteries are limited
            if (!showFlashlightInventory && !infiniteFlashlight)
            {
                Debug.LogWarning($"{nameof(FlashlightController)}: You may want to make the flashlight infinite if you're not showing the flashlight UI");
            }
        }

        void ToggleInventory()
        {
            if (!showFlashlightInventory) return;
            FLUIManager.instance.ToggleFlashlightInventory(); // Toggle UI visibility
        }

        public void CollectFlashlight()
        {
            hasFlashlight = true; // Grant flashlight
            FlashlightPickupSound();
        }

        public void CollectBattery(int batteries)
        {
            batteryCount += batteries; // Add spare batteries
            FLUIManager.instance.UpdateBatteryUI(batteryCount);
            FlashlightPickupSound();
        }

        void Update()
        {
            if (hasFlashlight)
            {
                PlayerInput();

                // Drain battery only when flashlight is on and not infinite
                if (isFlashlightOn && !infiniteFlashlight)
                {
                    DrainBattery();
                }
            }
        }

        void PlayerInput()
        {
            if (Input.GetKeyDown(flashlightSwitch))
            {
                FlashlightSwitch(); // Turn flashlight on/off
            }

            if (!infiniteFlashlight)
            {
                // Hold to reload battery
                if (Input.GetKey(reloadBattery) && batteryCount >= 1)
                {
                    ReplaceBattery();
                }
                else
                {
                    CoolDownTimer(); // Slowly resets timer if player stops holding reload
                }

                if (Input.GetKeyUp(reloadBattery))
                {
                    shouldUpdate = true; // Begin cooldown to reset reload timer
                }
            }

            if (Input.GetKeyDown(toggleFlashlightInv))
            {
                ToggleInventory();
            }
        }

        void FlashlightSwitch()
        {
            isFlashlightOn = !isFlashlightOn; // Flip state

            flashlightSpot.enabled = isFlashlightOn; // Enable/disable light
            FLUIManager.instance.FlashlightIndicatorColor(isFlashlightOn); // Change UI color
            FlashlightClickSound();
        }

        void ReplaceBattery()
        {
            shouldUpdate = false;
            replaceBatteryTimer -= Time.deltaTime; // Count down reload timer

            // Show radial UI while reloading
            if (showFlashlightInventory)
            {
                ToggleRadialIndicator(true);
                UpdateRadialIndicator(replaceBatteryTimer);
            }

            if (replaceBatteryTimer > 0f) return; // Not finished reloading

            // Perform battery replacement
            if (batteryCount <= 0) return;
            batteryCount--;
            currentBatteryLevel = 1f; // Reset battery to full

            // Restore beam intensity if dimming mode is active
            flashlightSpot.intensity = Mathf.Clamp(flashlightSpot.intensity + maxFlashlightIntensity, 0f, maxFlashlightIntensity);

            // Update UI
            if (showFlashlightInventory)
            {
                FLUIManager.instance.UpdateBatteryUI(batteryCount);
                FLUIManager.instance.MaximumBatteryLevel(maxFlashlightIntensity);
                UpdateRadialIndicator(maxReplaceBatteryTimer);
                ToggleRadialIndicator(false);
            }

            FlashlightReloadSound();
            replaceBatteryTimer = maxReplaceBatteryTimer; // Reset timer
        }

        void CoolDownTimer()
        {
            if (shouldUpdate)
            {
                replaceBatteryTimer += Time.deltaTime; // Slowly restore reload timer
                if (showFlashlightInventory)
                {
                    UpdateRadialIndicator(replaceBatteryTimer);
                }

                if (replaceBatteryTimer >= maxReplaceBatteryTimer)
                {
                    replaceBatteryTimer = maxReplaceBatteryTimer; // Cap timer
                    if (showFlashlightInventory)
                    {
                        UpdateRadialIndicator(maxReplaceBatteryTimer);
                        ToggleRadialIndicator(false);
                    }
                    shouldUpdate = false;
                }
            }
        }

        void DrainBattery()
        {
            if (!infiniteFlashlight && isFlashlightOn)
            {
                float drainAmount = batteryDrainAmount * Time.deltaTime;
                currentBatteryLevel = Mathf.Clamp01(currentBatteryLevel - drainAmount); // Drain battery level

                if (showFlashlightInventory)
                {
                    FLUIManager.instance.UpdateBatteryLevelUI(drainAmount); // Update UI bar
                }

                if (!drainBatteryWithoutDimming)
                {
                    // Dimming mode: reduce beam brightness over time
                    flashlightSpot.intensity = Mathf.Clamp(flashlightSpot.intensity - drainAmount * maxFlashlightIntensity, 0, maxFlashlightIntensity);
                }

                // Auto shut off flashlight if empty
                if (currentBatteryLevel <= 0f)
                {
                    isFlashlightOn = false;
                    flashlightSpot.enabled = false;
                    FLUIManager.instance.FlashlightIndicatorColor(false);
                }
            }
        }

        void ToggleRadialIndicator(bool on)
        {
            FLUIManager.instance.ToggleRadialIndicator(on);
        }

        void UpdateRadialIndicator(float amount)
        {
            FLUIManager.instance.UpdateRadialIndicatorUI(amount);
        }

        void FlashlightPickupSound()
        {
            FLAudioManager.instance.Play(flashlightPickup);
        }

        void FlashlightClickSound()
        {
            FLAudioManager.instance.Play(flashlightClick);
        }

        void FlashlightReloadSound()
        {
            FLAudioManager.instance.Play(flashlightReload);
        }
    }
}
