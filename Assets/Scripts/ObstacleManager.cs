using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GridObstacleData obstacleData;
    public GameObject obstaclePrefab;

    void Start()
    {
        GenerateObstacles(); // Generate obstacles on the grid
    }

    void GenerateObstacles()
    {
        int gridSize = 10; // Define the size of the grid (10x10)

        // Iterate through each cell in the grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                int index = x * gridSize + y; // Calculate the index in the obstacle array

                // Check if the current cell is marked as an obstacle
                if (obstacleData.obstacleGrid[index])
                {
                    // Set the position for the obstacle prefab to be instantiated
                    Vector3 position = new Vector3(x, 0.67f, y); // Adjust the y position to appear above the tile

                    // Instantiate the obstacle prefab at the specified position with no rotation
                    Instantiate(obstaclePrefab, position, Quaternion.identity);
                }
            }
        }
    }
}