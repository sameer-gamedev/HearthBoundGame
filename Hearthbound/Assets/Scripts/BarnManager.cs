using UnityEngine;
using DG.Tweening;

/// <summary>
/// Orchestrates the barn build: enables BarnOuterRoot, then drops each module
/// from above one-by-one, while playing the player's repair animation.
/// </summary>
public class BarnManager : MonoBehaviour
{
    [Header("Barn Outer (Modular)")]
    [SerializeField] private GameObject barnOuterRoot;           // Parent of all modules
    [SerializeField] private Transform[] modulesInBuildOrder;    // Drag 6 children here in desired order

    [Header("Drop Animation")]
    [SerializeField] private float dropHeight = 2.5f;            // World units above final position
    [SerializeField] private float dropDuration = 0.45f;         // Seconds for each piece to drop
    [SerializeField] private Ease dropEase = Ease.OutBounce;     // Ease.OutCubic is also nice
    [SerializeField] private float delayBetweenPieces = 0.55f;   // Extra delay after each drop (total per piece ≈ dropDuration + delay)

    [Header("Optional Polish")]
    [SerializeField] private bool doTinyPunchOnLand = true;
    [SerializeField] private float punchScale = 0.08f;
    [SerializeField] private float punchDuration = 0.18f;

    [Header("Player Repair Animation")]
    [SerializeField] private RepairAction repairAction;          // Your existing component on player

    [Header("Optional: Interior")]
    [SerializeField] private GameObject barnInterior;            // If you want to hide it after build
    [SerializeField] private bool hideInteriorOnComplete = false;

    private Vector3[] _finalPos;
    private Quaternion[] _finalRot;
    private Vector3[] _finalScale;

    private Sequence _buildSeq;
    private bool _isBuilding;
    private bool _built;

    private void Awake()
    {
        CacheFinalTransforms();

        // Start with outer hidden (if you prefer it off at start)
        if (barnOuterRoot != null)
            barnOuterRoot.SetActive(false);

        // Ensure all modules start hidden for the build reveal
        HideAllModules();
    }

    private void OnDisable()
    {
        // Prevent tweens from running on disabled object
        _buildSeq?.Kill();
        _buildSeq = null;
    }

    private void CacheFinalTransforms()
    {
        if (modulesInBuildOrder == null) return;

        int n = modulesInBuildOrder.Length;
        _finalPos = new Vector3[n];
        _finalRot = new Quaternion[n];
        _finalScale = new Vector3[n];

        for (int i = 0; i < n; i++)
        {
            var t = modulesInBuildOrder[i];
            if (t == null) continue;

            _finalPos[i] = t.position;
            _finalRot[i] = t.rotation;
            _finalScale[i] = t.localScale;
        }
    }

    private void HideAllModules()
    {
        if (modulesInBuildOrder == null) return;
        for (int i = 0; i < modulesInBuildOrder.Length; i++)
        {
            if (modulesInBuildOrder[i] != null)
                modulesInBuildOrder[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Call this when you press E (or from your MenuManager) to start building.
    /// </summary>
    public void StartBuildSequence()
    {
        if (_isBuilding || _built) return;

        if (barnOuterRoot == null || modulesInBuildOrder == null || modulesInBuildOrder.Length == 0)
        {
            Debug.LogWarning("BarnManager: Assign barnOuterRoot and modulesInBuildOrder.");
            return;
        }

        _isBuilding = true;

        // Start repair animation + disable controller (your RepairAction already does this)
        if (repairAction != null)
            repairAction.StartRepair();

        // Activate outer root and (re)hide modules so sequence is deterministic
        barnOuterRoot.SetActive(true);
        HideAllModules();

        // Kill any old sequence
        _buildSeq?.Kill();
        _buildSeq = DOTween.Sequence();

        // Build sequence: drop each module from above
        for (int i = 0; i < modulesInBuildOrder.Length; i++)
        {
            int idx = i;
            Transform m = modulesInBuildOrder[idx];
            if (m == null) continue;

            _buildSeq.AppendCallback(() =>
            {
                // Reset to final transform data in case you re-run in editor
                m.gameObject.SetActive(true);
                m.rotation = _finalRot[idx];
                m.localScale = _finalScale[idx];

                // Start above final position
                Vector3 finalPos = _finalPos[idx];
                m.position = finalPos + Vector3.up * dropHeight;

                // Drop tween
                m.DOMove(finalPos, dropDuration)
                 .SetEase(dropEase);

                // Tiny punch on land (optional)
                if (doTinyPunchOnLand)
                {
                    m.DOPunchScale(Vector3.one * punchScale, punchDuration, vibrato: 6, elasticity: 0.7f)
                     .SetDelay(dropDuration * 0.85f);
                }
            });

            // Wait: drop + extra delay before next piece
            _buildSeq.AppendInterval(dropDuration + delayBetweenPieces);
        }

        _buildSeq.OnComplete(() =>
        {
            _built = true;
            _isBuilding = false;

            if (repairAction != null)
                repairAction.StopRepair();

            if (hideInteriorOnComplete && barnInterior != null)
                barnInterior.SetActive(false);
        });

        _buildSeq.Play();
    }

    /// <summary>
    /// Optional helper if you want to force-reset during testing.
    /// </summary>
    public void ResetBuildForTesting()
    {
        _buildSeq?.Kill();
        _buildSeq = null;

        _built = false;
        _isBuilding = false;

        if (barnOuterRoot != null)
            barnOuterRoot.SetActive(false);

        HideAllModules();

        // Restore transforms (so you can re-run without drift)
        if (modulesInBuildOrder == null) return;
        for (int i = 0; i < modulesInBuildOrder.Length; i++)
        {
            var t = modulesInBuildOrder[i];
            if (t == null) continue;
            t.position = _finalPos[i];
            t.rotation = _finalRot[i];
            t.localScale = _finalScale[i];
        }

        if (barnInterior != null)
            barnInterior.SetActive(true);
    }
}
