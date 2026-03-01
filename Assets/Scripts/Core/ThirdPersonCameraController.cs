using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 pivotOffset = new(0f, 1.6f, 0f);
    [SerializeField] private float distance = 6f;
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private float minPitch = -25f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float followSmooth = 14f;

    private float yaw;
    private float pitch = 20f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + pivotOffset;
        Vector3 desiredPosition = pivot - rotation * Vector3.forward * distance;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmooth * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, followSmooth * Time.deltaTime);
    }
}
