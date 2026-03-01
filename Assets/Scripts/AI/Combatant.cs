using UnityEngine;

public class Combatant : MonoBehaviour
{
    public enum Faction
    {
        Player,
        Ally,
        Enemy
    }

    public enum WeaponType
    {
        SwordShield,
        BowSword
    }

    [Header("Identity")]
    [SerializeField] private string displayName = "Unit";
    [SerializeField] private Faction faction;
    [SerializeField] private WeaponType weaponType;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damage = 18;
    [SerializeField] private float attackInterval = 0.9f;
    [SerializeField] private float meleeRange = 1.9f;
    [SerializeField] private float rangedRange = 9f;

    private float attackCooldown;

    public string DisplayName => displayName;
    public Faction Team => faction;
    public WeaponType Weapon => weaponType;
    public int Health { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsAlive => Health > 0;
    public float AttackRange => weaponType == WeaponType.BowSword ? rangedRange : meleeRange;

    private void Awake()
    {
        Health = maxHealth;
    }

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public bool TryAttack(Combatant target)
    {
        if (!IsAlive || target == null || !target.IsAlive || attackCooldown > 0f)
        {
            return false;
        }

        target.ReceiveDamage(damage);
        attackCooldown = attackInterval;
        return true;
    }

    public void ReceiveDamage(int amount)
    {
        if (!IsAlive)
        {
            return;
        }

        Health -= Mathf.Max(0, amount);
        if (Health <= 0)
        {
            Health = 0;
            gameObject.SetActive(false);
        }
    }
}
