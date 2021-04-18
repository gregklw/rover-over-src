using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIAnimation
{
    [System.Serializable]
    public class TextAnimation
    {
        [SerializeField] private Text Output;
        [SerializeField] private int Rate = 5;
        [SerializeField] [TextArea(5, 10)] private string Text;

        public bool Playing { get; private set; }

        public IEnumerator Play()
        {
            Playing = true;

            if (Output) Output.text = "";

            var timer = 0f;
            var index = 0;

            while (index < Text.Length)
            {
                timer += Time.deltaTime;

                if (timer >= (1 / (float)Rate))
                {
                    if (Output) Output.text += Text[index];
                    index++;
                    timer = 0;
                }

                yield return new WaitForEndOfFrame();
            }

            Playing = false;
        }
    }
}