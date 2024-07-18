using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using System;

public class GridManager : MonoBehaviour
{
    public GameObject[] elementPrefabs; // 预制体数组
    public static int rows = 9;
    public static int columns = 9;
    public float spacing = 1.0f;
    private TMP_Text scoreText;
    private TMP_Text timeText;
    GameObject[,] gameObjects = new GameObject[rows, columns];
    private GameObject firstSelected;
    private GameObject secondSelected;
    public UnityEngine.Color selectedColor = UnityEngine.Color.red;
    private UnityEngine.Color originalColor;

    float time = 30f;
    void Start()
    {
        scoreText = GameObject.Find("scoreText").GetComponent<TMP_Text>();
        timeText = GameObject.Find("timeText").GetComponent<TMP_Text>();
        GenerateGrid();

    }

    private void Update()
    {
        time -= Time.deltaTime;

        if (time < 0)
        {
            Application.Quit();
        }

        int val = (int)Math.Floor(time);
        timeText.text = "Time:"+val.ToString();
        if (Input.GetMouseButtonDown(0))
        {
            SelectElement();
        }
        MoveObjectsDownwards();
    }

    void SelectElement()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            if (firstSelected == null)
            {
                firstSelected = hit.collider.gameObject;
                originalColor = firstSelected.GetComponent<SpriteRenderer>().color;
                firstSelected.GetComponent<SpriteRenderer>().color = selectedColor;
            }
            else if (secondSelected == null && hit.collider.gameObject != firstSelected)
            {
                secondSelected = hit.collider.gameObject;
                if (AreAdjacent(firstSelected, secondSelected))
                {
                    SwapElements(firstSelected, secondSelected);
                    
                    CheckAndDestroy(firstSelected);
                    if(secondSelected != null)
                    CheckAndDestroy(secondSelected);

                    
                    MoveObjectsDownwards();

                }
                firstSelected.GetComponent<SpriteRenderer>().color = originalColor;
                firstSelected = null;
                secondSelected = null;
            }
        }
    }

    void MoveObjectsDownwards()
    {
        bool flag = true;
        while (flag)
        {
            flag = false;
            // 从顶部开始遍历二维数组
            for (int i = 1; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // 如果当前游戏对象不为空且下一行的游戏对象为空，则向下移动
                    if (gameObjects[i, j] != null && gameObjects[i - 1, j] == null)
                    {
                        flag = true;
                        // 移动当前游戏对象到下一行
                        gameObjects[i - 1, j] = gameObjects[i, j];
                        gameObjects[i, j] = null;

                        // 更新游戏对象的位置（示例：将位置向下移动）
                        if (gameObjects[i - 1, j] != null)
                        {
                            Vector3 newPosition = gameObjects[i - 1, j].transform.position;
                            newPosition.y -= 1.0f; // 示例：向下移动一个单位
                            gameObjects[i - 1, j].transform.position = newPosition;
                        }
                    }
                }
            }
        }
        
       
    }

    Point FindEle(GameObject gameObject)
    {
        int r = 0, c = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (gameObjects[i, j] == gameObject)
                {
                    r = i; c = j; break;
                }
            }

        }
        return new Point(r, c);
    }
    void desEle(Point point)
    {
        Destroy(gameObjects[point.X,point.Y]);
    }
    GameObject getEle(Point point)
    {
        return gameObjects[point.X, point.Y];
    }
    void CheckAndDestroy(GameObject gameObject)
    {
        List<Point> objectsToDestroyH = new List<Point>(); // 水平待销毁
        List<Point> objectsToDestroyV = new List<Point>(); // 竖直待销毁

        Point point = FindEle(gameObject);
        bool des = false;
        int r , c;
        r = point.X; c = point.Y;

        for (int i = c + 1; i < columns; i++)
        {
            Point newPoint = new Point(r, i);
            if (getEle(newPoint)!=null&&getEle(newPoint).tag == gameObject.tag)
            {
                objectsToDestroyH.Add(newPoint);
            }
            else break;
        }
        for (int i = c -1; i > -1; i--)
        {
            Point newPoint = new Point(r, i);
            if (getEle(newPoint) != null&&getEle(newPoint).tag == gameObject.tag)
            {
                objectsToDestroyH.Add(newPoint);
            }
            else break;
        }


        for (int i = r + 1; i < rows; i++)
        {
            Point newPoint = new Point(i, c);
            if (getEle(newPoint) != null&&getEle(newPoint).tag == gameObject.tag)
            {
                objectsToDestroyH.Add(newPoint);
            }
            else break;
        }

        for (int i = r - 1; i > -1; i--)
        {
            Point newPoint = new Point(i, c);
            if (getEle(newPoint) != null&&getEle(newPoint).tag == gameObject.tag)
            {
                objectsToDestroyH.Add(newPoint);
            }
            else break;
        }

        if (objectsToDestroyH.Count >= 2)
        {
            des = true;
            // 遍历列表中的每个对象，并销毁它
            foreach (Point p in objectsToDestroyH)
            {
                desEle(p);
                // 增加分数
                int score = int.Parse(scoreText.text.Substring(6)) + 1;
                scoreText.SetText(string.Concat("Score:", score.ToString()));
            }
        }
        if (objectsToDestroyV.Count >= 2)
        {
            des = true;
            // 遍历列表中的每个对象，并销毁它
            foreach (Point p in objectsToDestroyH)
            {
                desEle(p);
                // 增加分数
                int score = int.Parse(scoreText.text.Substring(6)) + 1;
                scoreText.SetText(string.Concat("Score:", score.ToString()));
            }
        }

        if(des)
        {
            if(gameObject!=null)
            Destroy(gameObject);
            // 增加分数
            int score = int.Parse(scoreText.text.Substring(6)) + 1;
            scoreText.SetText(string.Concat("Score:", score.ToString()));
        }
    }

   

    bool AreAdjacent(GameObject a, GameObject b)
    {
        Vector2 posA = a.transform.position;
        Vector2 posB = b.transform.position;

        float distance = Vector2.Distance(posA, posB);
        return Mathf.Approximately(distance, 1.0f);
    }

    void SwapElements(GameObject a, GameObject b)
    {
        Vector3 tempPosition = a.transform.position;
        a.transform.position = b.transform.position;
        b.transform.position = tempPosition;

        Point pa = FindEle(a);
        int ra = pa.X, ca = pa.Y;

        Point pb = FindEle(b);
        int rb = pb.X, cb = pb.Y;

        GameObject tmp = gameObjects[ra, ca];
        gameObjects[ra, ca] = gameObjects[rb, cb];
        gameObjects[rb, cb] = tmp;
    }



    void GenerateGrid()
    {
        float offsetX = (columns - 1) * spacing / 2;
        float offsetY = (rows - 1) * spacing / 2;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int randomIndex = UnityEngine.Random.Range(0, elementPrefabs.Length);
                GameObject element = Instantiate(elementPrefabs[randomIndex], transform);
                gameObjects[y, x] = element;
                element.transform.position = new Vector3(x * spacing - offsetX, y * spacing - offsetY, 0);
            }
        }
    }
}
