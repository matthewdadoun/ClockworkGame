using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
    COUNT
}

public struct AttackReturn
{
    public string name;
    public bool isDead;
    public bool wasDead;
}

public class BattlePartyManager : MonoBehaviour
{
    public static BattlePartyManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }


    [System.Serializable]
    public struct PartyPosition
    {
        public BattlePartyMember memberAtPos;
        public Vector2 position;
    }
    public PartyPosition[] partyPositions;

    public GameObject arrow;

    public float turnSpeedModifier;

    private void Start()
    {
        ResetPlayerPositions();
    }

    public void SavePlayerHealthValues()
    {
        for (int i = 0; i < partyPositions.Length; i++)
        {
            PersistentHealthManager.instance.partyHealth[i] = BattleTurnManager.instance.party[i].health;
        }
    }

    private void ResetPlayerPositions()
    {
        foreach (var playerPos in partyPositions)
        {
            playerPos.memberAtPos.transform.position = playerPos.position;
        }
    }

    public AttackReturn CommitAttack(Direction direction, int amount)
    {
        AttackReturn returnValue = new AttackReturn();
        BattlePartyMember member = partyPositions[(int)direction].memberAtPos;
        if (member)
        {
            returnValue.wasDead = returnValue.isDead;
            if (!member.IsDead())
            {
                member.TakeDamage(amount);
            }
        }
        returnValue.name = member.data.name;
        returnValue.isDead = member.IsDead();

        return returnValue;
    }

    public void HideArrow()
    {
        arrow.SetActive(false);
    }

    public void ShowArrowAt(int i)
    {
        arrow.SetActive(true);
        if (i >= 0 && i < partyPositions.Length)
        {
            arrow.transform.position = partyPositions[i].position - new Vector2(0.2f, 0.0f);
        }
    }

    public int GetPartyMemberIndex(BattlePartyMember member)
    {
        for (int i = 0; i < partyPositions.Length; i++)
        {
            if (partyPositions[i].memberAtPos == member)
            {
                return i;
            }
        }
        return 0;
    }

    public Direction GetFacing(int index)
    {
        switch (index)
        {
            case 0:
                return Direction.Down;
            case 1:
                return Direction.Left;
            case 2:
                return Direction.Up;
            case 3:
                return Direction.Right;
        }

        return Direction.Down;
    }

    public IEnumerator TurnParty()
    {
        // Set animators to walking
        for (int i = 0; i < partyPositions.Length; i++)
        {
            partyPositions[i].memberAtPos.SetWalking(true);
        }

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * turnSpeedModifier;
            if (t > 1)
            {
                t = 1;
            }

            for (int i = 0; i < partyPositions.Length; i++)
            {
                int nextPos = i + 1;
                if (nextPos >= partyPositions.Length)
                {
                    nextPos = 0;
                }

                partyPositions[i].memberAtPos.transform.position = Vector2.Lerp(partyPositions[i].position, partyPositions[nextPos].position, t);
            }

            yield return null;
        }

        // Swap the party's actual positions in the party positions array
        BattlePartyMember lastMember = partyPositions[partyPositions.Length - 1].memberAtPos;
        for (int i = partyPositions.Length - 1; i > 0; i--)
        {
            partyPositions[i].memberAtPos = partyPositions[i - 1].memberAtPos;
        }
        partyPositions[0].memberAtPos = lastMember;

        // Set animators to stop walking and update facing
        for (int i = 0; i < partyPositions.Length; i++)
        {
            partyPositions[i].memberAtPos.SetWalking(false);
            partyPositions[i].memberAtPos.SetFacing(GetFacing(i));
        }
    }

    public static Vector2 DirectionToVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Right:
                return Vector2.right;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
        }
        return Vector2.zero;
    }

    public static Direction NextDirectionClockwise(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Direction.Right;
            case Direction.Right:
                return Direction.Down;
            case Direction.Down:
                return Direction.Left;
            case Direction.Left:
                return Direction.Up;
        }

        return Direction.Up;
    }

    public BattlePartyMember GetRandomLivingMember()
    {
        int initialIndex = Random.Range(0, partyPositions.Length);
        int index = initialIndex;
        BattlePartyMember selectedMember = partyPositions[index].memberAtPos;
        while (selectedMember == null)
        {
            index++;
            if (index >= partyPositions.Length)
            {
                index = 0;
            }

            if (index == initialIndex)
            {
                return null;
            }

            selectedMember = partyPositions[index].memberAtPos;
        }

        return selectedMember;
    }
}
