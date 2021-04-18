using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PII;

namespace PII.Utilities
{
    [Serializable]
    public class HeadBob
    {
        public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                            new Keyframe(2f, 0f)); // sin curve for head bob
        public float BobDuration;
        public float BobAmount;

        private float cyclePositionX;
        private float cyclePositionY;
        private float offset;

        public float CycleOffset { get { return offset; } }
        
        public Vector2 GetOffset(float speed)
        {
            var time = Bobcurve[Bobcurve.length - 1].time;
            float x = Bobcurve.Evaluate(cyclePositionX);
            float y = Bobcurve.Evaluate(cyclePositionY);

            cyclePositionX += (speed * Time.deltaTime);
            cyclePositionY += (speed * Time.deltaTime);

            if (cyclePositionX > time)
            {
                cyclePositionX -= time;
            }

            if (cyclePositionY > time)
            {
                cyclePositionY -= time;
            }

            return new Vector2(x,y + offset);
        }

        public IEnumerator DoBobCycle()
        {
            // make the camera move down slightly
            float t = 0f;
            while (t < BobDuration)
            {
                offset = Mathf.Lerp(0f, BobAmount, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            // make it move back to neutral
            t = 0f;
            while (t < BobDuration)
            {
                offset = Mathf.Lerp(BobAmount, 0f, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            offset = 0f;
        }
    }
}