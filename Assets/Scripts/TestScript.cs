using System;
using TMPro;
using UnityEngine;

public class TestScript:MonoBehaviour
{
    public TextMeshPro b;
    public TMP_Text c;

    public void Awake()
    {
        Debug.Log("bType="+b.GetType());
        Debug.Log("cType="+c.GetType());
    }
}