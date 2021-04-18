using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIAnimation
{
    [System.Serializable]
    public class SpriteAnimation
    {
        [SerializeField] private Image Image;
        [SerializeField] private Sprite[] Frames;
        [SerializeField] private int FrameRate = 60;
        
        private int index = 0;
        private float timer = 0;

        public void Play()
        {
            if (Frames.Length <= 0) return;

            timer += Time.deltaTime;
            if (timer > (1 / (float)FrameRate))
            {
                index = index < Frames.Length - 1 ? index + 1 : 0;
                timer = 0;
            }

            if (Image) Image.sprite = Frames[index];
        }

        public void Reset()
        {
            index = 0;
            timer = 0;
        }

        public void SetImageAlpha(float a)
        {
            if (!Image) return;

            var color = Image.color;
            color.a = a;
            Image.color = color;
        }
    }
}