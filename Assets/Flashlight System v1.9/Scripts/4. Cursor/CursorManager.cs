using UnityEngine;

namespace FlashlightSystem
{
    public class CursorManager : MonoBehaviour
    {
        [Header("Cursor Settings")]
        [SerializeField] private bool lockCursorOnStart = true;   // Should the cursor lock when the game starts?
        [SerializeField] private bool hideCursorOnStart = true;   // Should the cursor be hidden when the game starts?

        [Header("Should persist?")]
        [SerializeField] private bool persistAcrossScenes = true; // Optional: persist this object between scenes

        public static CursorManager instance;

        private void Awake()
        {
            // Singleton setup (optional — in case other scripts want to access this easily)
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

            ApplyCursorSettings();
        }

        // Applies the initial cursor settings
        private void ApplyCursorSettings()
        {
            SetCursorLock(lockCursorOnStart);
            SetCursorVisibility(!hideCursorOnStart);
        }

        // Lock or unlock the cursor
        public void SetCursorLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        // Show or hide the cursors
        public void SetCursorVisibility(bool visible)
        {
            Cursor.visible = visible;
        }

        // Full toggle in one call
        public void SetCursorState(bool locked, bool visible)
        {
            SetCursorLock(locked);
            SetCursorVisibility(visible);
        }
    }
}

