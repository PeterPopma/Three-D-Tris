using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    Vector3Int position;

    bool isMoving;
    List<Cell> cells = new List<Cell>();

    public Block(BlockType blockType, Vector3Int position)
    {
        this.position = position;
        this.isMoving = true;
        switch (blockType)
        {
            case BlockType.BLOKTYPE_L:
                CreateBlockL();
                break;
        }
    }

    private void CreateBlockL()
    {
        cells.Add(new Cell(this, new Vector3Int(1, 1, 0)));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 1)));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 2)));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 3)));
        cells.Add(new Cell(this, new Vector3Int(1, 2, 3)));
    }

    public bool IsMoving { get => isMoving; set => isMoving = value; }


    public List<Cell> Cells { get => cells; set => cells = value; }
    public Vector3Int Position { get => position; set => position = value; }
}
