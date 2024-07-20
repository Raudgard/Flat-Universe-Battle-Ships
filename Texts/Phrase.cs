using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Texts
{
    public class Phrase
    {
        public Character WhoTold { get; set; }
        public string Text { get; set; }

        public Phrase(Character character, string text)
        {
            WhoTold = character;
            Text = text;
        }
    }
}