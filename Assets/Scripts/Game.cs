using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BlockType
{
    BLOKTYPE_L,
    BLOKTYPE_O,
    BLOKTYPE_Z,
    BLOKTYPE_T
}

public class Game : MonoBehaviour
{
    public const float GRID_OFFSET_X = 0f;
    public const float GRID_OFFSET_Y = -0.5f;
    public const float GRID_OFFSET_Z = 0f;
    const int PLAYFIELD_WIDTH = 14;
    const int PLAYFIELD_DEPTH = 14;
    const int PLAYFIELD_HEIGHT = 30;
    float timeSinceBlockSpawn = 4;
    float timeSinceBlockDrop;
    List<Block> blocks = new List<Block>();
    [SerializeField] List<Material> cellMaterials = new List<Material>();
    [SerializeField] TextMeshProUGUI[] LevelTexts;
    [SerializeField] TextMeshProUGUI[] scoreTexts;
    [SerializeField] GameObject panelGameOver;
    Block currentBlock;
    float rotateLeftRight;
    float rotateForwardBack;
    private int score, level;
    bool[,,] playField = new bool[PLAYFIELD_WIDTH, PLAYFIELD_HEIGHT, PLAYFIELD_DEPTH];
    bool playingGame;

    // Start is called before the first frame update
    void Start()
    {
        //        DisplayPlayfield();
        ResetGame();
    }

    public void OnContinue()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        playingGame = true;
        panelGameOver.SetActive(false);
        SetSore(0);
        SetLevel(1);

        // clear playfield
        for (int y = 0; y < PLAYFIELD_HEIGHT; y++)
        {
            for (int x = 0; x < PLAYFIELD_WIDTH; x++)
            {
                for (int z = 0; z < PLAYFIELD_DEPTH; z++)
                {
                    playField[x, 0, z] = false;
                }
            }
        }

        // mark boundaries of playfield
        for (int y = 0; y < PLAYFIELD_HEIGHT; y++)
        {
            for (int x = 0; x < PLAYFIELD_WIDTH; x++)
            {
                playField[x, y, 0] = true;
                playField[x, y, PLAYFIELD_DEPTH - 1] = true;
            }
            for (int z = 0; z < PLAYFIELD_DEPTH; z++)
            {
                playField[0, y, z] = true;
                playField[PLAYFIELD_WIDTH - 1, y, z] = true;
            }
        }
        for (int x = 0; x < PLAYFIELD_WIDTH; x++)
        {
            for (int z = 0; z < PLAYFIELD_DEPTH; z++)
            {
                playField[x, 0, z] = true;
            }
        }
    }

    private void SetSore(int value)
    {
        score = value;
        foreach (TextMeshProUGUI scoreText in scoreTexts)
        {
            scoreText.text = score.ToString("00000");
        }
    }

    private void SetLevel(int value)
    {
        level = value;
        foreach (TextMeshProUGUI levelText in LevelTexts)
        {
            levelText.text = level.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playingGame)
        {
            return;
        }

        timeSinceBlockSpawn += Time.deltaTime;
        if (timeSinceBlockSpawn > 5)
        {
            timeSinceBlockSpawn = -10000;
            int materialIndex = Random.Range(0, cellMaterials.Count);
            currentBlock = new Block(BlockType.BLOKTYPE_L, new Vector3Int(PLAYFIELD_WIDTH / 2, 22, PLAYFIELD_DEPTH / 2), cellMaterials[materialIndex]);
        }

        timeSinceBlockDrop += Time.deltaTime;
        if (timeSinceBlockDrop > 1)
        {
            timeSinceBlockDrop = 0;
            LowerBlockByOne();
        }
    }

    private void GameOverCheck()
    {
        foreach(Cell cell in currentBlock.Cells)
        {
            if (cell.AbsolutePosition.y >= 15)
            {
                panelGameOver.SetActive(true);
                playingGame = false;
                return;
            }
        }
    }

    private bool LowerBlockByOne()
    {
        SetSore(score + 1);
        
        Vector3Int newPosition = new Vector3Int(currentBlock.Position.x, currentBlock.Position.y - 1, currentBlock.Position.z);
        if (!CollisionCheck(currentBlock, newPosition))
        {
            currentBlock.Position = newPosition;
            return true;
        }
        else
        {
            GameOverCheck();
            AddCellPositionsToPlayfield();
            int materialIndex = Random.Range(0, cellMaterials.Count);
            currentBlock = new Block(BlockType.BLOKTYPE_L, new Vector3Int(PLAYFIELD_WIDTH / 2, 22, PLAYFIELD_DEPTH / 2), cellMaterials[materialIndex]);
            return false;
        }
    }

    private void AddCellPositionsToPlayfield()
    {
        foreach(Cell cell in currentBlock.Cells)
        {
            playField[cell.AbsolutePosition.x, cell.AbsolutePosition.y, cell.AbsolutePosition.z] = true;
        }
    }

    private void DisplayPlayfield()
    {
        for (int x = 0; x < PLAYFIELD_WIDTH; x++)
        {
            for (int y = 0; y < PLAYFIELD_HEIGHT; y++)
            {
                for (int z = 0; z < PLAYFIELD_DEPTH; z++)
                {
                    if (playField[x, y, z]==true)
                    {
                        Instantiate(Resources.Load("pfCube"), new Vector3(x, y, z), Quaternion.identity);
                    }
                }
            }
        }
    }

    private bool CollisionCheck(Block block, Vector3Int position)
    {
        foreach (Cell cell in block.Cells)
        {
            if (playField[position.x + cell.Position.x, position.y + cell.Position.y, position.z + cell.Position.z])
            {
                return true;
            }
        }

        return false;
    }

    private void RotateYAxisCW(Block block)
    {
        foreach (Cell cell in block.Cells)
        {
            cell.Position = new Vector3Int(cell.Position.z, cell.Position.y, 4 - cell.Position.x);
        }
    }

    private void RotateYAxisCCW(Block block)
    {
        foreach (Cell cell in block.Cells)
        {
            cell.Position = new Vector3Int(4 - cell.Position.z, cell.Position.y, cell.Position.x);
        }
    }

    private void RotateXAxisCW(Block block)
    {
        foreach (Cell cell in block.Cells)
        {
            cell.Position = new Vector3Int(cell.Position.x, cell.Position.z, 4 - cell.Position.y);
        }
    }

    private void RotateXAxisCCW(Block block)
    {
        foreach (Cell cell in block.Cells)
        {
            cell.Position = new Vector3Int(cell.Position.x, 4 - cell.Position.z, cell.Position.y);
        }
    }

    public void OnLeft()
    {
        if (currentBlock != null)
        {
            Vector3Int newPosition = new Vector3Int(currentBlock.Position.x + 1, currentBlock.Position.y, currentBlock.Position.z);
            if (!CollisionCheck(currentBlock, newPosition))
            {
                currentBlock.Position += new Vector3Int(1, 0, 0);
            }
        }
    }

    public void OnRight()
    {
        if (currentBlock != null)
        {
            Vector3Int newPosition = new Vector3Int(currentBlock.Position.x - 1, currentBlock.Position.y, currentBlock.Position.z);
            if (!CollisionCheck(currentBlock, newPosition))
            {
                currentBlock.Position += new Vector3Int(-1, 0, 0);
            }
        }
    }

    public void OnForward()
    {
        if (currentBlock != null)
        {
            Vector3Int newPosition = new Vector3Int(currentBlock.Position.x, currentBlock.Position.y, currentBlock.Position.z + 1);
            if (!CollisionCheck(currentBlock, newPosition))
            {
                currentBlock.Position += new Vector3Int(0, 0, 1);
            }
        }
    }

    public void OnBack()
    {
        if (currentBlock != null)
        {
            Vector3Int newPosition = new Vector3Int(currentBlock.Position.x, currentBlock.Position.y, currentBlock.Position.z - 1);
            if (!CollisionCheck(currentBlock, newPosition))
            {
                currentBlock.Position += new Vector3Int(0, 0, -1);
            }
        }
    }

    public void OnDrop()
    {
        while (LowerBlockByOne()) ;
    }

    public void OnRotateYAxisCW()
    {
        if (currentBlock != null)
        {
            RotateYAxisCW(currentBlock);
        }
    }

    public void OnRotateYAxisCCW()
    {
        if (currentBlock != null)
        {
            RotateYAxisCCW(currentBlock);
        }
    }

    public void OnRotateXAxisCW()
    {
        if (currentBlock != null)
        {
            RotateXAxisCW(currentBlock);
        }
    }

    public void OnRotateXAxisCCW()
    {
        if (currentBlock != null)
        {
            RotateXAxisCCW(currentBlock);
        }
    }
}
