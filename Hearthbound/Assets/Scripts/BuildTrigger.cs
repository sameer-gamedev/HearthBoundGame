using UnityEngine;

public class BuildTrigger : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;

    private void Reset()
    {
        // Ensure collider is trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (menuManager != null)
            menuManager.ShowBuildButton(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (menuManager != null)
            menuManager.ShowBuildButton(false);
    }
}