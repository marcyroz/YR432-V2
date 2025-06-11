using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Dialog/Dialog Sequence")]
public class DialogData : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] lines;

    public string spriteName;
}
