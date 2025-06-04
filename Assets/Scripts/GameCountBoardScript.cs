using TMPro;
using UnityEngine;

public class GameCountBoardScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI virusKilledText;
    [SerializeField] private TextMeshProUGUI wbcKilledText;
    [SerializeField] private TextMeshProUGUI rbcTransformedText;
    [SerializeField] private TextMeshProUGUI rbcInfectedText;
    [SerializeField] private TextMeshProUGUI rbcCuredText;
    [SerializeField] private TextMeshProUGUI totalCellsBornText;

    void Start()
    {
        var stats = GameStatsTracker.Instance;

        if (stats == null) return;

        virusKilledText.text = stats.virusKilled.ToString();
        wbcKilledText.text = stats.wbcKilled.ToString();
        rbcTransformedText.text = stats.rbcTransformed.ToString();
        rbcInfectedText.text = stats.rbcInfected.ToString();
        rbcCuredText.text = stats.rbcCured.ToString();
        totalCellsBornText.text = stats.totalCellsBorn.ToString();
    }
}
