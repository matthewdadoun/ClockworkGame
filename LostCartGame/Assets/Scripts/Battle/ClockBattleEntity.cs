using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockBattleEntity : BattleEnemy
{
    public int turnsTicking = 0;

    public override IEnumerator RunTurn()
    {
        // Clock AI
        if(turnsTicking == 2)
        {
            BattleTurnManager.instance.ClearBattleText();
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " turns!"));

            SetFacing(BattlePartyManager.NextDirectionClockwise(facing));

            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " attacks " + facing.ToString() + "!"));
            AttackReturn retVal = BattlePartyManager.instance.CommitAttack(facing, data.baseAttackDamage);
            if (retVal.wasDead)
            {
                yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("Missed!"));
            }
            else
            {
                yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.baseAttackDamage + " damage!"));
                string hitIdentifier = "hit";
                if (retVal.isDead)
                {
                    hitIdentifier = "defeated";
                }
                yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(retVal.name + " is " + hitIdentifier + "!"));
            }
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("PRESS A..."));
            yield return StartCoroutine(BattleTurnManager.instance.WaitForAnyKey());

            turnsTicking = 0;
        }
        else
        {
            BattleTurnManager.instance.ClearBattleText();
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " is ticking..."));
            yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("PRESS A..."));
            yield return StartCoroutine(BattleTurnManager.instance.WaitForAnyKey());
            turnsTicking++;
        }

        yield return null;
    }
}
