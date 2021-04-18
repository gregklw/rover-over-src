using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII.Utilities
{
    public class Environment : MonoBehaviour
    {
        public const float MaxHour = 24.9f;
        public const float MinHour = 0;
        public const float RealTimeRate = 0.00027778f;

        [SerializeField] private Camera MainCamera;
        [SerializeField] private float TimeRate = RealTimeRate;
        [SerializeField] [Range(MinHour, MaxHour)] private float SunRiseTime = 6;
        [SerializeField] [Range(MinHour, MaxHour)] private float SunSetTime = 18;
        [SerializeField] [Range(MinHour, MaxHour)] private float TimeOfDay = 8;
        [SerializeField] private Light SunLight;
        [SerializeField] private Light MoonLight;

        private static Environment instance;

        public bool isDayTime { get { return (TimeOfDay >= SunRiseTime && TimeOfDay <= SunSetTime); } }
        public float rate { get { return TimeRate; } }
        public float time { get { return TimeOfDay; } }
        public int hour { get { return (int)TimeOfDay; } }
        public int minute { get { return (int)((TimeOfDay - hour) * 60); } }

        public string timeHHMM { get { return Format(hour) + ":" + Format(minute); } }
        
        public static Environment Instance { get { return instance; } }

        private void Awake()
        {
            if (instance)
                DestroyImmediate(this);
            else
                instance = this;
        }

        private void Start()
        {

        }

        private void LateUpdate()
        {
            PassTime();
            SetLights();
            SetRenderSettings();
            
            if (MainCamera)
            {
                MainCamera.gameObject.SetActive(!GameManager.User);
            }
        }

        private void PassTime()
        {
            TimeOfDay += TimeRate * Time.deltaTime;

            if (TimeOfDay > MaxHour)
                TimeOfDay = 0;

            TimeOfDay = Mathf.Clamp(TimeOfDay, MinHour, MaxHour);
        }

        private void SetLights()
        {
            var angle = Utility.Interpolate(0, 180, SunRiseTime, SunSetTime, TimeOfDay);

            if (SunLight)
            {
                SunLight.transform.position = Vector3.zero;
                SunLight.enabled = isDayTime;
                SunLight.transform.rotation = Quaternion.Euler(angle, 0, 0);
            }

            angle -= 180;

            if (MoonLight)
            {
                MoonLight.transform.position = Vector3.zero;
                MoonLight.enabled = !isDayTime;
                MoonLight.transform.rotation = Quaternion.Euler(angle, 0, 0);
            }
        }

        private void SetRenderSettings()
        {
            var intensity = 0f;

            if (isDayTime)
            {
                var time = SunRiseTime + ((SunSetTime - SunRiseTime) * 0.5f);

                if (TimeOfDay <= time)
                {
                    intensity = Utility.Interpolate(0, 1, SunRiseTime, time, TimeOfDay);
                }
                else
                {
                    intensity = Utility.Interpolate(1, 0, time, SunSetTime, TimeOfDay);
                }
            }

            RenderSettings.reflectionIntensity = intensity;
        }

        private string Format(int number)
        {
            var text = "";

            if (number < 10)
                text += "0";

            text += number;

            return text;
        }
    }
}