using UnityEngine;

public class AbodiCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensetivity = 100f;
    public Transform CamX;
    float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        float MouseX = Input.GetAxis("Mouse X") * mouseSensetivity;
        float MouseY = Input.GetAxis("Mouse Y") * mouseSensetivity;

        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        CamX.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * MouseX);
    }
}
