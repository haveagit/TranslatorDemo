using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingletonText : Singleton<SingletonText>
{

    public void SetText(string s)
    {
        gameObject.GetComponent<Text>().text = s;
    }
}