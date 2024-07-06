using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Speed at which the player moves
    public GridObstacleData obstacleData; // Reference to the obstacle data
    public Transform enemy; // Reference to the enemy transform
    private List<Vector3> path; // The path to follow
    private bool isMoving; // Is the player currently moving

    // Specify the starting row and column for the player
    public int startRow = 0;
    public int startColumn = 0;

    void Start()
    {
        // Initialize player position based on startRow and startColumn
        InitializePlayerPosition(startRow, startColumn);
    }

    void Update()
    {
        HandleInput(); // Check for user input to move the player
        if (isMoving)
        {
            MoveAlongPath(); // Move the player along the calculated path
        }
    }

    private void InitializePlayerPosition(int row, int column)
    {
        // Ensure row and column are within grid bounds
        if (row < 0 || row >= GridPathfinder.gridSize || column < 0 || column >= GridPathfinder.gridSize)
        {
            Debug.LogError("Specified row or column is out of grid bounds!");
            return; // Exit the function if out of bounds
        }

        // Check if the initial position is obstructed by an obstacle or enemy
        int index = row * GridPathfinder.gridSize + column;
        if (obstacleData.obstacleGrid[index] || IsObstructedByEnemy(row, column))
        {
            Debug.LogError("Cannot spawn player on an obstacle or enemy!");
            return; // Exit the function if the position is obstructed
        }

        // Calculate initial position based on row and column
        Vector3 initialPosition = new Vector3(row, 0.5f, column); // Adjusted Y to 0.5 for visibility
        transform.position = initialPosition; // Set the player's position to the calculated initial position

        // Debug log to confirm player position
        Debug.Log($"Player spawned at row {row}, column {column}");
    }

    private void HandleInput()
    {
        // Check if the left mouse button is clicked and the player is not currently moving
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            // Create a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Get the world position where the ray hit
                Vector3 targetPosition = hit.point;

                // Find a path to the target position, avoiding obstacles and the enemy
                path = GridPathfinder.FindPath(transform.position, targetPosition, obstacleData, enemy.position);

                // If a valid path is found, start moving
                if (path.Count > 0)
                {
                    isMoving = true;
                }
                else
                {
                    Debug.Log("Player is stuck! No more moves");
                }
            }
        }
    }

    private void MoveAlongPath()
    {
        if (path.Count > 0)
        {
            // Get the next position in the path
            Vector3 targetPosition = path[0];

            // Calculate the direction to move in
            Vector3 direction = targetPosition - transform.position;

            // Calculate the distance to move this frame
            float distance = moveSpeed * Time.deltaTime;

            // If close enough to the target, snap to it and remove it from the path
            if (direction.magnitude <= distance)
            {
                transform.position = targetPosition;
                path.RemoveAt(0); // Remove the target position from the path
            }
            else
            {
                // Move towards the target position
                transform.Translate(direction.normalized * distance, Space.World);
            }

            // If there are no more positions in the path, stop moving
            if (path.Count == 0)
            {
                isMoving = false;
            }
        }
    }

    // Check if the specified row and column are obstructed by the enemy
    private bool IsObstructedByEnemy(int row, int column)
    {
        Vector3 enemyGridPosition = WorldToGridPosition(enemy.position);
        return enemyGridPosition.x == row && enemyGridPosition.z == column;
    }

    // Convert world position to grid position (integer coordinates)
    private Vector3Int WorldToGridPosition(Vector3 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x), 0, Mathf.RoundToInt(position.z));
    }

    // Check if grid position is obstructed by an obstacle
    private bool IsObstacle(Vector3Int gridPosition, GridObstacleData obstacleData)
    {
        int index = gridPosition.x * GridPathfinder.gridSize + gridPosition.z;
        return obstacleData.obstacleGrid[index];
    }
}
