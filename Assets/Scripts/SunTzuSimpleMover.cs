using UnityEngine;

[RequireComponent(typeof(Combatant))]
public class SunTzuSimpleMover : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Combatant combatant;

    private void Awake()
    {
        combatant = GetComponent<Combatant>();
    }

    private void Update()
    {
        if (!combatant.IsAlive)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        transform.position += direction * speed * Time.deltaTime;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction, Vector3.up),
                12f * Time.deltaTime);
        }
    }
}
