using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, AI
{
    public float moveSpeed = 2.0f; // Speed at which the enemy moves
    public GridObstacleData obstacleData; // Reference to the obstacle data
    private List<Vector3> path; // The calculated path
    private bool isMoving; // Flag to indicate if the enemy is currently moving

    private PlayerController playerController;

    void Start()
    {
        // Find the player's controller in the scene
        playerController = FindObjectOfType<PlayerController>();

        // Find the obstacle data from the ObstacleManager in the scene
        obstacleData = FindObjectOfType<ObstacleManager>().obstacleData;
    }

    void Update()
    {
        // If not currently moving, calculate path towards player
        if (!isMoving)
        {
            MoveTowards(playerController.transform.position);
        }
        else // Otherwise, move along the calculated path
        {
            MoveAlongPath();
        }
    }

    // Calculate the path towards the target position (player's position)
    public void MoveTowards(Vector3 targetPosition)
    {
        // Convert current and target positions to grid positions
        Vector3Int enemyGridPos = WorldToGridPosition(transform.position);
        Vector3Int playerGridPos = WorldToGridPosition(targetPosition);

        // Define adjacent positions around the player's position
        Vector3Int[] adjacentPositions = new Vector3Int[]
        {
            playerGridPos + Vector3Int.left,
            playerGridPos + Vector3Int.right,
            playerGridPos + Vector3Int.forward,
            playerGridPos + Vector3Int.back
        };

        // Find the closest valid adjacent position to move towards
        Vector3Int bestTarget = playerGridPos;
        float bestDistance = float.MaxValue;

        foreach (var pos in adjacentPositions)
        {
            if (IsWithinGrid(pos) && !IsObstacle(pos) && !IsPlayerPosition(pos))
            {
                float distance = Vector3Int.Distance(enemyGridPos, pos);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTarget = pos;
                }
            }
        }

        // Calculate path using A* algorithm from current position to best adjacent position
        path = GridPathfinder.FindPath(transform.position, GridToWorldPosition(bestTarget), obstacleData, playerController.transform.position);
        isMoving = true; // Set moving flag to true
    }

    // Move the enemy along the calculated path
    private void MoveAlongPath()
    {
        if (path != null && path.Count > 0)
        {
            Vector3 targetPosition = path[0];
            Vector3 direction = targetPosition - transform.position;
            float distance = moveSpeed * Time.deltaTime;

            // Move towards the next node in the path
            if (direction.magnitude <= distance)
            {
                transform.position = targetPosition;
                path.RemoveAt(0); // Remove the current node from the path list
            }
            else
            {
                transform.Translate(direction.normalized * distance, Space.World);
            }

            // If reached the end of the path, stop moving
            if (path.Count == 0)
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false; // Stop moving if no valid path
        }
    }

    // Convert world position to grid position (integer coordinates)
    private Vector3Int WorldToGridPosition(Vector3 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x), 0, Mathf.RoundToInt(position.z));
    }

    // Convert grid position to world position (adjusted Y for visibility)
    private Vector3 GridToWorldPosition(Vector3Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0.5f, gridPosition.z);
    }

    // Check if grid position is within grid bounds
    private bool IsWithinGrid(Vector3Int gridPosition)
    {
        int gridSize = 10; // Assuming grid size is 10x10
        return gridPosition.x >= 0 && gridPosition.x < gridSize && gridPosition.z >= 0 && gridPosition.z < gridSize;
    }

    // Check if grid position is obstructed by an obstacle
    private bool IsObstacle(Vector3Int gridPosition)
    {
        int gridSize = 10; // Assuming grid size is 10x10
        int index = gridPosition.x * gridSize + gridPosition.z;
        return obstacleData.obstacleGrid[index];
    }

    // Check if grid position matches player's current grid position
    private bool IsPlayerPosition(Vector3Int gridPosition)
    {
        Vector3Int playerGridPos = WorldToGridPosition(playerController.transform.position);
        return gridPosition == playerGridPos;
    }
}
