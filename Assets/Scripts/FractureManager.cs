using UnityEngine;

public class FractureManager : MonoBehaviour
{
    /// <summary>
    /// Causes fracture on all GameObjects with the specified tag
    /// </summary>
    /// <param name="tagName">The tag name to search for</param>
    public void FractureObjectsByTag(string tagName)
    {
        // Find all GameObjects with the specified tag
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tagName);

        // Check if any objects were found
        if (objectsWithTag.Length == 0)
        {
            Debug.LogWarning($"No GameObjects found with tag: {tagName}");
            return;
        }

        Debug.Log($"Found {objectsWithTag.Length} GameObject(s) with tag: {tagName}");

        // Iterate through each object and try to fracture it
        foreach (GameObject obj in objectsWithTag)
        {
            // Try to get the Fracture component
            Fracture fractureComponent = obj.GetComponent<Fracture>();

            if (fractureComponent != null)
            {
                // Call CauseFracture on the component
                fractureComponent.CauseFracture();
                Debug.Log($"Fracturing object: {obj.name}");
            }
            else
            {
                Debug.LogWarning($"GameObject '{obj.name}' with tag '{tagName}' does not have a Fracture component");
            }
        }
    }
}
