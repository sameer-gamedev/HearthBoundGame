using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private CinemachineDirector camDirector;
    [SerializeField] private BarnManager barnManager;


    [Header("UI References")]
    [SerializeField] private GameObject buildBarnLabel;

    private void Start()
    {
        if (buildBarnLabel != null)
            buildBarnLabel.SetActive(false);
    }

    private void Update()
    {
        // Only respond if the label is visible
        if (buildBarnLabel != null && buildBarnLabel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EnterBuildMode();
            }
        }
    }

    private void EnterBuildMode()
    {
        Debug.Log("E pressed: entering Build camera");

        if (camDirector != null)
            camDirector.EnterBuildView();
        
        if (barnManager != null)
            barnManager.StartBuildSequence();
        
        buildBarnLabel.SetActive(false);
    }

    /// <summary>
    /// Called by trigger when player enters/exits range
    /// </summary>
    public void ShowBuildButton(bool show)
    {
        if (buildBarnLabel != null)
            buildBarnLabel.SetActive(show);
    }
}