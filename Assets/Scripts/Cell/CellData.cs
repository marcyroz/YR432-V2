using UnityEngine;

[CreateAssetMenu(fileName = "NewCellData", menuName = "Cells/Cell Data")]
public class CellData : ScriptableObject
{
    public string entityType;
    public int health;
    public int resistance;
    public int reproductionRate;
    public float velocity;
}
