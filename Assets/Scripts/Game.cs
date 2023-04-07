using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BlockType
{
    BLOKTYPE_L,
    BLOKTYPE_O,
    BLOKTYPE_Z,
    BLOKTYPE_T
}

public class Game : MonoBehaviour
{
    public const float GRID_OFFSET_X = -0.5f;
    public const float GRID_OFFSET_Y = -0.5f;
    public const float GRID_OFFSET_Z = -0.5f;
    const int PLAYFIELD_WIDTH = 14;
    const int PLAYFIELD_DEPTH = 14;
    const int PLAYFIELD_HEIGHT = 30;
    float timeSinceBlockSpawn = 4;
    List<Block> blocks = new List<Block>();
    Block currentBlock;
    float rotateLeftRight;
    float rotateForwardBack;
    bool[,,] playField = new bool[PLAYFIELD_WIDTH, PLAYFIELD_HEIGHT, PLAYFIELD_DEPTH];

    // Start is called before the first frame update
    void Start()
    {
        // mark boundaries of playfield
        for(int y = 0; y < PLAYFIELD_HEIGHT; y++)
        {
            for (int x = 0; x < PLAYFIELD_WIDTH; x++)
            {
                playField[x, y, 0] = true;
                playField[x, y, PLAYFIELD_DEPTH - 1] = true;
            }
            for (int z = 0; z < PLAYFIELD_DEPTH; z++)
            {
                playField[0, y, z] = true;
                playField[PLAYFIELD_WIDTH-1, y, z] = true;
            }
        }
        DisplayPlayfield();

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceBlockSpawn += Time.deltaTime;
        if (timeSinceBlockSpawn > 5)
        {
            timeSinceBlockSpawn = -10000;
            currentBlock = new Block(BlockType.BLOKTYPE_L, new Vector3Int(PLAYFIELD_WIDTH / 2, PLAYFIELD_HEIGHT, PLAYFIELD_DEPTH / 2));
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
                        Instantiate(Resources.Load("pfCube"), new Vector3(x + GRID_OFFSET_X, y + GRID_OFFSET_Y, z + GRID_OFFSET_Z), Quaternion.identity);
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

    private void OnLeft(InputValue value)
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

    private void OnRight(InputValue value)
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

    private void OnForward(InputValue value)
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

    private void OnBack(InputValue value)
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

    private void OnRotateYAxisCW(InputValue value)
    {
        if (currentBlock != null)
        {
            RotateYAxisCW(currentBlock);
        }
    }

    private void OnRotateYAxisCCW(InputValue value)
    {
        if (currentBlock != null)
        {
            RotateYAxisCCW(currentBlock);
        }
    }

    private void OnRotateXAxisCW(InputValue value)
    {
        if (currentBlock != null)
        {
            RotateXAxisCW(currentBlock);
        }
    }

    private void OnRotateXAxisCCW(InputValue value)
    {
        if (currentBlock != null)
        {
            RotateXAxisCCW(currentBlock);
        }
    }
}
