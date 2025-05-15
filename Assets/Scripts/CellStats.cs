[System.Serializable]
public class CellStats
{
    public int health;
    public int resistance;
    public int reproductionRate;
    public float velocity;

    public int strength;
    public bool infected;

    public CellStats(CellData data)
    {
        health = data.health;
        resistance = data.resistance;
        reproductionRate = data.reproductionRate;
        velocity = data.velocity;

        // Detecta tipo em tempo de execução e copia propriedades específicas
        if (data is WhiteBloodCellData wbc)
        {
            strength = wbc.strength;
        }
        else if (data is VirusData virus)
        {
            strength = virus.strength;
        }
        else if (data is RedBloodCellData rbc)
        {
            infected = rbc.infected;
        }
    }
}
