using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    //ʹ�ó�������������  
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}