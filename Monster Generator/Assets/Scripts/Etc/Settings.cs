using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour{

    const float MIN_ISO_LEVEL = 0, MAX_ISO_LEVEL = 100;
    const int MAX_ARMS = 4, MAX_HEADS = 4, MAX_LEGS = 4;

    public int amountArms, amountHeads, amountLegs;
    public float isoLevel = 15;

    public Slider legSlider, armSlider, headSlider, isoSlider;

    public Settings(int arms, int heads, int legs, float isoLevel)
    {
        amountArms = arms;
        amountLegs = legs;
        amountHeads = heads;
        this.isoLevel = isoLevel; 
    }

    public void ChangeArmAmount(float arms)
    {
        amountArms = (int) arms;
    }

    public void ChangeLegAmount(float legs)
    {
        amountLegs = (int)legs;
    }

    public void ChangeHeadAmount(float heads)
    {
        amountHeads = (int) heads;
    }

    public void ChangeIsoLevel(float isoLevel)
    {
        this.isoLevel = isoLevel;
    }

    public void Random()
    {
        // no need to change values here as sliders onvaluechanged events do it too
        legSlider.value = UnityEngine.Random.Range(0, MAX_LEGS + 1);
        armSlider.value = UnityEngine.Random.Range(0, MAX_ARMS + 1);
        headSlider.value = UnityEngine.Random.Range(0, MAX_HEADS + 1);

        isoSlider.value = UnityEngine.Random.Range(MIN_ISO_LEVEL, MAX_ISO_LEVEL + 1);
    }
}
