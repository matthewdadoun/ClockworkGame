using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    Vector3 playerPos;
    Vector2 playerDir;
    public static PlayerDataManager instance;
    public List<Vector3> partyMemPos;
    public List<Vector2> partyMemDir; 
    public List <Queue<Vector3>> partyMemQueue = new List<Queue<Vector3>>(); 


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        playerPos = new Vector3(0, 0, -0.01f);
        playerDir = new Vector2(0, -1);

        for(int i = 0; i<3; i++)
        {
            partyMemQueue.Add(new Queue<Vector3>());
        }
        
    }

    public void StorePlayerPositionAndDirection(Vector3 position, Vector2 direction)
    {
        playerPos = position;
        playerDir = direction;
    }

    public void StorePartyMemberPositionDirectionAndQueue(Vector3 position, Vector2 direction, Queue<Vector3> queue, int partyMemberIndex)
    {
        partyMemPos[partyMemberIndex] = position;
        partyMemDir[partyMemberIndex] = direction;
        partyMemQueue[partyMemberIndex] = queue;
    }

    public Vector3 LoadPlayerPosition()
    {
        return playerPos;
    }

    public Vector2 LoadPlayerDirection()
    {
        return playerDir;
    }

    public Vector3 LoadPartyPosition(int partyMemberIndex)
    {
        return partyMemPos[partyMemberIndex];
    }

    public Vector2 LoadPartyDirection(int partyMemberIndex)
    {
        return partyMemDir[partyMemberIndex];
    }

    public Queue<Vector3> LoadPartyQueue(int partyMemberIndex)
    {
        return new Queue<Vector3>(partyMemQueue[partyMemberIndex]);
    }

}
