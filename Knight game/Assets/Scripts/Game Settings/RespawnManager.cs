using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("Respawn")]
    public Transform currentSpawnPoint;
    public float respawnDelay = 0.25f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetSpawnPoint(Transform newSpawn)
    {
        currentSpawnPoint = newSpawn;
    }

    public void Respawn(PlayerLife player)
    {
        StartCoroutine(RespawnRoutine(player));
    }

    private IEnumerator RespawnRoutine(PlayerLife player)
    {
        if (currentSpawnPoint == null)
        {
            Debug.LogError("No spawn point set in RespawnManager!");
            yield break;
        }

        player.SetControlEnabled(false);

        // Optional small delay (feel free to set to 0)
        yield return new WaitForSeconds(respawnDelay);

        // Reset physics + teleport
        player.TeleportTo(currentSpawnPoint.position);

        player.SetControlEnabled(true);
    }
}
