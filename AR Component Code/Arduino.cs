using UnityEngine;
using System.IO.Ports;
using TMPro;

public class Arduino : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM3", 9600);

    [Header("Assign your TMP Text here")]
    public TMP_Text targetText;

    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }

    void Update()
    {
        if (sp != null && sp.IsOpen)
        {
            try
            {
                int value = sp.ReadByte();
                ControlTextVisibility(value);
            }
            catch (System.Exception)
            {
            }
        }
    }

    void ControlTextVisibility(int value)
    {
        if (targetText == null) return;

        if (value == 1)
        {
            targetText.gameObject.SetActive(true);   // SHOW
        }
        else if (value == 2)
        {
            targetText.gameObject.SetActive(false);  // HIDE
        }
    }
}
