using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BlockType
{
    BLOKTYPE_L,
    BLOKTYPE_O,
    BLOKTYPE_Z,
    BLOKTYPE_T,
    BLOKTYPE_I
}

public enum RotationType
{
    XAxisCW,
    XAxisCCW,
    YAxisCW,
    YAxisCCW
}

public class Game : MonoBehaviour
{
    const int BLOCK_SIZE = 4;
    const int PLAYFIELD_SIZE_X = 12;
    const int PLAYFIELD_SIZE_Y = 26;
    const int PLAYFIELD_SIZE_Z = 12;
    const int PLAYFIELD_HEIGHT_BLOCKS_ALLOWED = 15;
    float timeSinceBlockDrop;
    [SerializeField] List<Material> cellMaterials = new();
    [SerializeField] TextMeshProUGUI[] levelTexts;
    [SerializeField] TextMeshProUGUI[] scoreTexts;
    [SerializeField] GameObject panelGameOver;
    Block currentBlock;
    private int score, rowsCompleted, level;
    readonly GameObject[,,] playField = new GameObject[PLAYFIELD_SIZE_X, PLAYFIELD_SIZE_Y, PLAYFIELD_SIZE_Z];
    bool playingGame;

    void Start()
    {
        ResetGame();
        CreateLayerWithHole(0, new Vector2Int(3,3));
        CreateLayerWithHole(1, new Vector2Int(5,6));
        CreateLayerWithHole(2, new Vector2Int(7,8));
    }

    void CreateLayerWithHole(int y, Vector2Int holePosition)
    {
        for (int x = 0; x < PLAYFIELD_SIZE_X; x++)
        {
            for (int z = 0; z < PLAYFIELD_SIZE_Z; z++)
            {
                if (x!=holePosition.x || z != holePosition.y)
                {
                    GameObject gameObject = Instantiate(Resources.Load("pfCell", typeof(GameObject))) as GameObject;
                    gameObject.transform.position = new Vector3(x, y, z);
                    playField[x, y, z] = gameObject;
                }
            }
        }
    }

    public void OnContinue()
    {
        if (!playingGame)
        {
            ResetGame();
        }
    }
    private void ResetGame()
    {
        SetLevel(1);
        SetSore(0);
        ResetLevel();
        NewRandomBlock();
        playingGame = true;
    }

    private void NewRandomBlock()
    {
        int materialIndex = UnityEngine.Random.Range(0, cellMaterials.Count);
        Array values = Enum.GetValues(typeof(BlockType));
        BlockType blockType = (BlockType)values.GetValue(new System.Random().Next(values.Length));
        currentBlock = new Block(blockType, new Vector3Int(PLAYFIELD_SIZE_X / 2, PLAYFIELD_SIZE_Y - BLOCK_SIZE, PLAYFIELD_SIZE_Z / 2), cellMaterials[materialIndex]);
    }

    private void ResetLevel()
    {
        rowsCompleted = 0;
        panelGameOver.SetActive(false);
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");
        foreach (GameObject cell in cells)
        {
            Destroy(cell);
        }

        // clear playfield
        for (int y = 0; y < PLAYFIELD_SIZE_Y; y++)
        {
            for (int x = 0; x < PLAYFIELD_SIZE_X; x++)
            {
                for (int z = 0; z < PLAYFIELD_SIZE_Z; z++)
                {
                    if(playField[x, y, z] != null)
                    {
                        Destroy(playField[x, y, z]);
                        playField[x, y, z] = null;
                    }
                }
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
        foreach (TextMeshProUGUI levelText in levelTexts)
        {
            levelText.text = level.ToString();
        }
    }

    void Update()
    {
        if (!playingGame)
        {
            return;
        }

        timeSinceBlockDrop += Time.deltaTime;
        if (timeSinceBlockDrop > (1.1 - (level * 0.2)))
        {
            timeSinceBlockDrop = 0;
            LowerBlockByOne();
        }
    }

    private void GameOverCheck()
    {
        foreach (Cell cell in currentBlock.Cells)
        {
            if (cell.AbsolutePosition.y >= PLAYFIELD_HEIGHT_BLOCKS_ALLOWED)
            {
                panelGameOver.SetActive(true);
                playingGame = false;
                return;
            }
        }
    }

    private void RowCompletedCheck()
    {
        for (int y = 0; y < PLAYFIELD_HEIGHT_BLOCKS_ALLOWED; y++)
        {
            bool layerContainsEmptyCells = false;
            for (int x = 0; x < PLAYFIELD_SIZE_X; x++)
            {
                for (int z = 0; z < PLAYFIELD_SIZE_Z; z++)
                {
                    if (playField[x, y, z] == null)
                    {
                        layerContainsEmptyCells = true;
                        break;
                    }
                }
                if (layerContainsEmptyCells)
                {
                    break;
                }
            }
            if (!layerContainsEmptyCells)
            {
                RemoveLayer(y);
            }
        }
    }

    private void LowerAllLayersHigher(int yPosition)
    {
        for (int y = yPosition; y < PLAYFIELD_SIZE_Y - 2; y++)
        {
            for (int x = 0; x < PLAYFIELD_SIZE_X; x++)
            {
                for (int z = 0; z < PLAYFIELD_SIZE_Z; z++)
                {
                    playField[x, y, z] = playField[x, y + 1, z];
                    if (playField[x, y, z] != null)
                    {
                        playField[x, y, z].transform.position += new Vector3(0, -1, 0);
                    }
                }
            }
        }
    }

    private void RemoveLayer(int yPosition)
    {
        for (int x = 0; x < PLAYFIELD_SIZE_X; x++)
        {
            for (int z = 0; z < PLAYFIELD_SIZE_Z; z++)
            {
                Destroy(playField[x, yPosition, z]);
            }
        }
        LowerAllLayersHigher(yPosition);
        SetSore(score + level * (PLAYFIELD_SIZE_X - 2) * (PLAYFIELD_SIZE_Z - 2));
        rowsCompleted++;
        if (rowsCompleted > 10)
        {
            level++;
            ResetLevel();
        }
    }

    private bool LowerBlockByOne()
    {
        if (!CollisionCheck(CurrentBlockToPositionsList(new Vector3Int(0, -1, 0))))
        {
            currentBlock.Position += new Vector3Int(0, -1, 0);
            return true;
        }
        else
        {
            GameOverCheck();
            AddCellsToPlayfield();
            RowCompletedCheck();
            NewRandomBlock();
            return false;
        }
    }

    private void AddCellsToPlayfield()
    {
        foreach(Cell cell in currentBlock.Cells)
        {
            if(cell.AbsolutePosition.x >= 0 && cell.AbsolutePosition.x < PLAYFIELD_SIZE_X &&
                cell.AbsolutePosition.y >= 0 && cell.AbsolutePosition.y < PLAYFIELD_SIZE_Y &&
                cell.AbsolutePosition.z >= 0 && cell.AbsolutePosition.z < PLAYFIELD_SIZE_Z)
            playField[cell.AbsolutePosition.x, cell.AbsolutePosition.y, cell.AbsolutePosition.z] = cell.GameObject;
        }
    }

    private bool CollisionCheck(List<Vector3Int> cells)
    {
        foreach (Vector3Int cell in cells)
        {
            if (cell.x < 0 || cell.x >= PLAYFIELD_SIZE_X ||
                cell.y < 0 || cell.y >= PLAYFIELD_SIZE_Y ||
                cell.z < 0 || cell.z >= PLAYFIELD_SIZE_Z ||
                playField[cell.x, cell.y, cell.z] != null)
            {
                return true;
            }
        }

        return false;
    }

    public void OnLeft()
    {
        if (currentBlock != null)
        {
            if (!CollisionCheck(CurrentBlockToPositionsList(new Vector3Int(1, 0, 0))))
            {
                currentBlock.Position += new Vector3Int(1, 0, 0);
            }
        }
    }

    public void OnRight()
    {
        if (currentBlock != null)
        {
            if (!CollisionCheck(CurrentBlockToPositionsList(new Vector3Int(-1, 0, 0))))
            {
                currentBlock.Position += new Vector3Int(-1, 0, 0);
            }
        }
    }

    public void OnForward()
    {
        if (currentBlock != null)
        {
            if (!CollisionCheck(CurrentBlockToPositionsList(new Vector3Int(0, 0, 1))))
            { 
                currentBlock.Position += new Vector3Int(0, 0, 1);
            }
        }
    }

    public void OnBack()
    {
        if (currentBlock != null)
        {
            if (!CollisionCheck(CurrentBlockToPositionsList(new Vector3Int(0, 0, -1))))
            {
                currentBlock.Position += new Vector3Int(0, 0, -1);
            }
        }
    }

    public void OnDrop()
    {
        if (!playingGame)
        {
            ResetGame();
        }
        while (LowerBlockByOne());
    }

    private List<Vector3Int> CurrentBlockToPositionsList(Vector3Int offset)
    {
        List<Vector3Int> positionsList = new();
        foreach (Cell cell in currentBlock.Cells)
        {
            positionsList.Add(cell.AbsolutePosition + offset);
        }

        return positionsList;
    }

    private List<Vector3Int> CurrentBlockToRotatedPositionsList(RotationType rotationType)
    {
        List<Vector3Int> positionsList = new();
        foreach (Cell cell in currentBlock.Cells)
        {
            positionsList.Add(Rotate(cell.Position, rotationType) + currentBlock.Position);
        }

        return positionsList;
    }

    private Vector3Int Rotate(Vector3Int rotationObject, RotationType rotationType)
    {
        return rotationType switch
        {
            RotationType.XAxisCW => new Vector3Int(rotationObject.z, rotationObject.y, BLOCK_SIZE - rotationObject.x),
            RotationType.XAxisCCW => new Vector3Int(rotationObject.x, BLOCK_SIZE - rotationObject.z, rotationObject.y),
            RotationType.YAxisCW => new Vector3Int(rotationObject.z, rotationObject.y, BLOCK_SIZE - rotationObject.x),
            RotationType.YAxisCCW => new Vector3Int(BLOCK_SIZE - rotationObject.z, rotationObject.y, rotationObject.x),
            _ => throw new NotImplementedException()
        };
    }

    private void RotateIfPossible(RotationType rotationType)
    {
        if (currentBlock != null)
        {
            if (!CollisionCheck(CurrentBlockToRotatedPositionsList(rotationType)))
            {
                foreach (Cell cell in currentBlock.Cells)
                {
                    cell.Position = Rotate(cell.Position, rotationType);
                }
            }
        }
    }

    public void OnRotateXAxisCW()
    {
        RotateIfPossible(RotationType.XAxisCW);
    }

    public void OnRotateXAxisCCW()
    {
        RotateIfPossible(RotationType.XAxisCCW);
    }

    public void OnRotateYAxisCW()
    {
        RotateIfPossible(RotationType.YAxisCW);
    }

    public void OnRotateYAxisCCW()
    {
        RotateIfPossible(RotationType.YAxisCCW);
    }

}
