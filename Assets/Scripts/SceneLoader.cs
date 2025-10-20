using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;
    void Start()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
