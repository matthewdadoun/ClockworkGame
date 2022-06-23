using UnityEngine;
using UnityEngine.SceneManagement;

public class PressKeyToSwitchScene : MonoBehaviour
{
    public string nextScene;
    public KeyCode key;
    void Update()
    {
        if(Input.GetKeyDown(key))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
