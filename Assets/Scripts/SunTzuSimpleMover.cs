using UnityEngine;

[RequireComponent(typeof(Combatant))]
public class SunTzuSimpleMover : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 1.7f;
    [SerializeField] private float turnSmooth = 14f;

    private Combatant combatant;
    private bool isControlled;

    public bool IsControlled => isControlled;

    private void Awake()
    {
        combatant = GetComponent<Combatant>();
    }

    public void SetControlEnabled(bool value)
    {
        isControlled = value;
    }

    private void Update()
    {
        if (!isControlled || !combatant.IsAlive)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        Vector3 forward = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;
        Vector3 right = Camera.main != null ? Camera.main.transform.right : Vector3.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * input.z + right * input.x;
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * sprintMultiplier : speed;
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDirection, Vector3.up),
                turnSmooth * Time.deltaTime);
        }
    }
}
