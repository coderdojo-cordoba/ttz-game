using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class copyTo : MonoBehaviour
{
    string text;
    public void setButton(string s)
    {
        text = s;
        GetComponent<Button>().onClick.AddListener(copy);
    }
    public void copy()
    {
        GUIUtility.systemCopyBuffer = text;
    }
}
