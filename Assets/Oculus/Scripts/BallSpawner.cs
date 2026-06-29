using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject mathBallPrefab;
    public Transform player;
    public RingMover[] rings;

    private float nextSpawnTime;
    private bool active = false;
    private float currentSpawnInterval = 3f;
    private float baseTierSpeed = 4.0f;

    public void StartSpawning()
    {
        active = true;
        nextSpawnTime = Time.time + 2f;
    }

    public void StopSpawning()
    {
        active = false;
    }

    void Update()
    {
        if (!active) return;
        if (Time.time >= nextSpawnTime)
        {
            SpawnBall();
            nextSpawnTime = Time.time +
                Random.Range(currentSpawnInterval * 0.8f,
                             currentSpawnInterval * 1.2f);
        }
    }

    

    public void ApplyTierSettings(TierSettings settings)
    {
        currentSpawnInterval = settings.spawnInterval;
        baseTierSpeed = settings.ballSpeed;
    }

    void SpawnBall()
    {
        int target = GameManager.Instance.targetNumber;
        bool correct = Random.value < 0.6f;

        string equation = correct
            ? GenerateCorrect(target)
            : GenerateWrong(target);

        int ringIndex = Random.Range(0, rings.Length);
        RingMover chosenRing = rings[ringIndex];
        chosenRing.FlashShoot();

        GameObject ball = Instantiate(mathBallPrefab,
            chosenRing.transform.position, Quaternion.identity);

        // reset scale to 1 at spawn
        ball.transform.localScale = Vector3.one;

        MathBall mb = ball.GetComponent<MathBall>();

        float dynamicSpeed = baseTierSpeed
            + (GameManager.Instance.score * 0.005f)  // very gradual score influence
            + (GameManager.Instance.GetCurrentStreak() * 0.05f); // streak influence
        mb.speed = Mathf.Min(dynamicSpeed, 15f);

        mb.Launch(equation, correct, player);
    }

    string GenerateCorrect(int x)
    {
        int level = GameManager.Instance.currentLevel;
        int maxOp = level < 3 ? 2 : 4;
        int op = Random.Range(0, maxOp);

        if (op == 0)
        {
            int y = Random.Range(1, Mathf.Min(x, GetMaxOperand(level)));
            return (x + y) + " - " + y;
        }
        else if (op == 1)
        {
            if (x <= 1) goto fallback;
            int y = Random.Range(1, x);
            return (x - y) + " + " + y;
        }
        else if (op == 2)
        {
            int y = Random.Range(2, GetMaxOperand(level));
            return (x * y) + " / " + y;
        }
        else
        {
            int[] factors = GetFactors(x);
            if (factors.Length == 0) goto fallback;
            int f = factors[Random.Range(0, factors.Length)];
            return f + " * " + (x / f);
        }

        fallback:
        int yy = Random.Range(1, GetMaxOperand(level));
        return (x + yy) + " - " + yy;
    }

    string GenerateWrong(int x)
    {
        int level = GameManager.Instance.currentLevel;
        int wrong;
        do { wrong = Random.Range(2, 15 + level); } while (wrong == x);
        int y = Random.Range(1, wrong);
        return y + " + " + (wrong - y);
    }

    int GetMaxOperand(int level)
    {
        return Mathf.Min(3 + level, 10);
    }

    int[] GetFactors(int n)
    {
        var factors = new System.Collections.Generic.List<int>();
        for (int i = 2; i <= n / 2; i++)
            if (n % i == 0) factors.Add(i);
        return factors.ToArray();
    }
}