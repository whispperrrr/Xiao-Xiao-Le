using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    //使用场景名称来加载  
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}