using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePartyMember : BattleEntity
{
    public enum SelectedAction
    {
        Attack,
        Turn,
        Pass,
        COUNT
    }

    public SelectedAction selectedAction;
    protected override void OnStart()
    {
        int index = BattlePartyManager.instance.GetPartyMemberIndex(this);
        SetFacing(BattlePartyManager.instance.GetFacing(index));
    }

    public override void StartTurn()
    {
        selectedAction = SelectedAction.Attack;

        BattlePartyManager.instance.ShowArrowAt(BattlePartyManager.instance.GetPartyMemberIndex(this));
    }

    public override IEnumerator RunTurn()
    {
        List<string> selectorContents = new List<string>();
        for (int i = 0; i < (int)SelectedAction.COUNT; i++)
        {
            selectorContents.Add(((SelectedAction)i).ToString());
        }

        BattleTurnManager.instance.DisplaySelector(selectorContents, (int)selectedAction);

        // Action selection loop
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                int sel = (int)selectedAction + 1;
                if (sel == (int)SelectedAction.COUNT)
                {
                    sel = 0;
                }

                selectedAction = (SelectedAction)sel;
                BattleTurnManager.instance.DisplaySelector(selectorContents, (int)selectedAction);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                int sel = (int)selectedAction - 1;
                if (sel < 0)
                {
                    sel = (int)SelectedAction.COUNT - 1;
                }

                selectedAction = (SelectedAction)sel;
                BattleTurnManager.instance.DisplaySelector(selectorContents, (int)selectedAction);
            }

            yield return null;
        }

        yield return null;

        // Action performed loop
        switch (selectedAction)
        {
            case SelectedAction.Attack:
                yield return StartCoroutine(AttackSelectionLoop());
                BattlePartyManager.instance.HideArrow();
                break;
            case SelectedAction.Turn:
                // Move each character to the next point
                BattlePartyManager.instance.HideArrow();
                yield return StartCoroutine(BattlePartyManager.instance.TurnParty());
                break;
            case SelectedAction.Pass:
                BattlePartyManager.instance.HideArrow();
                yield return null;
                break;
        }

    }

    public IEnumerator AttackSelectionLoop()
    {
        int selectedEnemy = 0;
        List<BattleEnemy> enemies = new List<BattleEnemy>(BattleTurnManager.instance.enemies);

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDead())
            {
                enemies.RemoveAt(i);
                i--;
            }
        }

        List<string> selectorContents = new List<string>();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDead())
            {
                continue;
            }

            selectorContents.Add(enemies[i].name);
        }

        BattleTurnManager.instance.DisplaySelector(selectorContents, selectedEnemy);
        int realEnemyIndex = BattleTurnManager.instance.GetEnemyIndex(enemies[selectedEnemy]);
        BattleTurnManager.instance.ShowEnemyArrow(realEnemyIndex);

        // Action selection loop
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedEnemy++;
                if (selectedEnemy >= enemies.Count)
                {
                    selectedEnemy = 0;
                }

                realEnemyIndex = BattleTurnManager.instance.GetEnemyIndex(enemies[selectedEnemy]);
                BattleTurnManager.instance.DisplaySelector(selectorContents, selectedEnemy);
                BattleTurnManager.instance.ShowEnemyArrow(realEnemyIndex);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedEnemy--;
                if (selectedEnemy <= 0)
                {
                    selectedEnemy = enemies.Count - 1;
                }

                realEnemyIndex = BattleTurnManager.instance.GetEnemyIndex(enemies[selectedEnemy]);
                BattleTurnManager.instance.DisplaySelector(selectorContents, selectedEnemy);
                BattleTurnManager.instance.ShowEnemyArrow(realEnemyIndex);
            }

            yield return null;
        }

        // Perform the attack
        realEnemyIndex = BattleTurnManager.instance.GetEnemyIndex(enemies[selectedEnemy]);
        BattleTurnManager.instance.ClearBattleText();
        yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.name + " attacks!"));
        BattleTurnManager.instance.AttackEnemy(realEnemyIndex, data.baseAttackDamage);
        yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(data.baseAttackDamage + " damage!"));
        string deathString = "hit";
        if (enemies[selectedEnemy].IsDead())
        {
            deathString = "defeated";
        }
        yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed(enemies[selectedEnemy].data.name + " is " + deathString + "!"));
        yield return StartCoroutine(BattleTurnManager.instance.PrintTextDelayed("PRESS A..."));
        yield return StartCoroutine(BattleTurnManager.instance.WaitForAnyKey());


        BattleTurnManager.instance.HideEnemyArrow();
    }
}
