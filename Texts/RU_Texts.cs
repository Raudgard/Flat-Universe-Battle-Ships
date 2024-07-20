using Texts;
using UnityEngine;
using System;

public class RU_Texts : Texts.Texts
{
    #region Singleton
    private static RU_Texts instance;
    public static RU_Texts Instance
    {
        get
        {
            if (instance == null)
                instance = new RU_Texts();
            return instance;
        }
    }
    #endregion

    public RU_Texts()
    {
        var text = Resources.Load<TextAsset>("RU_Texts");

        ParseText(text.text);

        var exDialog = GetDialog(3, 1, "before");

        //Debug.Log(exDialog.NextPhrase().Text);
        //Debug.Log(exDialog.NextPhrase().Text);
        //Debug.Log(exDialog.NextPhrase().Text);
        //Debug.Log(exDialog.NextPhrase().Text);

    }





}


