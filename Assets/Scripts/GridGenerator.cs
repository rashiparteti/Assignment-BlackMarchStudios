using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject cubePrefab; // Prefab to use for each grid tile
    public int rows = 10; // Number of rows in the grid
    public int columns = 10; // Number of columns in the grid
    public float spacing = 1.1f; // Space between each grid tile

    void Start()
    {
        GenerateGrid(); // function call to generate the grid
    }

    void GenerateGrid()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                // Calculate the position of each cube in the grid
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);

                // Instantiate cube at the calculated position 
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);

                // Adding TileInfo component for tile information
                cube.AddComponent<TileInfo>().SetTileInfo(x, y);
            }
        }
    }
}
