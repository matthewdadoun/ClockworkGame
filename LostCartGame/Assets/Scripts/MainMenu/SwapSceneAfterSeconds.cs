using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapSceneAfterSeconds : MonoBehaviour
{
    public string nextScene;
    public float seconds;

    private void Start()
    {
        StartCoroutine(SwapAfterSeconds());
    }

    IEnumerator SwapAfterSeconds()
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(nextScene);
    }
}
