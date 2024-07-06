using UnityEngine;

public class TileInfo : MonoBehaviour
{
    // Public fields to store the row and column information of this tile
    public int row;
    public int column;

    // Method to set the row and column information of the tile
    public void SetTileInfo(int r, int c)
    {
        // Assign the provided row and column values to the respective fields
        row = r;
        column = c;
    }
}
