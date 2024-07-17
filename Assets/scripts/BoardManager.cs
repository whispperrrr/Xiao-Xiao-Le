using UnityEngine;
using System.Collections.Generic;
using System;

public class BoardManager : MonoBehaviour
{
    public GameObject[] blockPrefabs; // ���ַ���Ԥ�������飬�����0���ǿ�Ԥ����
    public GameObject tilemap; // Tilemap GameObject
    public int boardSize = 9; // ���̴�С

    private System.Random random = new System.Random();
    private GameObject[,] grid; // �洢�����Ϸ��������
    private bool[,] matched; // �洢��ƥ��ķ���״̬

    void Start()
    {
        // ��ʼ������
        CreateBoard();
        // ��������������ɷ���
        RandomlyGenerateBlocks();
    }

    void CreateBoard()
    {
        // ������������
        grid = new GameObject[boardSize, boardSize];
        matched = new bool[boardSize, boardSize];

        // �����������ĵ��λ�ã�������Tilemap��λ��
        float xOffset = -((float)boardSize - 1) / 2f;
        float yOffset = -((float)boardSize - 1) / 2f;
        tilemap.transform.position = new Vector3(xOffset, yOffset, 0);

        // ������̱���
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                // ʵ������������
                GameObject background = Instantiate(blockPrefabs[0],
                    new Vector3(x + xOffset, y + yOffset, 0), Quaternion.identity, tilemap.transform);
                grid[x, y] = background;
            }
        }
    }

    void RandomlyGenerateBlocks()
    {
        int blocksToGenerate = boardSize * boardSize / 2; // ��ʼ����һ��ķ���

        for (int i = 0; i < blocksToGenerate; i++)
        {
            int x = random.Next(boardSize);
            int y = random.Next(boardSize);

            // ȷ������������������ɵ�λ���������µķ���
            while (x < 0 || x >= boardSize || y < 0 || y >= boardSize || grid[x, y] == null)
            {
                x = random.Next(boardSize);
                y = random.Next(boardSize);
            }

            GameObject block = Instantiate(blockPrefabs[random.Next(1, blockPrefabs.Length)],
                new Vector3(x + grid[x, y].transform.position.x, y + grid[x, y].transform.position.y, 0),
                Quaternion.identity, tilemap.transform);
            grid[x, y] = block;

            // ��鲢����ƥ��ķ���
            CheckMatchesAndRemove(x, y);
        }
    }

    void CheckMatchesAndRemove(int x, int y)
    {
        // ���ˮƽ�����ƥ��
        if (CheckAndMarkMatches(x, y, 0, 1))
        {
            RemoveMatchedBlocks();
            FillVacantSlots();
            return;
        }

        // ��鴹ֱ�����ƥ��
        if (CheckAndMarkMatches(x, y, 1, 0))
        {
            RemoveMatchedBlocks();
            FillVacantSlots();
        }
    }

    bool CheckAndMarkMatches(int startX, int startY, int dx, int dy)
    {
        int count = 1; // ��ǰ����

        // ���һ������
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

        // ������Ҫ3���������ƥ��
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

        // ����ƥ��״̬
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
        // �ӵײ���ʼ��������ķ�����������λ
        for (int y = boardSize - 1; y > 0; y--)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (grid[x, y] == null)
                {
                    // �ҵ���λ�������濪ʼѰ�ҵ�һ���ǿշ���
                    for (int fillY = y - 1; fillY >= 0; fillY--)
                    {
                        if (grid[x, fillY] != null)
                        {
                            // �ƶ����鵽��λ
                            grid[x, y] = grid[x, fillY];
                            grid[x, fillY] = null;
                            Vector3 targetPosition = grid[x, y].transform.position;
                            targetPosition.y += 1; // �����ƶ�һ��
                            grid[x, y].transform.position = targetPosition;
                            break;
                        }
                    }
                }
            }
        }

        // �ڶ��������·���
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