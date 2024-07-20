using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Texts
{
    public class Choice
    {
        public int VariantFrom { get; set; }
        public int VariantTo { get; set; }
        public string Text { get; set; }

        public Choice(int variantFrom, int variantTo, string text)
        {
            VariantFrom = variantFrom;
            VariantTo = variantTo;
            Text = text;
        }
       
        
    }
}