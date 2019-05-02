using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChange : MonoBehaviour {

    public Text text;

    public void ChangeText(float number)
    {
       /* Check if number is whole
        * If it is a whole number display without decimals else with two decimals
        * This is done so that the iso slider only displays two decimals
        */
        if(number % 1 == 0)
            text.text = number.ToString();
        else
            text.text = number.ToString("f2");
    }
}
