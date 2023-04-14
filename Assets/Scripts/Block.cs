using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    Vector3Int position;
    Material materialCell;
    BlockType blockType;

    List<Cell> cells = new();

    public Block(BlockType blockType, Vector3Int position, Material materialCell)
    {
        this.blockType = blockType;
        this.position = position;
        this.materialCell = materialCell;
        switch (blockType)
        {
            case BlockType.BLOKTYPE_L:
                CreateBlockL();
                break;
            case BlockType.BLOKTYPE_O:
                CreateBlockO();
                break;
            case BlockType.BLOKTYPE_Z:
                CreateBlockZ();
                break;
            case BlockType.BLOKTYPE_T:
                CreateBlockT();
                break;
            case BlockType.BLOKTYPE_I:
                CreateBlockI();
                break;
        }
    }

    private void CreateBlockL()
    {
        cells.Add(new Cell(this, new Vector3Int(1, 1, 0), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 3), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 2, 3), materialCell));
    }

    private void CreateBlockO()
    {
        cells.Add(new Cell(this, new Vector3Int(1, 1, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 2, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 2, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 1, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 2, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 1, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 2, 2), materialCell));
    }

    private void CreateBlockZ()
    {
        cells.Add(new Cell(this, new Vector3Int(1, 1, 0), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 2, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 2, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 1, 0), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 1, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 2, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 2, 2), materialCell));
    }

    private void CreateBlockT()
    {
        cells.Add(new Cell(this, new Vector3Int(1, 1, 0), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(2, 1, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(0, 1, 2), materialCell));
    }

    private void CreateBlockI()
    {
        cells.Add(new Cell(this, new Vector3Int(1, 1, 0), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 1), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 2), materialCell));
        cells.Add(new Cell(this, new Vector3Int(1, 1, 3), materialCell));
    }

    public List<Cell> Cells { get => cells; set => cells = value; }
    public Vector3Int Position { get => position; set { position = value; foreach(Cell cell in cells) { cell.SetGameObjectPosition(); } } }
    public Material MaterialCell { get => materialCell; set => materialCell = value; }
    public BlockType BlockType { get => blockType; set => blockType = value; }
}
