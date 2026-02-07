using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var life = other.GetComponent<PlayerLife>();
        if (life != null) life.Die();
    }
}
