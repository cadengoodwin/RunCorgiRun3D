using UnityEngine;
using UnityEngine.UI;

public class CoinDisplay : MonoBehaviour
{
    public Text coinText;

    void Update()
    {
        int coinCount = PlayerPrefs.GetInt("CoinCount", 0);
        coinText.text = "Coins: " + coinCount;
    }
}