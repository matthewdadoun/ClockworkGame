using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleTurnManager : MonoBehaviour
{
    public static BattleTurnManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;

        GeneratePlayerCharacters();
        GenerateEnemies();
    }

    public enum BattlePhase
    {
        Party,
        Enemies
    }

    public List<BattleEntityData> potentialEnemies;
    public List<BattleEntityData> partyData;

    public TextMeshProUGUI battleText;
    private Queue<string> battleTextContent = new Queue<string>();

    public Animator exitAnimator;
    public GameObject enemySelectorArrow;

    [System.Serializable]
    public struct LISTVEC3INTERNAL
    {
        public List<Vector3> items;
    }

    public List<LISTVEC3INTERNAL> spawnPointsPerGroupSize;

    public float letterDelay = 0.1f;

    [HeaderAttribute("Debug view data")]
    public List<BattlePartyMember> party;
    public List<BattleEnemy> enemies;

    public BattlePhase phase = BattlePhase.Party;
    public int battleIndex = 0;


    private void Start()
    {
        StartCoroutine(MainTurnCoroutine());
    }

    IEnumerator MainTurnCoroutine()
    {
        yield return StartCoroutine(DisplayIntroText());
        yield return StartCoroutine(WaitForAnyKey());
        ClearBattleText();

        while (!IsBattleOver())
        {
            yield return null;
            yield return StartCoroutine(RunCurrentTurn());
            IncrementTurn();
        }

        BattlePartyManager.instance.SavePlayerHealthValues();

        exitAnimator.SetTrigger("TriggerExit");
        // Hardcoded 2 second delay because I know that's how long the battle transition is
        yield return new WaitForSeconds(2.0f);
        SwapToNextScene();
    }

    public IEnumerator WaitForAnyKey()
    {
        // Condition broken down for clarity sake
        // Check if any key is pressed AND it wasn't the mouse 
        while (! // Inverse
                (Input.anyKeyDown && // Any key pressed
                    !(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) // Not one of the mouse buttons
                )
             )
        {
            yield return null;
        }
    }

    private void GeneratePlayerCharacters()
    {
        for (int i = 0; i < 4; i++)
        {
            BattlePartyMember partyMember = (BattlePartyMember)BattleEntityFactory.CreateBattleEntity(partyData[i]);
            party.Add(partyMember);
            BattlePartyManager.instance.partyPositions[i].memberAtPos = partyMember;
            partyMember.health = PersistentHealthManager.instance.partyHealth[i];
            partyMember.UpdateHealthbarAnimator();
        }
    }

    private void GenerateEnemies()
    {
        int amount = Random.Range(1, 4);
        for (int i = 0; i < amount; i++)
        {
            BattleEnemy enemy = (BattleEnemy)BattleEntityFactory.CreateBattleEntity(potentialEnemies[Random.Range(0, potentialEnemies.Count)]);
            enemies.Add(enemy);
            enemy.transform.position = spawnPointsPerGroupSize[amount - 1].items[i];
            enemy.GetComponent<SpriteRenderer>().sortingOrder = i;

            if (amount == 3 && i == 0)
            {
                enemy.healthBarAnimator.gameObject.transform.localPosition = new Vector3(0.09f, -0.14f, 0);
            }
            else if (amount == 3 && i == 1)
            {
                enemy.healthBarAnimator.gameObject.transform.localPosition = new Vector3(-0.09f, -0.14f, 0);
            }

        }
    }

    private IEnumerator DisplayIntroText()
    {
        foreach (var enemy in enemies)
        {
            yield return StartCoroutine(PrintTextDelayed("A " + enemy.data.name + " appears!"));
        }
        yield return StartCoroutine(PrintTextDelayed("PRESS A..."));
    }

    public void DisplaySelector(List<string> contents, int selected)
    {
        ClearBattleText();

        for (int i = 0; i < contents.Count; i++)
        {
            if (i == selected)
            {
                PrintText("->" + contents[i]);
            }
            else
            {
                PrintText(contents[i]);
            }
        }
    }

    private IEnumerator RunCurrentTurn()
    {
        BattleEntity entity = null;
        switch (phase)
        {
            case BattlePhase.Party:
                entity = party[battleIndex];
                break;
            case BattlePhase.Enemies:
                entity = enemies[battleIndex];
                break;
        }
        if (entity == null)
        {
            yield break;
        }

        // Do not run turn for dead character
        if (entity.IsDead())
        {
            yield break;
        }

        entity.StartTurn();
        yield return StartCoroutine(entity.RunTurn());
        entity.EndTurn();
    }

    private void IncrementTurn()
    {
        battleIndex++;
        int count = 0;
        switch (phase)
        {
            case BattlePhase.Party:
                count = party.Count;
                break;
            case BattlePhase.Enemies:
                count = enemies.Count;
                break;
        }
        if (battleIndex >= count)
        {
            SwapPhases();
            battleIndex = 0;
        }
    }

    private void SwapPhases()
    {
        if (phase == BattlePhase.Enemies)
        {
            phase = BattlePhase.Party;
        }
        else
        {
            phase = BattlePhase.Enemies;
        }
    }

    private bool IsBattleOver()
    {
        return IsPartyDefeated() || AreEnemiesDefeated();
    }

    public void PrintText(string text)
    {
        battleTextContent.Enqueue(text);
        if (battleTextContent.Count > 4)
        {
            battleTextContent.Dequeue();
        }
        battleText.text = StringArrayToString(battleTextContent.ToArray());

    }

    public IEnumerator PrintTextDelayed(string text)
    {
        if (battleTextContent.Count > 3)
        {
            battleTextContent.Dequeue();
        }
        battleText.text = StringArrayToString(battleTextContent.ToArray());

        battleTextContent.Enqueue(text);

        // Add text to battleText letter by letter
        for (int i = 0; i < text.Length; i++)
        {
            yield return new WaitForSeconds(letterDelay);
            battleText.text += text[i];
        }
    }

    public string StringArrayToString(string[] stringArray)
    {
        string content = "";
        foreach (var str in stringArray)
        {
            content += str + "\n";
        }

        return content;
    }

    public void ClearBattleText()
    {
        battleTextContent.Clear();
        battleText.text = "";
    }

    public void SwapToNextScene()
    {
        string sceneName = "FieldScene";
        if (IsPartyDefeated())
        {
            sceneName = "GameOverScene";
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public bool IsPartyDefeated()
    {
        bool defeat = true;
        foreach (var member in party)
        {
            if (member.health > 0)
            {
                defeat = false;
            }
        }

        return defeat;
    }

    public bool AreEnemiesDefeated()
    {
        bool defeat = true;
        foreach (var enemy in enemies)
        {
            if (enemy.health > 0)
            {
                defeat = false;
            }
        }

        return defeat;
    }

    public int GetEnemyIndex(BattleEnemy enemy)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == enemy)
            {
                return i;
            }
        }

        return 0;
    }

    public void HideEnemyArrow()
    {
        enemySelectorArrow.SetActive(false);
    }

    public void ShowEnemyArrow(int enemyIndex)
    {
        enemySelectorArrow.SetActive(true);
        if (enemyIndex >= enemies.Count)
        {
            enemyIndex = 0;
        }
        enemySelectorArrow.transform.position = enemies[enemyIndex].transform.position + new Vector3(0, 0.2f, 0);
    }

    public AttackReturn AttackEnemy(int enemyIndex, int amount)
    {
        enemies[enemyIndex].TakeDamage(amount);
        AttackReturn retVal = new AttackReturn();

        retVal.name = enemies[enemyIndex].name;
        retVal.isDead = enemies[enemyIndex].IsDead();

        return retVal;
    }
}
