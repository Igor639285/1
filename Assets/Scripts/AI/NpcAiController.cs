using UnityEngine;

[RequireComponent(typeof(Combatant))]
public class NpcAiController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float orbitRadius = 4f;

    private Combatant self;
    private Combatant forcedTarget;
    private bool hasMoveCommand;
    private Vector3 commandPoint;

    public void SetMoveCommand(Vector3 point)
    {
        commandPoint = point;
        hasMoveCommand = true;
        forcedTarget = null;
    }

    public void SetFocusTarget(Combatant target)
    {
        forcedTarget = target;
        hasMoveCommand = false;
    }

    public void ClearCommand()
    {
        forcedTarget = null;
        hasMoveCommand = false;
    }

    private void Awake()
    {
        self = GetComponent<Combatant>();
    }

    private void Update()
    {
        if (!self.IsAlive)
        {
            return;
        }

        if (hasMoveCommand)
        {
            ProcessMoveCommand();
            return;
        }

        Combatant target = forcedTarget != null && forcedTarget.IsAlive ? forcedTarget : FindClosestTarget();
        if (target == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        float attackRange = self.AttackRange;

        if (distance > attackRange)
        {
            Vector3 destination = target.transform.position;
            if (self.Weapon == Combatant.WeaponType.BowSword && distance < attackRange + orbitRadius)
            {
                Vector3 side = Vector3.Cross((target.transform.position - transform.position).normalized, Vector3.up);
                destination = target.transform.position + side * Mathf.Sin(Time.time * 2f) * 2f;
            }

            MoveTo(destination);
            return;
        }

        FaceTarget(target.transform.position);
        self.TryAttack(target);
    }

    private void ProcessMoveCommand()
    {
        float d = Vector3.Distance(transform.position, commandPoint);
        if (d <= 0.4f)
        {
            hasMoveCommand = false;
            return;
        }

        MoveTo(commandPoint);
    }

    private void MoveTo(Vector3 point)
    {
        Vector3 direction = (point - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;
        FaceTarget(transform.position + direction);
    }

    private void FaceTarget(Vector3 point)
    {
        Vector3 look = point - transform.position;
        look.y = 0f;

        if (look.sqrMagnitude < 0.001f)
        {
            return;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 10f * Time.deltaTime);
    }

    private Combatant FindClosestTarget()
    {
        Combatant[] all = FindObjectsOfType<Combatant>();
        Combatant best = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < all.Length; i++)
        {
            Combatant candidate = all[i];
            if (!candidate.IsAlive || candidate == self || !IsEnemy(candidate.Team))
            {
                continue;
            }

            float d = Vector3.Distance(transform.position, candidate.transform.position);
            if (d < bestDistance)
            {
                bestDistance = d;
                best = candidate;
            }
        }

        return best;
    }

    private bool IsEnemy(Combatant.Faction other)
    {
        if (self.Team == Combatant.Faction.Enemy)
        {
            return other == Combatant.Faction.Player || other == Combatant.Faction.Ally;
        }

        return other == Combatant.Faction.Enemy;
    }
}
