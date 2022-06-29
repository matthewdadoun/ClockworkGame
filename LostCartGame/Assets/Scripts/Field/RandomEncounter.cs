using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RandomEncounter : MonoBehaviour
{
    // persistent amount of distance traveled
    float distanceTraveled;

    // player movement component
    PlayerMovement playerMovement;

    // percentage increase each time 
    [SerializeField] float encounterIncreaseDelta;

    // percentage by which the likelihood a random encounter will start this check
    float encounterChance;

    // amount of units to move to reach the first random encounter check
    [SerializeField] int startingDistanceThreshold;

    // a value that constantly updates per encounter check to check if the distance has passed this by 1 unit
    int lastDistThreshold;

    // modifier which determines how often encounters should be checked
    [SerializeField] float encounterMultiplier;

    [SerializeField] AudioSource fieldMusic, encounterMusic;

    [SerializeField] Animator encounterAnimator;


    void Start()
    {
        if (startingDistanceThreshold > 0)
        {
            lastDistThreshold = startingDistanceThreshold - 1;
        }

        if (encounterIncreaseDelta <= 0)
        {
            Debug.Log("Invalid increase delta. Setting to 5%");
            encounterIncreaseDelta = 0.05f;
        }
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void LateUpdate()
    {
        distanceTraveled += playerMovement.movement.magnitude * playerMovement.movementSpeed * Time.deltaTime * encounterMultiplier;
        if (distanceTraveled - lastDistThreshold > 1)
        {
            lastDistThreshold = Mathf.FloorToInt(distanceTraveled);

            if (Random.value < encounterChance)
            {
                EnemyEncounter();
            }

            else
                encounterChance += encounterIncreaseDelta;
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
        for (int i = 0; i < playerMovement.party.Count; i++)
        {
            PlayerDataManager.instance.StorePartyMemberPositionDirectionAndQueue(playerMovement.party[i].transform.position, playerMovement.GetPartyDirection(i), playerMovement.GetPartyQueue(i), i);
        }

        StartCoroutine(BattleAnimation());

    }
}
