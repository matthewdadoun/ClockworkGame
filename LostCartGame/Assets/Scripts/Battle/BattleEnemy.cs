using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy : BattleEntity
{
    protected override void OnStart()
    {
        SetFacing((Direction)Random.Range(0, 4));
    }

    public override IEnumerator RunTurn()
    {
        yield return null;
    }

}
