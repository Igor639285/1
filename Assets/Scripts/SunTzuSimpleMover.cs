using UnityEngine;

public class SunTzuSimpleMover : MonoBehaviour
{
    public float Speed { get; set; } = 5f;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        transform.position += direction * Speed * Time.deltaTime;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction, Vector3.up),
                12f * Time.deltaTime);
        }
    }
}
