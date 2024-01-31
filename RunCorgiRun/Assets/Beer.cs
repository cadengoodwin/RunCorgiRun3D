using UnityEngine;

public class Beer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming you have a Player script, call a method to handle beer pickup
            other.GetComponent<Player>().PickUpBeer();
            Destroy(gameObject);
        }
    }
}