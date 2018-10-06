using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertPopup : MonoBehaviour {
    public Text text;
    public void SetText(string text) {
        this.text.text = text;
    }
    public void OnClick() {
        Destroy(gameObject);
    }
}
