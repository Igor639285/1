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

    [SerializeField] private Faction faction;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private int maxHealth = 100;

    public Faction Team => faction;
    public WeaponType Weapon => weaponType;
    public int Health { get; private set; }
    public bool IsAlive => Health > 0;

    private void Awake()
    {
        Health = maxHealth;
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
