using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            if (playerMovement != null)
            {
                playerMovement.coinCount++;
                PlayerPrefs.SetInt("CoinCount", playerMovement.coinCount);
                Destroy(other.gameObject); // Destroy the coin
            }
        }
    }
}