using UnityEngine;

public class ToggleBillboard : MonoBehaviour
{
    private bool spritesEnabled = true;

    // This function toggles the visibility of all SpriteRenderers in the scene
    public void ToggleSprites()
    {
        // Find all objects with a SpriteRenderer in the scene
        SpriteRenderer[] spriteRenderers = FindObjectsOfType<SpriteRenderer>();

        // Toggle the enabled state of each SpriteRenderer
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }

        // Update the state
        spritesEnabled = !spritesEnabled;
    }
}
