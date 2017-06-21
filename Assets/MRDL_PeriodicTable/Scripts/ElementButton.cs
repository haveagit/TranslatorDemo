//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using HUX.Interaction;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ElementButton : Button {    
    
    public override void Pressed(InteractionManager.InteractionEventArgs args)
    {
        Element element = gameObject.GetComponent<Element>();
        StartCoroutine(ExecuteTranslate(element.ElementDescription.text)); //翻訳処理の呼び出し

        // User has clicked us
        // If we're the active element button, reset ourselves
        if (Element.ActiveElement == element)
        {
            // If we're the current element, reset ourselves
            Element.ActiveElement = null;
        }
        else
        {
            Element.ActiveElement = element;
            element.Open();
        }
    }

    //追加メソッド
    System.Collections.IEnumerator ExecuteTranslate(string text)
    {
        string translateTokenUrl = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
        var translateHeaders = new Dictionary<string, string>() {
            { "Content-Type" , "application/json" },
            { "Ocp-Apim-Subscription-Key", "APPKEY" } //自身のAPPキーを入力
        };

        WWW tokenWWW = new WWW(translateTokenUrl, new byte[1], translateHeaders);
        yield return tokenWWW;

        string req = text.Replace(" ", "%20"); //英文に含まれる半角スペースを変換

        string japanUrl = "https://api.microsofttranslator.com/v2/http.svc/Translate?appid=Bearer%20" + tokenWWW.text + "&text=" + req + "&from=en&to=ja";

        WWW getWWW = new WWW(japanUrl);
        yield return getWWW;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(getWWW.text));
        XmlNode root = xmlDoc.FirstChild;
        XmlNodeList talkList = xmlDoc.GetElementsByTagName("string");
        XmlNode talk0 = talkList[0];
        string jaString = talk0.InnerText;

        SingletonText.Instance.SetText(jaString);
    }

    public override void OnStateChange(ButtonStateEnum newState)
    {
        Element element = gameObject.GetComponent<Element>();

        switch (newState)
        {
            case ButtonStateEnum.ObservationTargeted:
            case ButtonStateEnum.Targeted:
                // If we're not the active element, light up
                if (Element.ActiveElement != this)
                {
                    element.Highlight();
                }
                break;

            default:
                element.Dim();
                break;
        }

        base.OnStateChange(newState);
    }
}
