using System;
using UnityEngine;
using TMPro;

public class CountBoardScript : MonoBehaviour
{
    [SerializeField] private int virusCount;
    [SerializeField] private int redBloodCellCount;
    [SerializeField] private int infectedRedBloodCellCount;
    [SerializeField] private int whiteBloodCellCount;
    [SerializeField] private TextMeshProUGUI virusText;
    [SerializeField] private TextMeshProUGUI redBloodCellText;
    [SerializeField] private TextMeshProUGUI infectedRedBloodCellText;
    [SerializeField] private TextMeshProUGUI whiteBloodCellText;


    void Start()
    {
        UpdateUI();
    }

    public void addEntity(string entityType)
    {
        switch (entityType)
        {
            case "Virus":
                virusCount++;
                break;
            case "RBC":
                redBloodCellCount++;
                break;
            case "IRBC":
                infectedRedBloodCellCount++;
                break;
            case "WBC":
                whiteBloodCellCount++;
                break;
            default:
                Debug.LogWarning("Tipo de entidade desconhecido: " + entityType);
                return;
        }

        UpdateUI();
    }

    public void removeEntity(string entityType)
    {
        switch (entityType)
        {
            case "Virus":
                virusCount--;
                break;
            case "RBC":
                redBloodCellCount--;
                break;
            case "IRBC":
                infectedRedBloodCellCount--;
                break;
            case "WBC":
                whiteBloodCellCount--;
                break;
            default:
                Debug.LogWarning("Tipo de entidade desconhecido: " + entityType);
                return;
        }

        UpdateUI();
    }

    public void ResetCounts()
    {
        virusCount = 0;
        redBloodCellCount = 0;
        infectedRedBloodCellCount = 0;
        whiteBloodCellCount = 0;

        UpdateUI();
    }


    private void UpdateUI()
    {
        if (virusText != null)
            virusText.text = virusCount.ToString();

        if (redBloodCellText != null)
            redBloodCellText.text = redBloodCellCount.ToString();

        if (infectedRedBloodCellText != null)
            infectedRedBloodCellText.text = infectedRedBloodCellCount.ToString();

        if (whiteBloodCellText != null)
            whiteBloodCellText.text = whiteBloodCellCount.ToString();
    }

}
