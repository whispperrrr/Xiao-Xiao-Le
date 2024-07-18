using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class timecontrol : MonoBehaviour
{
    public Text timerText;
    public float moveTimeLimit = 10f; // 移动时间限制，单位秒
    private float currentTime;
    private bool timerRunning;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = moveTimeLimit;
        timerRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0f)
            {
                EndGame();
            }
        }
    }
    void UpdateTimerDisplay()
    {
        timerText.text = "Time: " + currentTime.ToString("0");
    }
    public void PlayerMoved()
    {
        currentTime = moveTimeLimit;
    }

    void EndGame()
    {
        timerRunning = false;
        SceneManager.LoadScene("EndScene"); // 替换成你的结束场景的名称或索引
    }
}
