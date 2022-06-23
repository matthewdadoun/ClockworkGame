using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEntity : MonoBehaviour
{
    public BattleEntityData data;
    public int health = 10;
    public Animator healthBarAnimator;
    public Animator entityAnimator;
    protected Direction facing;
    private void Start()
    {
        OnStart();
    }
    public bool IsDead()
    {
        return health <= 0;
    }
    public void TakeDamage(int amount)
    {
        health -= amount;
        UpdateHealthbarAnimator();
    }

    public void UpdateHealthbarAnimator()
    {
        healthBarAnimator.SetFloat("InvHealthPercent", 1.0f - ((float)health) / ((float)data.maxHealth));
    }
    public void SetFacing(Direction direction)
    {
        if((int)direction >= (int)Direction.COUNT)
        {
            direction = Direction.Up;
        }

        facing = direction;
        Vector2 dir = BattlePartyManager.DirectionToVector(direction);
        entityAnimator.SetFloat("MoveX", dir.x);
        entityAnimator.SetFloat("MoveY", dir.y);
    }

    public void SetWalking(bool value)
    {
        entityAnimator.SetBool("IsMoving", value);
    }

    public virtual IEnumerator RunTurn() { Debug.LogError("RunTurn() called on base battle entity!"); yield return null; }
    public virtual void StartTurn() { }
    public virtual void EndTurn() { }

    protected virtual void OnStart() { }
}
