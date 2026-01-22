using UnityEngine;
using StarterAssets;

public class RepairAction : MonoBehaviour
{
    public Animator animator;
    public ThirdPersonController controller;

    bool isRepairing;

    public void StartRepair()
    {
        if (isRepairing) return;
        isRepairing = true;

        controller.enabled = false;
        animator.SetBool("IsRepairing", true);
    }

    public void StopRepair()
    {
        if (!isRepairing) return;
        isRepairing = false;

        animator.SetBool("IsRepairing", false);
        controller.enabled = true;
    }

    void Update()
    {
        // TEMP TEST: press E to start, press Q to stop
        // if (Input.GetKeyDown(KeyCode.E)) StartRepair();
        // if (Input.GetKeyDown(KeyCode.Q)) StopRepair();
    }
}