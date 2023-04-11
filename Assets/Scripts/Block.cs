using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    Vector3Int position;
    Material materialCell;

    bool isMoving;
    List<Cell> cells = new List<Cell>();

    public Block(BlockType blockType, Vector3Int position, Material materialCell)
    {
        this.position = position;
        this.materialCell = materialCell;
        switch (blockType)
        {
            case BlockType.BLOKTYPE_L:
                CreateBlockL();
                break;
        }
        isMoving = true;
    }

    private void CreateBlockL()
    {
        cells.Add(new Cell(this, new Vector3Int(1, 1, 0), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 3), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 2, 3), materialCell));
    }

    public bool IsMoving { get => isMoving; set => isMoving = value; }


    public List<Cell> Cells { get => cells; set => cells = value; }
    public Vector3Int Position { get => position; set { position = value; foreach(Cell cell in cells) { cell.SetGameObjectPosition(); } } }
    public Material MaterialCell { get => materialCell; set => materialCell = value; }
}
