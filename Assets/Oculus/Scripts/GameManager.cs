using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Home Screen UI")]
    public GameObject homeDisplay;           // Drag 'HomeDisplay' parent here
    public TextMeshPro homeTierDisplay;       // Drag 'Tier' child here
    public TextMeshPro homeHighScoreDisplay;   // Drag 'HighScore' child here

    [Header("Game Over Screen UI")]
    public GameObject gameOverDisplay;       // Drag 'GameOverDisplay' parent here
    public TextMeshPro gameOverTierDisplay;   // Drag 'Tier' child under GameOverDisplay here
    public TextMeshPro gameOverHighScoreDisplay; // Drag 'HighScore' child under GameOverDisplay here
    public TextMeshPro gameOverScoreDisplay;

    [Header("3D Game Over Model")]
    public GameObject gameOver3DModel;

    [Header("UI")]
    public TextMeshPro targetDisplay;
    public TextMeshPro scoreDisplay;
    public TextMeshPro streakText;
    public GameObject streakDisplay;
    public GameObject scoreCube;
    public GameObject livesObject;

    [Header("Game Objects")]
    public GameObject ringController;

    [Header("Audio")]
    public AudioClip targetChangeClip;
    public AudioClip gameOverClip;

    [Header("Streak Sounds")]
    public AudioClip nice;
    public AudioClip amazing;
    public AudioClip onFire;
    public AudioClip genius;
    public AudioClip legendary;
    public AudioClip conqueror;
    public AudioClip unbelievable;

    [Header("Scoring")]
    public int score { get; private set; }
    private int highScore = 0;

    [Header("Level & Streaks")]
    public int currentLevel = 1;
    private int ballsThisLevel = 0;
    private int correctThisLevel = 0;
    private int currentStreak = 0;

    public int targetNumber { get; private set; }
    private BallSpawner ballSpawner;
    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        

        if (homeDisplay != null) homeDisplay.SetActive(true);
        if (gameOverDisplay != null) gameOverDisplay.SetActive(false); 
        if (gameOver3DModel != null) gameOver3DModel.SetActive(false);

        if (homeHighScoreDisplay != null) homeHighScoreDisplay.text =  highScore.ToString();
        if (homeTierDisplay != null && TierManager.Instance != null) homeTierDisplay.text = TierManager.Instance.GetTierName();
    }

   public void StartGame()
    {
        scoreCube.SetActive(true);
        livesObject.SetActive(true);
        targetDisplay.gameObject.SetActive(true);
        if (homeDisplay != null) homeDisplay.SetActive(false);
        if (gameOverDisplay != null) gameOverDisplay.SetActive(false);
        if (gameOver3DModel != null) gameOver3DModel.SetActive(false);
        

        score = 0;
        currentLevel = 1;
        ballsThisLevel = 0;
        correctThisLevel = 0;
        currentStreak = 0;

        UpdateScoreUI();
        UpdateStreakUI();

        ringController.SetActive(true);
        ballSpawner = ringController.GetComponent<BallSpawner>();

        // apply tier settings first (sets base speed + interval)
        TierSettings settings = TierManager.Instance.GetCurrentSettings();
        ballSpawner.ApplyTierSettings(settings);

        // also update target pool from tier
        GenerateNewTargetInRange(settings.targetMin, settings.targetMax);

        ballSpawner.StartSpawning();

        // repeating target change uses tier range
        InvokeRepeating(nameof(GenerateNewTarget), 15f, 15f);

        LivesManager.Instance.ResetLives();
        TierManager.Instance.UpdateTierDisplay();
    }

    public void CorrectPunch(float velocity)
{
    int points = Mathf.Clamp(Mathf.RoundToInt(velocity), 1, 4);
    score += points;
    correctThisLevel++;
    ballsThisLevel++;
    currentStreak++;

    TierManager.Instance.OnCorrectPunch(velocity);

    if (ScorePopup.Instance != null) ScorePopup.Instance.Show("+" + points, true);

    UpdateScoreUI();
    UpdateStreakUI();
    CheckLevelUp();
    PlayHypeSound();
}

public void WrongPunch()
{
    ballsThisLevel++;
    ResetStreak();
    TierManager.Instance.OnWrongPunch();
    LivesManager.Instance.LoseLife();
    CheckLevelDown();

    if (ScorePopup.Instance != null) ScorePopup.Instance.Show("-5", false);
}

public void MissedCorrectBall()
{
    ballsThisLevel++;
    ResetStreak();
    TierManager.Instance.OnMissedCorrect();

    if (score == 0)
        LivesManager.Instance.LoseLife();
    else
    {
        score = Mathf.Max(0, score - 5);
        UpdateScoreUI();
    }
    CheckLevelDown();

    if (ScorePopup.Instance != null) ScorePopup.Instance.Show("-5", false);
}

public void WrongBallReachedPlayer()
{
    ResetStreak();
    TierManager.Instance.OnWrongHitPlayer();
    LivesManager.Instance.LoseLife();

    if (ScorePopup.Instance != null) ScorePopup.Instance.Show("-5", false);
}

    void UpdateStreakUI()
    {
        if (streakDisplay == null) return;

        if (currentStreak < 2)
        {
            streakDisplay.SetActive(false);
            return;
        }

        streakDisplay.SetActive(true);

        // bonus life at 8x streak if only 1 life left
        if (currentStreak == 8 && LivesManager.Instance.GetLives() == 1)
        {
            LivesManager.Instance.GainLife();
            Debug.Log("BONUS LIFE! Streak reward!");
        }

        if (currentStreak >= 20)
            streakText.text = "<color=#9B59B6>x" + currentStreak + "</color>";
        else if (currentStreak >= 17)
            streakText.text = "<color=#FF00FF>x" + currentStreak + "</color>";
        else if (currentStreak >= 14)
            streakText.text = "<color=#FF0000>x" + currentStreak + "</color>";
        else if (currentStreak >= 11)
            streakText.text = "<color=#FF4500>x" + currentStreak + "</color>";
        else if (currentStreak >= 8)
            streakText.text = "<color=#FFA500>x" + currentStreak + "</color>";
        else if (currentStreak >= 5)
            streakText.text = "<color=#FFD700>x" + currentStreak + "</color>";
        else
            streakText.text = "<color=#FFFF99>x" + currentStreak + "</color>";

        // bounce effect on every streak update
        StopCoroutine(nameof(BounceStreak));
        StartCoroutine(nameof(BounceStreak));
    }

    System.Collections.IEnumerator BounceStreak()
    {
        Vector3 originalScale = streakDisplay.transform.localScale;
        Vector3 bigScale = originalScale * 2f;
        float duration = 0.5f;
        float half = duration / 2f;

        // scale up
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            streakDisplay.transform.localScale = Vector3.Lerp(
                originalScale, bigScale, t / half);
            yield return null;
        }

        // scale back down
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            streakDisplay.transform.localScale = Vector3.Lerp(
                bigScale, originalScale, t / half);
            yield return null;
        }

        streakDisplay.transform.localScale = originalScale;
    }

    void ResetStreak()
    {
        currentStreak = 0;
        UpdateStreakUI();
    }

    void CheckLevelUp()
    {
        if (ballsThisLevel < 10) return;
        float accuracy = (float)correctThisLevel / ballsThisLevel;
        if (accuracy >= 0.7f)
        {
            currentLevel++;
            ballsThisLevel = 0;
            correctThisLevel = 0;
            UpdateTargetPool();
        }
    }

    void CheckLevelDown()
    {
        if (ballsThisLevel < 10) return;
        float accuracy = (float)correctThisLevel / ballsThisLevel;
        if (accuracy < 0.4f && currentLevel > 1)
        {
            currentLevel--;
            ballsThisLevel = 0;
            correctThisLevel = 0;
            UpdateTargetPool();
        }
    }

    void UpdateTargetPool()
    {
        int shift = (currentLevel - 1) / 5;
        int min = 1 + shift;
        int max = 11 + shift;
        GenerateNewTargetInRange(min, max);
    }

    void GenerateNewTarget()
    {
        int shift = (currentLevel - 1) / 5;
        int min = 1 + shift;
        int max = 11 + shift;
        GenerateNewTargetInRange(min, max);
    }

    void GenerateNewTargetInRange(int min, int max)
    {
        int newTarget;
        do { newTarget = Random.Range(min, max); }
        while (newTarget == targetNumber);
        targetNumber = newTarget;
        targetDisplay.text = "" + targetNumber;
        if (targetChangeClip != null)
            audioSource.PlayOneShot(targetChangeClip);
    }

    void UpdateScoreUI()
    {
        scoreDisplay.text = "Score: " + score;
    }

    void PlayHypeSound()
    {
        AudioClip clip = null;

        if (currentStreak >= 20)     clip = unbelievable;
        else if (currentStreak == 17) clip = conqueror;
        else if (currentStreak == 14) clip = legendary;
        else if (currentStreak == 11) clip = genius;
        else if (currentStreak == 8)  clip = onFire;
        else if (currentStreak == 5)  clip = amazing;
        else if (currentStreak == 2)  clip = nice;

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    // Add this helper property/method inside your GameManager class so BallSpawner can access it
    public int GetCurrentStreak()
    {
        return currentStreak;
    }

    

    public void GameOver()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (gameOverClip != null)
            audioSource.PlayOneShot(gameOverClip);

        ringController.SetActive(false);
        scoreCube.SetActive(false);
        livesObject.SetActive(false);
        if (streakDisplay != null) streakDisplay.SetActive(false);
        CancelInvoke();

        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("EqBall"))
            Destroy(ball);

        ballSpawner.StopSpawning();

        if (gameOver3DModel != null)
        {
            gameOver3DModel.SetActive(true);
        }

        // 2. Call the display transition after exactly 2 seconds
        Invoke(nameof(ShowGameOverInfoAndBag), 2f);
    }

    void ShowGameOverInfoAndBag()
    {
        // 3. Hide the 3D Model now that the 2 seconds have passed
        if (gameOver3DModel != null)
        {
            gameOver3DModel.SetActive(false);
        }
        
        // 4. Pop up the informational panel
        if (gameOverDisplay != null)
        {
            gameOverDisplay.SetActive(true);
            
            if (gameOverScoreDisplay != null) gameOverScoreDisplay.text = score.ToString();
            if (gameOverHighScoreDisplay != null) gameOverHighScoreDisplay.text = highScore.ToString();
            if (gameOverTierDisplay != null && TierManager.Instance != null) gameOverTierDisplay.text = TierManager.Instance.GetTierName();
        }

        // 5. Bring out the StartBag to allow replaying
        StartBag.Instance.ResetBag();
    }

    void ShowStartBag()
    {
        // Kept to prevent script compilation breaks across objects
        targetDisplay.gameObject.SetActive(false);
        StartBag.Instance.ResetBag();
    }
}