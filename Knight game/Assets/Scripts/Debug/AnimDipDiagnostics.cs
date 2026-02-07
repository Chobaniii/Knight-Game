using UnityEngine;

public class AnimDipDiagnostics : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Collider2D bodyCollider;

    [Header("Logging")]
    public KeyCode dumpKey = KeyCode.K;
    public float logThreshold = 0.01f; // how much change triggers a log

    float baseLocalY;
    float baseSpriteMinY;
    float baseDeltaFeet; // spriteMinY - colliderMinY

    void Reset()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        baseLocalY = transform.localPosition.y;
        baseSpriteMinY = spriteRenderer ? spriteRenderer.bounds.min.y : 0f;
        baseDeltaFeet = GetFeetDelta();
        Debug.Log($"[AnimDipDiagnostics] Baseline localY={baseLocalY:F4}, spriteMinY={baseSpriteMinY:F4}, feetDelta(spriteMinY-colliderMinY)={baseDeltaFeet:F4}");
    }

    // LateUpdate runs after animation has been applied for the frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(dumpKey))
        {
            Dump("ManualDump");
        }

        // Auto-log if something changes noticeably
        float localY = transform.localPosition.y;
        float spriteMinY = spriteRenderer ? spriteRenderer.bounds.min.y : 0f;
        float feetDelta = GetFeetDelta();

        if (Mathf.Abs(localY - baseLocalY) > logThreshold ||
            Mathf.Abs(spriteMinY - baseSpriteMinY) > logThreshold ||
            Mathf.Abs(feetDelta - baseDeltaFeet) > logThreshold)
        {
            Dump("AutoChange");
            baseLocalY = localY;
            baseSpriteMinY = spriteMinY;
            baseDeltaFeet = feetDelta;
        }
    }

    float GetFeetDelta()
    {
        if (!spriteRenderer || !bodyCollider) return 0f;
        return spriteRenderer.bounds.min.y - bodyCollider.bounds.min.y;
    }

    void Dump(string reason)
    {
        string stateName = "(no animator)";
        float normTime = 0f;

        if (animator)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            stateName = st.IsName("") ? st.shortNameHash.ToString() : st.shortNameHash.ToString();
            normTime = st.normalizedTime;
        }

        string spriteName = spriteRenderer && spriteRenderer.sprite ? spriteRenderer.sprite.name : "None";
        float localY = transform.localPosition.y;
        float worldY = transform.position.y;
        float spriteMinY = spriteRenderer ? spriteRenderer.bounds.min.y : 0f;
        float colliderMinY = bodyCollider ? bodyCollider.bounds.min.y : 0f;
        float feetDelta = GetFeetDelta();

        // Sprite pivot info (helpful for alignment issues)
        string pivotInfo = "";
        if (spriteRenderer && spriteRenderer.sprite)
        {
            var s = spriteRenderer.sprite;
            Vector2 pivotPixels = s.pivot;
            pivotInfo = $"pivot(px)=({pivotPixels.x:F1},{pivotPixels.y:F1}) ppu={s.pixelsPerUnit}";
        }

        Debug.Log(
            $"[AnimDipDiagnostics:{reason}] sprite={spriteName} stateHash={stateName} normTime={normTime:F2} | " +
            $"localY={localY:F4} worldY={worldY:F4} | " +
            $"spriteMinY={spriteMinY:F4} colliderMinY={colliderMinY:F4} feetDelta={feetDelta:F4} | {pivotInfo}"
        );
    }
}













