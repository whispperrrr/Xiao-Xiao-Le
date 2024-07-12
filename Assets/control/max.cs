using System.Collections;  
using System.Collections.Generic;  
using System.IO;  
using UnityEngine;
using UnityEngine.UI;

public class FindMaxScore : MonoBehaviour  
{
    public Text maxText;
    void Start()  
    {  
        string filePath = Path.Combine(Application.streamingAssetsPath, "输入文件"); 
        int maxScore = int.MinValue; 
        string line;  
  
        using (StreamReader reader = new StreamReader(filePath))  
        {  
            
            reader.ReadLine();  
  
            while ((line = reader.ReadLine()) != null)  
            {  
                string[] values = line.Split(',');  
                int score;  
  
                if (int.TryParse(values[1].Trim(), out score))  
                {  
                   
                    if (score > maxScore)  
                    {  
                        maxScore = score;  
                    }  
                }  
            }  
        }

        maxText.text=("历史最高:" + maxScore);  
    }  
}