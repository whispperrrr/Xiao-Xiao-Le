using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间

public class PlayerInput : MonoBehaviour
{
    public GameObject[] blockPrefabs; // 五种方块预制体数组
    public GameObject tilemap; // Tilemap GameObject
    public GameObject scoreText; // 分数显示的UI Text组件
    private int score = 0; // 玩家分数

    private GameObject selectedBlock1, selectedBlock2; // 存储选中的两个方块

    private System.Random random = new System.Random();
    int boardSize;
    bool[,] matched;
    private void Start()
    {
        BoardManager boardManager = new BoardManager();
        boardSize = boardManager.boardSize;

    }
    private void Update()
    {
        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject.CompareTag("Block"))
                {
                    if (selectedBlock1 == null)
                    {
                        // 第一次点击，选择第一个方块
                        selectedBlock1 = clickedObject;
                    }
                    else
                    {
                        // 第二次点击，选择第二个方块并尝试交换
                        selectedBlock2 = clickedObject;

                        // 检查两个方块是否相邻
                        if (AreAdjacent(selectedBlock1, selectedBlock2))
                        {
                            // 交换方块
                            SwapBlocks(selectedBlock1, selectedBlock2);

                            // 假设我们有一个方法来检查交换是否有效，并返回消除的方块数量
                            int matchesCount = CheckSwapValidity();

                            if (matchesCount > 0)
                            {
                                // 如果交换有效，计算分数
                                score += matchesCount - 2; // 根据消除的方块数量获得分数
                                UpdateScoreUI();
                            }
                            else
                            {
                                // 如果交换无效，撤销交换
                                SwapBlocks(selectedBlock1, selectedBlock2); // 再次交换以撤销交换
                            }
                        }

                        // 重置选中状态
                        selectedBlock1 = null;
                        selectedBlock2 = null;
                    }
                }
            }
        }
    }

    // 存储棋盘上方块的引用
    private GameObject[,] grid;
    //......

    // 检查方块交换是否有效的方法
    private int CheckSwapValidity()
    {
        if (selectedBlock1 == null || selectedBlock2 == null)
            return 0;

        // 获取方块的坐标
        int x1 = (int)selectedBlock1.transform.position.x;
        int y1 = (int)selectedBlock1.transform.position.y;
        int x2 = (int)selectedBlock2.transform.position.x;
        int y2 = (int)selectedBlock2.transform.position.y;

        // 保存交换前的方块引用
        GameObject blockRef1 = grid[x1, y1];
        GameObject blockRef2 = grid[x2, y2];

        // 模拟交换
        grid[x1, y1] = blockRef2;
        grid[x2, y2] = blockRef1;

        // 检查交换后是否形成匹配
        int matchesCount = 0;

        // 检查水平方向的匹配
        matchesCount += CheckMatchesInDirection(x1, y1, 0, 1);
        if (matchesCount >= 3)
        {
            MarkMatches(x1, y1, 0, 1);
        }

        // 检查垂直方向的匹配
        matchesCount += CheckMatchesInDirection(x2, y2, 1, 0);
        if (matchesCount >= 3)
        {
            MarkMatches(x2, y2, 1, 0);
        }

        // 恢复交换前的方块引用
        grid[x1, y1] = blockRef1;
        grid[x2, y2] = blockRef2;

        // 如果没有形成新的匹配，则返回0
        return matchesCount;
    }

    private int CheckMatchesInDirection(int startX, int startY, int dx, int dy)
    {
        int count = 0;
        int x = startX;
        int y = startY;

        // 找到第一个匹配的方块
        while (x >= 0 && x < boardSize && y >= 0 && y < boardSize && AreSameBlocks(grid[x, y], grid[startX, startY]))
        {
            count++;
            x += dx;
            y += dy;
        }

        // 如果形成匹配，返回匹配的数量，否则返回0
        return count;
    }

    private bool AreSameBlocks(GameObject block1, GameObject block2)
    {
        return block1 != null && block2 != null && block1.GetComponent<SpriteRenderer>().color == block2.GetComponent<SpriteRenderer>().color;
    }

    private void MarkMatches(int startX, int startY, int dx, int dy)
    {
        int matchCount = 0;
        int x = startX;
        int y = startY;

        while (x >= 0 && x < boardSize && y >= 0 && y < boardSize && AreSameBlocks(grid[x, y], grid[startX, startY]))
        {
            matched[x, y] = true;
            matchCount++;
            x += dx;
            y += dy;
        }
    }

    // 交换两个方块的位置
    private void SwapBlocks(GameObject block1, GameObject block2)
    {
        Vector3 tempPosition = block1.transform.position;
        block1.transform.position = block2.transform.position;
        block2.transform.position = tempPosition;
    }

    // 检查两个方块是否相邻
    private bool AreAdjacent(GameObject block1, GameObject block2)
    {
        int deltaX = Mathf.Abs((int)block1.transform.position.x - (int)block2.transform.position.x);
        int deltaY = Mathf.Abs((int)block1.transform.position.y - (int)block2.transform.position.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    // 更新分数显示的方法
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.GetComponent<Text>().text = "Score: " + score;
        }
    }
}