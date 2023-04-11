using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    Block parentBlock;
    Vector3Int position;
    GameObject gameObject;

    public GameObject GameObject { get => gameObject; set => gameObject = value; }
    public Vector3Int Position { get => position; set { position = value; SetGameObjectPosition(); } }
    public Vector3Int AbsolutePosition { get => parentBlock.Position + position; }

    public void SetGameObjectPosition()
    {
        gameObject.transform.position = new Vector3(
            parentBlock.Position.x + position.x + Game.GRID_OFFSET_X,
            parentBlock.Position.y + position.y + Game.GRID_OFFSET_Y,
            parentBlock.Position.z + position.z + Game.GRID_OFFSET_Z);
    }

    public Cell(Block parentBlock, Vector3Int position, Material materialCell)
    {
        this.parentBlock = parentBlock;
        gameObject = Object.Instantiate(Resources.Load("pfCell", typeof(GameObject))) as GameObject;
        gameObject.GetComponent<Renderer>().material = materialCell;
        Position = position;
    }
}
