using UnityEngine;

[RequireComponent(typeof(Combatant))]
public class NpcAiController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.4f;
    [SerializeField] private float meleeRange = 1.6f;
    [SerializeField] private float rangedRange = 8f;
    [SerializeField] private float attackCooldown = 1.2f;

    private Combatant self;
    private float cooldown;

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

        cooldown -= Time.deltaTime;
        Combatant target = FindClosestTarget();
        if (target == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        float attackRange = self.Weapon == Combatant.WeaponType.BowSword ? rangedRange : meleeRange;

        if (distance > attackRange)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 7f * Time.deltaTime);
            }

            return;
        }

        if (cooldown <= 0f)
        {
            int damage = self.Weapon == Combatant.WeaponType.BowSword ? 12 : 18;
            target.ReceiveDamage(damage);
            cooldown = attackCooldown;
        }
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
