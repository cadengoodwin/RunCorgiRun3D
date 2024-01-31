using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bool isWebVersion; // To determine if it's a web version

    private int beerCount = 0;
    public Text coinCountText;
    private int coinCount;
    public GameObject DrunkImage;
    public PostProcessVolume postProcessVolume;
    public PostProcessProfile normalProfile;
    public PostProcessProfile drunkProfile;

    public AudioClip beerPickupSound;   // Sound for picking up beer
    public AudioClip coinPickupSound;    // Sound for picking up coin
    public AudioSource audioSource;      // AudioSource to play the sounds
    public AudioSource audioSource2;     // AudioSource to play the sounds

    private Camera mainCamera;
    private float originalFOV;

    public void Start()
    {
        if (PlayerPrefs.HasKey("CoinCount"))
        {
            coinCount = PlayerPrefs.GetInt("CoinCount");
        }
        else
        {
            PlayerPrefs.SetInt("CoinCount", 0);
        }
        
        if (coinCountText != null)
        {
            coinCountText.text = coinCount.ToString();
        }

        mainCamera = Camera.main;
        originalFOV = mainCamera.fieldOfView;
    }

    public void PickUpCoin()
    {
        coinCount++;
        PlayerPrefs.SetInt("CoinCount", coinCount); // Save the coin count

        // Update UI
        if (coinCountText != null)
        {
            coinCountText.text = coinCount.ToString();
        }

        // Play coin pickup sound
        audioSource.PlayOneShot(coinPickupSound);
    }

    public void PickUpBeer()
    {
        beerCount++;
        if (beerCount >= 3)
        {
            GetDrunk();
        }

        // Play beer pickup sound
        audioSource.PlayOneShot(beerPickupSound);
    }

    private void GetDrunk()
    {
        Debug.Log("Getting Drunk");

        if (isWebVersion)
        {
            mainCamera.fieldOfView = 30;
            DrunkImage.SetActive(true);
        }
        else
        {
            postProcessVolume.profile = drunkProfile;
        }

        Time.timeScale = 0.7f;
        GetComponent<PlayerMovement>().isDrunk = true;
        audioSource.pitch = 0.7f;
        audioSource2.pitch = 0.65f;

        Invoke("SoberUp", 5f);
    }

    private void SoberUp()
    {
        Debug.Log("Sobering Up");

        if (isWebVersion)
        {
            mainCamera.fieldOfView = originalFOV;
            DrunkImage.SetActive(false);

        }
        else
        {
            postProcessVolume.profile = normalProfile;
        }

        Time.timeScale = 1.0f;
        GetComponent<PlayerMovement>().isDrunk = false;
        beerCount = 0;
        audioSource.pitch = 1.0f;
        audioSource2.pitch = 1.0f;
    }

    public void InstantSoberUp()
    {
        Debug.Log("Instantly Sobering Up");
        CancelInvoke("SoberUp");
        SoberUp();
    }
}
