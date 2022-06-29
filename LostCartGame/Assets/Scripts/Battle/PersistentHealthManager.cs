using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentHealthManager : MonoBehaviour
{
    public static PersistentHealthManager instance;

    public List<int> partyHealth;
    public int partyMaxHealth;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance == this)
            {
                // Don't re-initialize manager
                return;
            }

            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        for (int i = 0; i < 4; i++)
        {
            partyHealth.Add(10);
        }
    }

    public void FullHeal()
    {
        for (int i = 0; i < partyHealth.Count; i++)
        {
            partyHealth[i] = partyMaxHealth;
        }
    }
}
