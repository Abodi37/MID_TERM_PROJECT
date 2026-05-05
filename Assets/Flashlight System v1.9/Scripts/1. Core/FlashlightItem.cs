using UnityEngine;

namespace FlashlightSystem
{
    public class FlashlightItem : MonoBehaviour
    {
        [Space(10)]
        [SerializeField] private ObjectType _objectType = ObjectType.None; // Defines what type of object this is

        private enum ObjectType { None, Flashlight, Battery } // Possible item types

        [SerializeField] private int batteryAmount = 1; // Number of batteries this item provides (if type is Battery)

        // Called when the player interacts with this object
        public void ObjectInteract()
        {
            switch (_objectType)
            {
                case ObjectType.Flashlight:
                    FlashlightController.instance.CollectFlashlight(); // Give player the flashlight
                    break;

                case ObjectType.Battery:
                    FlashlightController.instance.CollectBattery(batteryAmount); // Add batteries to inventory
                    break;
            }

            gameObject.SetActive(false); // Disable this object after interaction
        }
    }
}

