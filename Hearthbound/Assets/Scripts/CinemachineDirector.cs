using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CinemachineDirector : MonoBehaviour
{
    [Header("Cinemachine Cameras (CM3)")]
    [SerializeField] private CinemachineCamera gameplayCam;
    [SerializeField] private CinemachineCamera buildCam;
    [SerializeField] private CinemachineCamera bonfireCam;

    [Header("Priorities")]
    [SerializeField] private int gameplayPriority = 10;
    [SerializeField] private int activePriority = 20;

    [Header("Timings")]
    [SerializeField] private float bonfireHoldSeconds = 1.2f;

    private Coroutine _routine;

    private void Start()
    {
        SetOnly(gameplayCam);
    }

    public void EnterBuildView() => SetOnly(buildCam);

    public void ExitToGameplay() => SetOnly(gameplayCam);

    public void ShowBonfireThenReturn()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(BonfireRoutine());
    }

    private IEnumerator BonfireRoutine()
    {
        SetOnly(bonfireCam);
        yield return new WaitForSeconds(bonfireHoldSeconds);
        SetOnly(gameplayCam);
        _routine = null;
    }

    private void SetOnly(CinemachineCamera active)
    {
        if (gameplayCam == null || buildCam == null || bonfireCam == null)
        {
            Debug.LogWarning("CinemachineDirectorCM3: Assign all three cameras in the Inspector.");
            return;
        }

        gameplayCam.Priority = (active == gameplayCam) ? activePriority : gameplayPriority;
        buildCam.Priority    = (active == buildCam)    ? activePriority : 0;
        bonfireCam.Priority  = (active == bonfireCam)  ? activePriority : 0;
    }
}