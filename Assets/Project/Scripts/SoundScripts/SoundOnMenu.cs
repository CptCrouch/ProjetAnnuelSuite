using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundOnMenu : MonoBehaviour {

    public Image textToChange;
    public Color colorToChange = Color.white;

    

    public void OnMouseEnterPointer()
    {
        if(textToChange != null)
            textToChange.color = colorToChange;

        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuMouseOver");
    }
    public void OnMouseExitPointer()
    {
        if (textToChange != null)
            textToChange.color = GetComponent<Button>().colors.normalColor;
    }

}
