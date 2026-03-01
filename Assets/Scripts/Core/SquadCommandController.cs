using System.Collections.Generic;
using UnityEngine;

public class SquadCommandController : MonoBehaviour
{
    private readonly List<SunTzuSimpleMover> controllables = new();
    private int currentIndex;

    public SunTzuSimpleMover CurrentUnit => controllables.Count == 0 ? null : controllables[currentIndex];

    private ThirdPersonCameraController cameraController;

    private void Start()
    {
        cameraController = FindObjectOfType<ThirdPersonCameraController>();
        RebuildUnitList();
        SetActiveUnit(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleUnit();
        }

        if (Input.GetMouseButtonDown(1))
        {
            IssueMoveCommand();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            IssueFocusCommand();
        }

        if (Input.GetMouseButtonDown(0) && CurrentUnit != null)
        {
            TryPlayerAttack();
        }
    }

    public void RebuildUnitList()
    {
        controllables.Clear();
        SunTzuSimpleMover[] movers = FindObjectsOfType<SunTzuSimpleMover>();
        for (int i = 0; i < movers.Length; i++)
        {
            Combatant c = movers[i].GetComponent<Combatant>();
            if (c.Team == Combatant.Faction.Player || c.Team == Combatant.Faction.Ally)
            {
                controllables.Add(movers[i]);
                movers[i].SetControlEnabled(false);
            }
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Max(controllables.Count - 1, 0));
    }

    public void CycleUnit()
    {
        if (controllables.Count == 0)
        {
            return;
        }

        currentIndex = (currentIndex + 1) % controllables.Count;
        SetActiveUnit(currentIndex);
    }

    private void SetActiveUnit(int index)
    {
        for (int i = 0; i < controllables.Count; i++)
        {
            bool active = i == index && controllables[i].GetComponent<Combatant>().IsAlive;
            controllables[i].SetControlEnabled(active);
        }

        if (cameraController != null && CurrentUnit != null)
        {
            cameraController.SetTarget(CurrentUnit.transform);
        }
    }

    private void IssueMoveCommand()
    {
        if (Camera.main == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            return;
        }

        NpcAiController[] allies = FindObjectsOfType<NpcAiController>();
        for (int i = 0; i < allies.Length; i++)
        {
            Combatant c = allies[i].GetComponent<Combatant>();
            if (c.Team == Combatant.Faction.Ally)
            {
                Vector3 spread = new Vector3((i - 1) * 1.5f, 0f, 0f);
                allies[i].SetMoveCommand(hit.point + spread);
            }
        }
    }

    private void IssueFocusCommand()
    {
        if (Camera.main == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            return;
        }

        Combatant enemy = hit.collider.GetComponentInParent<Combatant>();
        if (enemy == null || enemy.Team != Combatant.Faction.Enemy)
        {
            return;
        }

        NpcAiController[] allies = FindObjectsOfType<NpcAiController>();
        for (int i = 0; i < allies.Length; i++)
        {
            Combatant c = allies[i].GetComponent<Combatant>();
            if (c.Team == Combatant.Faction.Ally)
            {
                allies[i].SetFocusTarget(enemy);
            }
        }
    }

    private void TryPlayerAttack()
    {
        Combatant self = CurrentUnit.GetComponent<Combatant>();
        Combatant target = FindClosestEnemy(self.transform.position);
        if (target == null)
        {
            return;
        }

        float d = Vector3.Distance(self.transform.position, target.transform.position);
        if (d <= self.AttackRange + 0.4f)
        {
            self.TryAttack(target);
        }
    }

    private static Combatant FindClosestEnemy(Vector3 position)
    {
        Combatant[] all = FindObjectsOfType<Combatant>();
        Combatant best = null;
        float bestD = float.MaxValue;

        for (int i = 0; i < all.Length; i++)
        {
            if (!all[i].IsAlive || all[i].Team != Combatant.Faction.Enemy)
            {
                continue;
            }

            float d = Vector3.Distance(position, all[i].transform.position);
            if (d < bestD)
            {
                bestD = d;
                best = all[i];
            }
        }

        return best;
    }
}
