using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GridObstacleData))]
public class GridTool : Editor
{
    private GridObstacleData gridObstacleData;
    private bool[] obstacleGrid;
    private void OnEnable()
    {
        // Initialize references to the target GridObstacleData and its obstacle grid
        gridObstacleData = (GridObstacleData)target;
        obstacleGrid = gridObstacleData.obstacleGrid;
    }

    public override void OnInspectorGUI()
    {
        // Display a label for the obstacle grid
        EditorGUILayout.LabelField("Obstacle Grid", EditorStyles.boldLabel);

        // Track changes in the editor GUI
        EditorGUI.BeginChangeCheck();

        int gridSize = 10; // Define the size of the grid
        for (int i = 0; i < gridSize; i++)
        {
            EditorGUILayout.BeginHorizontal(); // Begin a horizontal layout for grid row
            for (int j = 0; j < gridSize; j++)
            {
                int index = i * gridSize + j; // Calculate index in the obstacle array
                // Create a toggle button for each grid cell to represent obstacles
                obstacleGrid[index] = EditorGUILayout.Toggle(obstacleGrid[index], GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal(); // End the horizontal layout for the grid row
        }

        // If any changes are detected, mark the object as dirty to save changes
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(gridObstacleData);
        }

        // Add a button to clear all obstacles from the grid
        if (GUILayout.Button("Clear Obstacles"))
        {
            // Iterate through the obstacle grid and set all cells to false
            for (int i = 0; i < obstacleGrid.Length; i++)
            {
                obstacleGrid[i] = false;
            }
            // Mark the object as dirty to ensure changes are saved
            EditorUtility.SetDirty(gridObstacleData);
        }

        EditorGUILayout.Space(); // Add space in the editor UI

        // Draw the default inspector for other serialized fields
        DrawDefaultInspector();
    }
}
