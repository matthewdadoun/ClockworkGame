using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleEntityType
{
    Player,
    Enemy
}

public enum BattleAIType
{ 
    Clock,
    Gear,
    Goblin
}

[CreateAssetMenu(fileName = "Battle Entity Data", menuName = "Data/Battle/Battle Entity Data")]
public class BattleEntityData : ScriptableObject
{
    public string name;
    public RuntimeAnimatorController animatorController;
    public BattleEntityType type;
    public BattleAIType aiType;
    public GameObject healthBarPrefab;
    public int maxHealth = 10;
    public Vector3 healthBarOffset;
    public int baseAttackDamage = 2;
}
