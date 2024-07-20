using System.Collections;
using System.Collections.Generic;
using System;

namespace Texts
{
    public class Dialog
    {
        public bool IsContainsChoice { get; set; }
        private List<Phrase> Phrases { get; set; }
        private List<Choice> Choices { get; set; }

        private int currentPhrase = 0;


        public Dialog(string text)
        {
            Phrases = new List<Phrase>();
            Choices = new List<Choice>();
            IsContainsChoice = false;
            ParseText(text);
        }

        public Phrase NextPhrase()
        {
            currentPhrase++;
            if (currentPhrase - 1 < Phrases.Count)
                return Phrases[currentPhrase - 1];
            else
            {
                ResetPhrasesCounter();
                return null;
            }
        }

        public void ResetPhrasesCounter()
        {
            currentPhrase = 0;
        }


        private void ParseText(string text)
        {
            var phrases = text.Split(new[] { "[NP]" }, StringSplitOptions.RemoveEmptyEntries);
            string[] namesOfCharacters = new string[phrases.Length];

            for (int i = 0; i < phrases.Length; i++)
            {
                var _ind = phrases[i].IndexOf(']');
                namesOfCharacters[i] = phrases[i].Substring(1, _ind - 1);
                phrases[i] = phrases[i].Substring(_ind + 1);



                if (phrases[i].Contains("[Choice]"))
                {
                    var index = phrases[i].IndexOf("[Choice]");
                    phrases[i] = phrases[i].Substring(0, index);

                    IsContainsChoice = true;
                }

                //Debug.Log("name of character: " + namesOfCharacters[i] + "     phrase:  " + phrases[i]);

                Phrases.Add(new Phrase(Characters.GetCharacters(namesOfCharacters[i]), phrases[i]));


            }

        }

    }
}