using UnityEngine;
using UnityEngine.UI; // ����UI�����ռ�

public class PlayerInput : MonoBehaviour
{
    public GameObject[] blockPrefabs; // ���ַ���Ԥ��������
    public GameObject tilemap; // Tilemap GameObject
    public GameObject scoreText; // ������ʾ��UI Text���
    private int score = 0; // ��ҷ���

    private GameObject selectedBlock1, selectedBlock2; // �洢ѡ�е���������

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
        // ��������
        if (Input.GetMouseButtonDown(0)) // ������
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
                        // ��һ�ε����ѡ���һ������
                        selectedBlock1 = clickedObject;
                    }
                    else
                    {
                        // �ڶ��ε����ѡ��ڶ������鲢���Խ���
                        selectedBlock2 = clickedObject;

                        // ������������Ƿ�����
                        if (AreAdjacent(selectedBlock1, selectedBlock2))
                        {
                            // ��������
                            SwapBlocks(selectedBlock1, selectedBlock2);

                            // ����������һ����������齻���Ƿ���Ч�������������ķ�������
                            int matchesCount = CheckSwapValidity();

                            if (matchesCount > 0)
                            {
                                // ���������Ч���������
                                score += matchesCount - 2; // ���������ķ���������÷���
                                UpdateScoreUI();
                            }
                            else
                            {
                                // ���������Ч����������
                                SwapBlocks(selectedBlock1, selectedBlock2); // �ٴν����Գ�������
                            }
                        }

                        // ����ѡ��״̬
                        selectedBlock1 = null;
                        selectedBlock2 = null;
                    }
                }
            }
        }
    }

    // �洢�����Ϸ��������
    private GameObject[,] grid;
    //......

    // ��鷽�齻���Ƿ���Ч�ķ���
    private int CheckSwapValidity()
    {
        if (selectedBlock1 == null || selectedBlock2 == null)
            return 0;

        // ��ȡ���������
        int x1 = (int)selectedBlock1.transform.position.x;
        int y1 = (int)selectedBlock1.transform.position.y;
        int x2 = (int)selectedBlock2.transform.position.x;
        int y2 = (int)selectedBlock2.transform.position.y;

        // ���潻��ǰ�ķ�������
        GameObject blockRef1 = grid[x1, y1];
        GameObject blockRef2 = grid[x2, y2];

        // ģ�⽻��
        grid[x1, y1] = blockRef2;
        grid[x2, y2] = blockRef1;

        // ��齻�����Ƿ��γ�ƥ��
        int matchesCount = 0;

        // ���ˮƽ�����ƥ��
        matchesCount += CheckMatchesInDirection(x1, y1, 0, 1);
        if (matchesCount >= 3)
        {
            MarkMatches(x1, y1, 0, 1);
        }

        // ��鴹ֱ�����ƥ��
        matchesCount += CheckMatchesInDirection(x2, y2, 1, 0);
        if (matchesCount >= 3)
        {
            MarkMatches(x2, y2, 1, 0);
        }

        // �ָ�����ǰ�ķ�������
        grid[x1, y1] = blockRef1;
        grid[x2, y2] = blockRef2;

        // ���û���γ��µ�ƥ�䣬�򷵻�0
        return matchesCount;
    }

    private int CheckMatchesInDirection(int startX, int startY, int dx, int dy)
    {
        int count = 0;
        int x = startX;
        int y = startY;

        // �ҵ���һ��ƥ��ķ���
        while (x >= 0 && x < boardSize && y >= 0 && y < boardSize && AreSameBlocks(grid[x, y], grid[startX, startY]))
        {
            count++;
            x += dx;
            y += dy;
        }

        // ����γ�ƥ�䣬����ƥ������������򷵻�0
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

    // �������������λ��
    private void SwapBlocks(GameObject block1, GameObject block2)
    {
        Vector3 tempPosition = block1.transform.position;
        block1.transform.position = block2.transform.position;
        block2.transform.position = tempPosition;
    }

    // ������������Ƿ�����
    private bool AreAdjacent(GameObject block1, GameObject block2)
    {
        int deltaX = Mathf.Abs((int)block1.transform.position.x - (int)block2.transform.position.x);
        int deltaY = Mathf.Abs((int)block1.transform.position.y - (int)block2.transform.position.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    // ���·�����ʾ�ķ���
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.GetComponent<Text>().text = "Score: " + score;
        }
    }
}