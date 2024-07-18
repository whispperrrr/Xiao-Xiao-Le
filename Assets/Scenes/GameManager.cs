using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float gameTime = 60f;        // 游戏总时间，单位秒
    public float moveTimeLimit = 5f;    // 每次移动的时间限制，单位秒

    public Text timeText;               // 显示剩余时间的UI文本
    private float currentTime;          // 当前剩余时间
    private float moveTimer;            // 移动计时器

    private bool isGameActive = false;  // 游戏是否激活中（即在进行中）

    void UpdateTimeUI()
    {
        timeText.text = "Time: " + Mathf.Round(currentTime).ToString();
    }
    void Start()
    {
        currentTime = gameTime;
        moveTimer = moveTimeLimit;
        UpdateTimeUI();

    }

    // Update is called once per frame
    void Update()
    {
        if (isGameActive)
        {
            // 更新时间
            currentTime -= Time.deltaTime;
            moveTimer -= Time.deltaTime;

            // 检查是否需要重新计时移动时间限制
            if (moveTimer <= 0f)
            {
                // 重新计时
                moveTimer = moveTimeLimit;
            }

            // 更新UI显示
            UpdateTimeUI();

            // 检查游戏是否结束
            if (currentTime <= 0f)
            {
                EndGame();
            }
        }

        void StartGame()
        {
            // 开始游戏按钮的点击事件（如果有）
            isGameActive = true;
        }

        void EndGame()
        {
            // 游戏结束逻辑
            isGameActive = false;
            SceneManager.LoadScene("EndScene");  // 跳转至游戏结束场景，记得在Build Settings中添加该场景
        }
    }
}

