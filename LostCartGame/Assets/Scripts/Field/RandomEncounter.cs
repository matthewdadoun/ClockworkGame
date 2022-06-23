using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RandomEncounter : MonoBehaviour
{
    float distanceTraveled;
    PlayerMovement playerMovement;
    [SerializeField] float encounterChance;
    int lastDistThreshold;
    public float encounterMultiplier;
    public AudioSource fieldMusic, encounterMusic;
    public Animator encounterAnimator; 


    void Start()
    {
        distanceTraveled = 0;
        encounterChance = 0; 
        lastDistThreshold = 1;
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void LateUpdate()
    {
        distanceTraveled += playerMovement.movement.magnitude * playerMovement.movementSpeed*Time.deltaTime*encounterMultiplier;
        if(distanceTraveled-lastDistThreshold > 1)
        {
            lastDistThreshold = Mathf.FloorToInt(distanceTraveled);

            if (Random.value < encounterChance)
            {
                EnemyEncounter();
            }

            else
                encounterChance += 0.05f;
        }
    }

    IEnumerator BattleAnimation()
    {
        yield return new WaitForSeconds(2.2f);
        SceneManager.LoadScene("BattleScene");
    }

    void EnemyEncounter()
    {
        fieldMusic.Stop();
        encounterMusic.Play();
        encounterAnimator.SetBool("Encounter", true);
        playerMovement.canMove = false;
        playerMovement.movement = Vector2.zero;
        playerMovement.playerRB.velocity = Vector2.zero;
        PlayerDataManager.instance.StorePlayerPositionAndDirection(playerMovement.transform.position, playerMovement.GetDirection());
        for(int i = 0; i < playerMovement.party.Count; i++)
        {
            PlayerDataManager.instance.StorePartyMemberPositionDirectionAndQueue(playerMovement.party[i].transform.position, playerMovement.GetPartyDirection(i), playerMovement.GetPartyQueue(i), i);
        }

        StartCoroutine(BattleAnimation());
        
    }
}
