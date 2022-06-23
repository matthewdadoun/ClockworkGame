using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBattleEntity : BattleEnemy
{
    protected override void OnStart()
    {
        horizontal = Random.value > 0.5f;
    }

    bool horizontal = false;

    public override IEnumerator RunTurn()
    {
        // Gear AI
        BattleTurnManager.instance.ClearBattleText();
        if (horizontal)
        {
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " attacks!"));
            AttackReturn hit1 = BattlePartyManager.instance.CommitAttack(Direction.Right, data.baseAttackDamage);
            AttackReturn hit2 = BattlePartyManager.instance.CommitAttack(Direction.Left, data.baseAttackDamage);
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("Hit " + hit1.name + " and " + hit2.name + "!"));
            if (hit1.isDead && !hit1.wasDead)
            {
                yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(hit1.name + " is defeated!"));
            }
            if (hit2.isDead && !hit2.wasDead)
            {
                yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(hit2.name + " is defeated!"));
            }
        }
        else
        {
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " attacks!"));
            AttackReturn hit1 = BattlePartyManager.instance.CommitAttack(Direction.Up, data.baseAttackDamage);
            AttackReturn hit2 = BattlePartyManager.instance.CommitAttack(Direction.Down, data.baseAttackDamage);
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("Hit " + hit1.name + " and " + hit2.name + "!"));
            if (hit1.isDead && !hit1.wasDead)
            {
                yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(hit1.name + " is defeated!"));
            }
            if (hit2.isDead && !hit2.wasDead)
            {
                yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(hit2.name + " is defeated!"));
            }
        }

        horizontal = !horizontal;

        yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("PRESS A..."));
        yield return StartCoroutine(BattleTurnManager.instance.WaitForAnyKey());
    }
}
