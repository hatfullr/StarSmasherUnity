using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class TextController : MonoBehaviour
{
    private TMPro.TMP_Text _text;

    [HideInInspector] public TMPro.TMP_Text text
    {
        get
        {
            if (_text == null) _text = GetComponent<TMPro.TMP_Text>();
            return _text;
        }
    }

    public void SetText(Slider slider) => text.SetText(slider.value.ToString());
    public void SetText(string text) => this.text.SetText(text);
}
