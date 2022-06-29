using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public Rigidbody2D playerRB;
    public Vector2 movement;
    public List<GameObject> party;

    public Animator playerAnim;
    public bool canMove;

    void Start()
    {
        this.transform.position = PlayerDataManager.instance.LoadPlayerPosition();
        for (int i = 0; i < party.Count; i++)
        {
            party[i].transform.position = PlayerDataManager.instance.LoadPartyPosition(i);
            var partyAnimator = party[i].GetComponent<FollowLeader>().partyAnim;
            partyAnimator.SetFloat("MoveX", PlayerDataManager.instance.LoadPartyDirection(i).x);
            partyAnimator.SetFloat("MoveY", PlayerDataManager.instance.LoadPartyDirection(i).y);
            party[i].GetComponent<FollowLeader>().record = PlayerDataManager.instance.LoadPartyQueue(i);
        }
        playerAnim.SetFloat("MoveX", PlayerDataManager.instance.LoadPlayerDirection().x);
        playerAnim.SetFloat("MoveY", PlayerDataManager.instance.LoadPlayerDirection().y);
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // prevents diagonal movement
            if (movement.x != 0)
            {
                movement.y = 0;
            }
            else if (movement.y != 0)
            {
                movement.x = 0;
            }

            bool isMoving = movement.magnitude > 0.5f;

            if (isMoving)
            {
                playerAnim.SetFloat("MoveX", movement.x);
                playerAnim.SetFloat("MoveY", movement.y);
            }

            playerAnim.SetBool("IsMoving", isMoving);

            playerRB.velocity = movement * movementSpeed;
        }
    }

    public Vector2 GetDirection()
    {
        var x = playerAnim.GetFloat("MoveX");
        var y = playerAnim.GetFloat("MoveY");
        return new Vector2(x, y);
    }

    public Vector2 GetPartyDirection(int partyMemberIndex)
    {
        var partyAnimator = party[partyMemberIndex].GetComponent<FollowLeader>().partyAnim;
        var x = partyAnimator.GetFloat("MoveX");
        var y = partyAnimator.GetFloat("MoveY");
        return new Vector2(x, y);
    }

    public Queue<Vector3> GetPartyQueue(int partyMemberIndex)
    {
        return new Queue<Vector3>(party[partyMemberIndex].GetComponent<FollowLeader>().record);
    }
}
