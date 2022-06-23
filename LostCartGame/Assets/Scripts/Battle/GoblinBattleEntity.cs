using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBattleEntity : BattleEnemy
{
    BattlePartyMember target;

    public IEnumerator PickNewTarget()
    {
        target = BattlePartyManager.instance.GetRandomLivingMember();
        yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " is staring"));
        yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("at " + target.data.name));
    }

    public override IEnumerator RunTurn()
    {
        BattleTurnManager.instance.ClearBattleText();
        if (target == null || target.IsDead())
        {
            yield return StartCoroutine(PickNewTarget());
        }

        // If we couldn't find a target
        if(target == null)
        {
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " refuses to"));
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("fight..."));
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("PRESS A..."));
            yield return StartCoroutine(BattleTurnManager.instance.WaitForAnyKey());
            yield break;
        }

        // Goblin AI
        int targetPosition = BattlePartyManager.instance.GetPartyMemberIndex(target);
        if ((int)facing != targetPosition)
        {
            // Turn clockwise
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " turns"));
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("toward " + target.data.name));
            SetFacing((Direction)(facing + 1));
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("PRESS A..."));
            yield return StartCoroutine(BattleTurnManager.instance.WaitForAnyKey());
        }
        else
        {
            // Attack!
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " attacks "));
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(facing.ToString() + "!"));
            AttackReturn retVal =  BattlePartyManager.instance.CommitAttack(facing, data.baseAttackDamage);
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.baseAttackDamage + " damage!"));
            string hitIdentifier = "hit";
            if (retVal.isDead)
            {
                hitIdentifier = "defeated";
            }
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(retVal.name + " is " + hitIdentifier + "!"));
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("PRESS A..."));
            yield return StartCoroutine(BattleTurnManager.instance.WaitForAnyKey());
        }

        yield return null;
    }
}
