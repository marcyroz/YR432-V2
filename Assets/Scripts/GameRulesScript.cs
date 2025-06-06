using UnityEngine;

public class GameRulesScript : MonoBehaviour
{
    public CountBoardScript countBoard;
    private int virusCount = 0;
    private int wbcCount = 0;
    private int rbcCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    public string checkGame()
    {
        virusCount = countBoard.VirusCount;
        wbcCount = countBoard.WhiteBloodCellCount;
        rbcCount = countBoard.RedBloodCellCount;

        if (virusCount > wbcCount + rbcCount)
        {
            return "gameOver";
        }
        else if (virusCount < 10 && rbcCount > 50)
        {
            return "goodEnding";
        }
        else
        {
            return "continue";
        }
    }

}
