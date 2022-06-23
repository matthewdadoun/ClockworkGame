using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickupp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PersistentHealthManager.instance.FullHeal();
        Destroy(this.gameObject);
    }
}
