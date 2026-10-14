using UnityEngine;

namespace FlashlightSystem
{
    public class FlashlightPickupTrigger : MonoBehaviour
    {
        [Space(10)]
        [SerializeField] private ObjectType _objectType = ObjectType.None; // Defines what type of pickup this is

        private enum ObjectType { None, Flashlight, Battery } // Types of pickup available

        [Header("Battery Number")]
        [SerializeField] private int batteryAmount = 1; // How many batteries this pickup gives (if type is Battery)

        private const string playerTag = "Player"; // Tag used to identify the player

        // Triggered when something enters this object's collider
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                PickupFlashlightItem(); // Give the item to the player
            }
        }

        // Handles the pickup logic based on item type
        private void PickupFlashlightItem()
        {
            switch (_objectType)
            {
                case ObjectType.Flashlight:
                    FlashlightController.instance.CollectFlashlight(); // Give flashlight to player
                    break;
                case ObjectType.Battery:
                    FlashlightController.instance.CollectBattery(batteryAmount); // Give batteries to player
                    break;
            }

            gameObject.SetActive(false); // Disable this pickup after use
        }
    }
}
