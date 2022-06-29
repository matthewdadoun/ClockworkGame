//script loosely based on https://gamedev.stackexchange.com/questions/178164/2d-party-follow-the-leader-in-unity
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLeader : MonoBehaviour
{
    [SerializeField] GameObject leaderToFollow;
    [SerializeField] int framesToStayBehind;
    PlayerMovement playerMovement;

    public Queue<Vector3> record = new Queue<Vector3>();
    private Vector3 lastRecord;

    public Animator partyAnim;

    private void Start()
    {
        playerMovement = leaderToFollow.GetComponent<PlayerMovement>();
    }
    private void FixedUpdate()
    {
        partyAnim.SetBool("IsMoving", playerMovement.movement.magnitude > 0.5f);
        if (playerMovement.movement.magnitude > 0.5f)
        {
            //record position of leader
            record.Enqueue(leaderToFollow.transform.position);
            //remove last position from queue and use it as our own
            if (record.Count > framesToStayBehind)
            {
                Vector3 nextPos = record.Dequeue();
                var movement = Vector3.Normalize(nextPos - this.transform.position);
                partyAnim.SetFloat("MoveX", movement.x);
                partyAnim.SetFloat("MoveY", movement.y);
                this.transform.position = nextPos;
            }
        }
    }
}
