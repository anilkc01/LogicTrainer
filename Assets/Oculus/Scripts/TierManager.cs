using UnityEngine;
using TMPro;

public class TierManager : MonoBehaviour
{
    public static TierManager Instance;
    public TextMeshPro tierDisplay;

    // tier thresholds in RANKING POINTS (not score)
    private int[] tierThresholds = { 0, 100, 250, 450, 700, 1000, 1350, 1750, 2200, 2700 };
    private string[] tierNames = {
        "Novice", "Learner", "Thinker", "Solver", "Strategist",
        "Ace", "Expert", "Master", "Genius", "Conqueror"
    };

    // settings per tier — minor changes
    private float[] spawnIntervals = { 2.1f, 1.98f, 1.86f, 1.74f, 1.62f, 1.5f, 1.32f, 1.2f, 1.02f, 0.84f };
    private float[] ballSpeeds = { 7.0f, 7.75f, 8.5f, 9.0f, 9.4f, 9.75f, 10.5f, 12.0f, 13.5f, 15.0f };
    private int[]   targetMins =     { 1,    1,    1,    2,    2,    3,    3,    4,    5,    6   };
    private int[]   targetMaxes =    { 10,   11,   12,   12,   13,   14,   15,   17,   20,   25  };

    // ranking points — this is the spectrum
    private float rankingPoints = 0f;
    private int currentTier = 0;
    private int previousTier = 0;

    // points gained/lost per action
    private const float CORRECT_PUNCH_POINTS  =  8f;
    private const float WRONG_PUNCH_POINTS    = -12f;
    private const float MISSED_CORRECT_POINTS = -5f;
    private const float WRONG_HIT_POINTS      = -10f;
    private const float DODGED_WRONG_POINTS   =  2f;

    void Awake()
    {
        Instance = this;
        // load saved ranking points
        rankingPoints = PlayerPrefs.GetFloat("RankingPoints", 0f);
        rankingPoints = Mathf.Max(0f, rankingPoints); // never below 0
        currentTier = GetTierFromPoints(rankingPoints);
        previousTier = currentTier;
    }

    // call these from GameManager instead of direct tier checks
    public void OnCorrectPunch(float velocity)
    {
        // harder punch = slightly more ranking points
        float bonus = Mathf.Clamp(velocity * 0.5f, 0f, 3f);
        AddPoints(CORRECT_PUNCH_POINTS + bonus);
    }

    public void OnWrongPunch()
    {
        AddPoints(WRONG_PUNCH_POINTS);
    }

    public void OnMissedCorrect()
    {
        AddPoints(MISSED_CORRECT_POINTS);
    }

    public void OnWrongHitPlayer()
    {
        AddPoints(WRONG_HIT_POINTS);
    }

    public void OnDodgedWrong()
    {
        AddPoints(DODGED_WRONG_POINTS);
    }

    void AddPoints(float points)
    {
        rankingPoints = Mathf.Max(0f, rankingPoints + points);

        // save immediately
        PlayerPrefs.SetFloat("RankingPoints", rankingPoints);
        PlayerPrefs.Save();

        // check tier change
        int newTier = GetTierFromPoints(rankingPoints);
        if (newTier != currentTier)
        {
            previousTier = currentTier;
            currentTier = newTier;
            OnTierChanged();
        }

        UpdateTierDisplay();
    }

    void OnTierChanged()
    {
        bool promoted = currentTier > previousTier;
        Debug.Log(promoted
            ? "PROMOTED to " + tierNames[currentTier]
            : "DEMOTED to " + tierNames[currentTier]);

        // you can trigger promotion/demotion sounds here later
    }

    int GetTierFromPoints(float points)
    {
        int tier = 0;
        for (int i = tierThresholds.Length - 1; i >= 0; i--)
        {
            if (points >= tierThresholds[i])
            {
                tier = i;
                break;
            }
        }
        return Mathf.Clamp(tier, 0, tierNames.Length - 1);
    }

    public TierSettings GetCurrentSettings()
    {
        return new TierSettings
        {
            spawnInterval = spawnIntervals[currentTier],
            ballSpeed     = ballSpeeds[currentTier],
            targetMin     = targetMins[currentTier],
            targetMax     = targetMaxes[currentTier],
            tierName      = tierNames[currentTier],
            tier          = currentTier,
            rankingPoints = rankingPoints
        };
    }

    public void UpdateTierDisplay()
    {
        if (tierDisplay != null)
            tierDisplay.text = tierNames[currentTier] + 
                               "\n<size=60%>" + Mathf.RoundToInt(rankingPoints) + " RP</size>";
    }

    public string GetTierName() => tierNames[currentTier];
    public int GetCurrentTier() => currentTier;
    public float GetRankingPoints() => rankingPoints;
}

public class TierSettings
{
    public float spawnInterval;
    public float ballSpeed;
    public int targetMin;
    public int targetMax;
    public string tierName;
    public int tier;
    public float rankingPoints;
}