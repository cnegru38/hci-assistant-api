using UnityEngine;
using TMPro;

public class TextChange : MonoBehaviour
{
    public TextMeshProUGUI screenText;

    public void ChangeText()
    {
        this.screenText.text = "Lighter - potential LPG gas source";
    }
}
