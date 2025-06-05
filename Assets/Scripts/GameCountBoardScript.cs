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
        RefreshBoard();
    }

    public void RefreshBoard()
    {

        var stats = GameStatsTracker.Instance;

        if (stats == null)
        {
            Debug.LogError("GameStatsTracker.Instance está null no RefreshBoard");
            return;
        }

        virusKilledText.text = stats.virusKilled.ToString();
        wbcKilledText.text = stats.wbcKilled.ToString();
        rbcTransformedText.text = stats.rbcTransformed.ToString();
        rbcInfectedText.text = stats.rbcInfected.ToString();
        rbcCuredText.text = stats.rbcCured.ToString();
        totalCellsBornText.text = stats.totalCellsBorn.ToString();

        Debug.Log(
            $"Stats Updated: Virus Killed: {stats.virusKilled}, WBC Killed: {stats.wbcKilled}, RBC Transformed: {stats.rbcTransformed}, RBC Infected: {stats.rbcInfected}, RBC Cured: {stats.rbcCured}, Total Cells Born: {stats.totalCellsBorn}"
        );
    }

}
