using System.Collections;
using UnityEngine;

public class GoodEndingManager : MonoBehaviour
{
    [SerializeField] private GameObject goodEndingScreen;
    [SerializeField] private float delayAfterImage = 7f;
    [SerializeField] private GameObject statsPanel;

    public void StartGoodEndingCycle()
    {
        goodEndingScreen.SetActive(true);
        StartCoroutine(ShowStatsAfterDelay());
    }

    private IEnumerator ShowStatsAfterDelay()
    {
        yield return new WaitForSeconds(delayAfterImage);
        statsPanel.SetActive(true);
    }

    public void EndGoodEndingCycle()
    {
        goodEndingScreen.SetActive(false);
        statsPanel.SetActive(false);
    }
}
