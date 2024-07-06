using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Camera mainCamera;
    public Text tileInfoText;

    void Update()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits any collider 
        if (Physics.Raycast(ray, out hit))
        {
            // Try to get the TileInfo component from the hit object
            TileInfo tileInfo = hit.collider.GetComponent<TileInfo>();

            // If the hit object has a TileInfo component, update the UI text with tile position
            if (tileInfo != null)
            {
                // Display the row and column of the tile
                tileInfoText.text = $"Tile Position: Row {tileInfo.row}, Column {tileInfo.column}";
            }
        }
    }
}
