using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleEntityFactory
{
    public static BattleEntity CreateBattleEntity(BattleEntityData data)
    {
        GameObject obj = new GameObject();
        obj.name = data.name;
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        Animator animator = obj.AddComponent<Animator>();
        animator.runtimeAnimatorController = data.animatorController;

        GameObject healthBar = GameObject.Instantiate(data.healthBarPrefab, data.healthBarOffset, Quaternion.identity, obj.transform);

        System.Type type;
        if (data.type == BattleEntityType.Player)
        {
            type = typeof(BattlePartyMember);
        }
        else
        {
            type = typeof(BattleEnemy);
            switch (data.aiType)
            {
                case BattleAIType.Clock:
                    type = typeof(ClockBattleEntity);
                    break;
                case BattleAIType.Gear:
                    type = typeof(GearBattleEntity);
                    break;
                case BattleAIType.Goblin:
                    type = typeof(GoblinBattleEntity);
                    break;
            }
        }

        BattleEntity be = (BattleEntity)obj.AddComponent(type);
        be.data = data;
        be.name = data.name;
        be.healthBarAnimator = healthBar.GetComponent<Animator>();
        be.entityAnimator = animator;

        return be;
    }
}
