using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private MonoBehaviour controller; // your PlayerController2D script

    private bool isDead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<MonoBehaviour>(); // will be replaced below if needed
        // Prefer exact type if your script is named PlayerController2D:
        controller = GetComponent<PlayerController2D>();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        RespawnManager.Instance.Respawn(this);
    }

    public void SetControlEnabled(bool enabled)
    {
        if (controller != null) controller.enabled = enabled;

        if (!enabled)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void TeleportTo(Vector2 pos)
    {
        // Slight upward bump prevents spawning inside ground
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        isDead = false;
    }
}
