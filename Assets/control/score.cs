using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FindLatestScore : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "输入文本名");
        string latestScoreLine = null;
        int latestScore = 0;

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                latestScoreLine = line;
            }

            if (latestScoreLine != null)
            {
                string[] values = latestScoreLine.Split(',');
                if (values.Length > 1 && int.TryParse(values[1].Trim(), out latestScore))
                {
                    scoreText.text = "当前成绩：" + latestScore;
                }
                else
                {
                    Debug.LogError("Failed to parse the latest score.");
                }
            }
            else
            {
                Debug.LogError("The file is empty or could not be read.");
            }
        }
    }


}