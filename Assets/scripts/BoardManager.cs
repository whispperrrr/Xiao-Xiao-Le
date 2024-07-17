using UnityEngine;
using System.Collections.Generic;
using System;

public class BoardManager : MonoBehaviour
{
    public GameObject[] blockPrefabs; // 五种方块预制体数组，假设第0个是空预制体
    public GameObject tilemap; // Tilemap GameObject
    public int boardSize = 9; // 棋盘大小

    private System.Random random = new System.Random();
    private GameObject[,] grid; // 存储棋盘上方块的引用
    private bool[,] matched; // 存储已匹配的方块状态

    void Start()
    {
        // 初始化棋盘
        CreateBoard();
        // 在棋盘上随机生成方块
        RandomlyGenerateBlocks();
    }

    void CreateBoard()
    {
        // 创建棋盘数组
        grid = new GameObject[boardSize, boardSize];
        matched = new bool[boardSize, boardSize];

        // 计算棋盘中心点的位置，并设置Tilemap的位置
        float xOffset = -((float)boardSize - 1) / 2f;
        float yOffset = -((float)boardSize - 1) / 2f;
        tilemap.transform.position = new Vector3(xOffset, yOffset, 0);

        // 填充棋盘背景
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                // 实例化背景方块
                GameObject background = Instantiate(blockPrefabs[0],
                    new Vector3(x + xOffset, y + yOffset, 0), Quaternion.identity, tilemap.transform);
                grid[x, y] = background;
            }
        }
    }

    void RandomlyGenerateBlocks()
    {
        int blocksToGenerate = boardSize * boardSize / 2; // 初始生成一半的方块

        for (int i = 0; i < blocksToGenerate; i++)
        {
            int x = random.Next(boardSize);
            int y = random.Next(boardSize);

            // 确保不在棋盘外和已生成的位置上生成新的方块
            while (x < 0 || x >= boardSize || y < 0 || y >= boardSize || grid[x, y] == null)
            {
                x = random.Next(boardSize);
                y = random.Next(boardSize);
            }

            GameObject block = Instantiate(blockPrefabs[random.Next(1, blockPrefabs.Length)],
                new Vector3(x + grid[x, y].transform.position.x, y + grid[x, y].transform.position.y, 0),
                Quaternion.identity, tilemap.transform);
            grid[x, y] = block;

            // 检查并消除匹配的方块
            CheckMatchesAndRemove(x, y);
        }
    }

    void CheckMatchesAndRemove(int x, int y)
    {
        // 检查水平方向的匹配
        if (CheckAndMarkMatches(x, y, 0, 1))
        {
            RemoveMatchedBlocks();
            FillVacantSlots();
            return;
        }

        // 检查垂直方向的匹配
        if (CheckAndMarkMatches(x, y, 1, 0))
        {
            RemoveMatchedBlocks();
            FillVacantSlots();
        }
    }

    bool CheckAndMarkMatches(int startX, int startY, int dx, int dy)
    {
        int count = 1; // 当前方块

        // 检查一个方向
        for (int i = 1; i < boardSize; i++)
        {
            int nextX = startX + i * dx;
            int nextY = startY + i * dy;

            if (!IsWithinBoard(nextX, nextY) || !AreSameBlocks(grid[startX, startY], grid[nextX, nextY]))
            {
                break;
            }

            matched[nextX, nextY] = true;
            count++;
        }

        // 至少需要3个方块才能匹配
        if (count >= 3)
        {
            matched[startX, startY] = true;
            return true;
        }

        return false;
    }

    bool IsWithinBoard(int x, int y)
    {
        return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
    }

    bool AreSameBlocks(GameObject block1, GameObject block2)
    {
        if (block1 == null || block2 == null) return false;
        return block1.GetComponent<SpriteRenderer>().color == block2.GetComponent<SpriteRenderer>().color;
    }

    void RemoveMatchedBlocks()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (matched[x, y])
                {
                    Destroy(grid[x, y]);
                    grid[x, y] = null;
                }
            }
        }

        // 重置匹配状态
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                matched[x, y] = false;
            }
        }
    }

    void FillVacantSlots()
    {
        // 从底部开始，将上面的方块下移填充空位
        for (int y = boardSize - 1; y > 0; y--)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (grid[x, y] == null)
                {
                    // 找到空位，从上面开始寻找第一个非空方块
                    for (int fillY = y - 1; fillY >= 0; fillY--)
                    {
                        if (grid[x, fillY] != null)
                        {
                            // 移动方块到空位
                            grid[x, y] = grid[x, fillY];
                            grid[x, fillY] = null;
                            Vector3 targetPosition = grid[x, y].transform.position;
                            targetPosition.y += 1; // 向上移动一格
                            grid[x, y].transform.position = targetPosition;
                            break;
                        }
                    }
                }
            }
        }

        // 在顶部生成新方块
        for (int x = 0; x < boardSize; x++)
        {
            if (grid[x, 0] == null)
            {
                GameObject newBlock = Instantiate(blockPrefabs[random.Next(1, blockPrefabs.Length)],
                    new Vector3(x, 0, 0), Quaternion.identity, tilemap.transform);
                grid[x, 0] = newBlock;
            }
        }
    }

    
}