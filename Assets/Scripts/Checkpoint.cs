using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Transform spawnLocation = null;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null) return;

        player.UpdateCheckpoint(spawnLocation);
    }
}
