using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Texts
{
    public class Texts
    {
        protected Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();
        protected Dictionary<string, string> texts = new Dictionary<string, string>();


        public Dialog GetDialog(int levelNumber, int variant, string suffix)
        {
            string identificator = "" + levelNumber + "." + variant + "." + suffix;
            if (dialogs.ContainsKey(identificator))
                return dialogs[identificator];
            else
                return new Dialog("ERROR_DIALOG");
        }

        protected void ParseText(string text)
        {
            var chapters = text.Split(new[] { "[Header]" }, StringSplitOptions.RemoveEmptyEntries);

            //Debug.Log("chapters.count = " + chapters.Length);
            //Debug.Log(text);

            foreach (var chap in chapters)
            {
                //Debug.Log("NEW CHAPTER" + Environment.NewLine + chap);

                if (!chap.Contains("[NP]") && !chap.Contains("[Choice]"))
                    continue;

                var index = chap.IndexOf("[NP]");
                var namesOfChaptersALL = chap.Substring(0, index);
                var namesOfChapters = namesOfChaptersALL.Split('[');
                var dialogText = chap.Substring(index);
                var dialog = new Dialog(dialogText);

                for (int i = 1; i < namesOfChapters.Length; i++)
                {
                    var _index = namesOfChapters[i].IndexOf(']');
                    namesOfChapters[i] = namesOfChapters[i].Remove(_index);

                    dialogs.Add(namesOfChapters[i], dialog);
                    //Debug.Log("nameOfChapters " + i + " = " + namesOfChapters[i]);

                }

            }



        }

        public string GetText(string name)
        {
            if (texts.ContainsKey(name))
                return texts[name];
            else
                return "ERROR_TEXT";
        }

    }
}
