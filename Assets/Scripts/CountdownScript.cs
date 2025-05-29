using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownScript : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private StatsManagerScript statsManager;

    [SerializeField] private float countdownDuration = 60f;
    private float currentTime;
    private Coroutine countdownRoutine;

    public void StartCountdownCycle()
    {
        if (countdownRoutine == null)
        {
            currentTime = countdownDuration;
            countdownRoutine = StartCoroutine(CountdownLoop());
        }
    }

    public void StopCountdownCycle()
    {
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
            if (timerText != null)
                timerText.text = $"00:00";
        }
    }

    private IEnumerator CountdownLoop()
    {
        while (true)
        {
            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI();
                yield return null;
            }

            currentTime = countdownDuration;
            RefillDoses();
        }
    }

    private void UpdateTimerUI()
    {
        int totalSeconds = Mathf.CeilToInt(currentTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }


    private void RefillDoses()
    {
        int newDoses = Random.Range(1, 21);
        statsManager.AddDoses(newDoses);
        // Debug.Log($"Novas {newDoses} doses concedidas.");
    }
}
