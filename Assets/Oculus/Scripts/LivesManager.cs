using UnityEngine;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance;

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;
    public AudioClip heartbeatSound;

    private int currentLives = 3;
    private AudioSource audioSource;
    private HeartBeat heart1Beat;

    void Awake()
    {
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = heartbeatSound;
    }

    void Start()
    {
        heart1Beat = heart1.GetComponentInChildren<HeartBeat>();
    }

    public void LoseLife()
    {
        currentLives--;

        switch (currentLives)
        {
            case 2:
                heart3.SetActive(false);
                break;
            case 1:
                heart2.SetActive(false);
                heart1Beat.StartBeating();
                if (heartbeatSound != null) audioSource.Play();
                break;
            case 0:
                heart1.SetActive(false);
                audioSource.Stop();
                GameManager.Instance.GameOver();
                break;
        }
    }

    public void GainLife()
    {
        if (currentLives >= 3) return;
        currentLives++;

        switch (currentLives)
        {
            case 2:
                heart2.SetActive(true);
                heart1Beat.StopBeating();
                audioSource.Stop();
                break;
            case 3:
                heart3.SetActive(true);
                break;
        }
    }

    public void ResetLives()
    {
        currentLives = 3;
        heart1.SetActive(true);
        heart2.SetActive(true);
        heart3.SetActive(true);
        heart1Beat.StopBeating();
        audioSource.Stop();
    }

    public int GetLives() => currentLives;
}