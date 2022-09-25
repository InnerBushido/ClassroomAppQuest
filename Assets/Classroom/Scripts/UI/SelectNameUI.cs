using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectNameUI : MonoBehaviour
{
    public TextMeshProUGUI textInput;
    public TouchScreenKeyboard keyboard;

    private void Update()
    {
        if(textInput == null)
        {
            Debug.LogError("MISSING TEXTMESHPROUGUI");
            return;
        }

        if (keyboard != null)
        {
            if (textInput.text != keyboard.text && keyboard.text != "")
            {
                Debug.Log("Keyboard Text: " + keyboard.text);
                textInput.text = keyboard.text;
            }
        }
    }

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void OpenSystemKeyboardEmail()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.EmailAddress, false, false, false, false);
    }
}
