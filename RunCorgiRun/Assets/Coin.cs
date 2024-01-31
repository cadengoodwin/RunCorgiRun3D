using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Call a method on the Player script to handle coin pickup
            other.GetComponent<Player>().PickUpCoin();
            Destroy(gameObject); // Destroy the coin
        }
    }
}