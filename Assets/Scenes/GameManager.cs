using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float gameTime = 60f;        // ��Ϸ��ʱ�䣬��λ��
    public float moveTimeLimit = 5f;    // ÿ���ƶ���ʱ�����ƣ���λ��

    public Text timeText;               // ��ʾʣ��ʱ���UI�ı�
    private float currentTime;          // ��ǰʣ��ʱ��
    private float moveTimer;            // �ƶ���ʱ��

    private bool isGameActive = false;  // ��Ϸ�Ƿ񼤻��У����ڽ����У�

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
            // ����ʱ��
            currentTime -= Time.deltaTime;
            moveTimer -= Time.deltaTime;

            // ����Ƿ���Ҫ���¼�ʱ�ƶ�ʱ������
            if (moveTimer <= 0f)
            {
                // ���¼�ʱ
                moveTimer = moveTimeLimit;
            }

            // ����UI��ʾ
            UpdateTimeUI();

            // �����Ϸ�Ƿ����
            if (currentTime <= 0f)
            {
                EndGame();
            }
        }

        void StartGame()
        {
            // ��ʼ��Ϸ��ť�ĵ���¼�������У�
            isGameActive = true;
        }

        void EndGame()
        {
            // ��Ϸ�����߼�
            isGameActive = false;
            SceneManager.LoadScene("EndScene");  // ��ת����Ϸ�����������ǵ���Build Settings����Ӹó���
        }
    }
}

