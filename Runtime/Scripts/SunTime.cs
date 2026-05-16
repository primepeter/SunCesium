/*
 *  Copyright (C) X Gemeente
 *                X Amsterdam
 *                X Economic Services Departments
 *
 *  Licensed under the EUPL, Version 1.2 or later (the "License");
 *  You may not use this work except in compliance with the License.
 *  You may obtain a copy of the License at:
 *
 *    https://github.com/Amsterdam/Netherlands3D/blob/main/LICENSE.txt
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" basis,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
 *  implied. See the License for the specific language governing
 *  permissions and limitations under the License.
 */

using CesiumForUnity;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PrimePeter.CesiumSun
{
    [ExecuteInEditMode]
    public class SunTime : MonoBehaviour
    {
        [Header("Time")] [SerializeField] private DateTimeKind dateTimeKind = DateTimeKind.Local;

        [FormerlySerializedAs("jumpToCurrentTimeAtStart")] [SerializeField]
        private bool useCurrentTime = false;

        [SerializeField] [Range(0, 23)] private int hour = 18;
        [SerializeField] [Range(0, 59)] private int minutes = 0;
        [SerializeField] [Range(0, 59)] private int seconds = 0;
        [SerializeField] [Range(1, 31)] private int day = 13;
        [SerializeField] [Range(1, 12)] private int month = 8;
        [SerializeField] [Range(1, 2050)] private int year = 2026;

        [Header("Settings")] [SerializeField] private Light sunDirectionalLight;
        [SerializeField] private bool animate = true;
        [SerializeField] private float timeSpeed = 1;
        [SerializeField] private int frameSteps = 1;

        [Header("Georeference")] 
        [SerializeField] private CesiumGeoreference cesiumGeoreference;
        [Tooltip("If true, will automatically look for CesiumGeoreference in the scene if not assigned")]
        [SerializeField] private bool autoFindCesiumGeoreference = true;

        [Header("Events")] public UnityEvent<DateTime> timeOfDayChanged = new();
        public UnityEvent<float> timeSpeedChanged = new();
        public UnityEvent<bool> useCurrentTimeChanged = new();
        public UnityEvent<bool> isAnimatingChanged = new();

        private double longitude;
        private double latitude;
        private DateTime time;
        private int frameStep;

        public DateTime Time
        {
            get => time;
            set
            {
                if (time == value)
                    return;

                time = value;
                UpdateTimeOfDayPartsFromTime();
                SetDirection();
                timeOfDayChanged.Invoke(time);
            }
        }

        public bool UseCurrentTime
        {
            get => useCurrentTime;
            set
            {
                useCurrentTime = value;
                if (value)
                    ResetToNow();

                useCurrentTimeChanged.Invoke(value);
            }
        }

        public bool IsAnimating => animate;

        private const int gizmoRayLength = 10000;

        private void OnEnable()
        {
            // Ensure time is correctly set when enabled
            EnsureTimeInitialized();
        }

        private void Start()
        {
            InitializeCesiumGeoreference();
            
            if (useCurrentTime)
            {
                ResetToNow();
            }
            else
            {
                EnsureTimeInitialized();
            }

            RecalculateOrigin();
        }

        private void EnsureTimeInitialized()
        {
            if (time == default(DateTime))
            {
                time = new DateTime(year, month, day, hour, minutes, seconds, dateTimeKind);
                SetDirection();
            }
        }

        private void SyncTimeFromInspector()
        {
            // Overwrite the internal time variable with inspector values
            time = new DateTime(year, month, day, hour, minutes, seconds, dateTimeKind);
            SetDirection();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Called in the editor when inspector values are changed
            if (!Application.isPlaying)
            {
                // Synchronize the time with the inspector values
                SyncTimeFromInspector();
                
                // Update sun position immediately in the editor
                if (cesiumGeoreference == null && autoFindCesiumGeoreference)
                {
                    InitializeCesiumGeoreference();
                }
                
                if (sunDirectionalLight != null)
                {
                    RecalculateOrigin();
                }
            }
            else
            {
                // In Play mode: update only the sun position, NOT the time variable
                // This allows using the inspector sliders without interrupting the running animation
                if (sunDirectionalLight != null && !animate)
                {
                    // Only update internal time from inspector if animation is NOT playing
                    SyncTimeFromInspector();
                }
            }
        }
#endif

        private void InitializeCesiumGeoreference()
        {
            if (cesiumGeoreference == null && autoFindCesiumGeoreference)
            {
                cesiumGeoreference = GetComponentInParent<CesiumGeoreference>(includeInactive: true)
                                     ?? FindObjectOfType<CesiumGeoreference>();
            }

            if (cesiumGeoreference == null)
                Debug.LogError("CesiumGeoreference not found! This package requires Cesium for Unity. Please add a CesiumGeoreference component to your scene.");
        }

        private void Update()
        {
            // Ensure time is initialized (important after assembly reload)
            EnsureTimeInitialized();

            if (!animate) return;

            // Update only every N-th frame (performance optimization)
            frameStep = (frameStep + 1) % frameSteps;
            if (frameStep != 0) return;

            Time = time.AddSeconds(timeSpeed * UnityEngine.Time.deltaTime * frameSteps);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (sunDirectionalLight == null) return;
            
            var position = this.transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(position, -sunDirectionalLight.transform.forward * gizmoRayLength);
        }
#endif

        public void ToggleAnimation(bool animate)
        {
            bool wasAnimating = this.animate;
            this.animate = animate;
            
            // When animation is enabled, synchronize the current time
            if (animate && !wasAnimating)
            {
                SyncTimeFromInspector();
            }
            
            isAnimatingChanged.Invoke(animate);
        }

        [Obsolete("Use the Time property instead")]
        public DateTime GetTime()
        {
            return time;
        }

        //function for in the inspector
        public void SetTime(DateTime time)
        {
            UseCurrentTime = false;
            Time = time;
        }

        public void SetTime(int hour, int minute, int second)
        {
            UseCurrentTime = false;

            hour = Mathf.Clamp(hour, 0, 23);
            minute = Mathf.Clamp(minute, 0, 59);
            second = Mathf.Clamp(second, 0, 59);

            Time = new DateTime(
                Time.Year,
                Time.Month,
                Time.Day,
                hour,
                minute,
                second,
                Time.Millisecond,
                Time.Kind);
        }

        public void SetHour(int hour)
        {
            SetTime(hour, Time.Minute, Time.Second);
        }

        public void SetMinutes(int minute)
        {
            SetTime(Time.Hour, minute, Time.Second);
        }

        public void SetSeconds(int second)
        {
            SetTime(Time.Hour, Time.Minute, second);
        }

        public void SetDate(int day, int month, int year)
        {
            UseCurrentTime = false;

            year = Mathf.Clamp(year, 1, 9999);
            month = Mathf.Clamp(month, 1, 12);
            var maxDay = DateTime.DaysInMonth(year, month);
            day = Mathf.Clamp(day, 1, maxDay);

            Time = new DateTime(
                year,
                month,
                day,
                Time.Hour,
                Time.Minute,
                Time.Second,
                Time.Millisecond,
                Time.Kind);
        }

        public void SetDay(int day)
        {
            SetDate(day, Time.Month, Time.Year);
        }

        public void SetMonth(int month)
        {
            SetDate(Time.Day, month, Time.Year);
        }

        public void SetYear(int year)
        {
            SetDate(Time.Day, Time.Month, year);
        }

        public void SetLocation(double longitude, double latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;

            SetDirection();
        }

        public void SetUpdateSteps(int i)
        {
            frameSteps = i;
        }

        public void MultiplyTimeSpeed(float multiplicationFactor)
        {
            timeSpeed = Math.Clamp(timeSpeed * multiplicationFactor, 1, 43200);
            timeSpeedChanged.Invoke(timeSpeed);
        }

        public void SetTimeSpeed(float speed)
        {
            timeSpeed = Math.Clamp(speed, 1, 43200);
            timeSpeedChanged.Invoke(timeSpeed);
        }

        public void ResetToNow()
        {
            useCurrentTime = true;
            useCurrentTimeChanged.Invoke(useCurrentTime);
            Time = DateTime.Now;
        }

        private void UpdateTimeOfDayPartsFromTime()
        {
            hour = time.Hour;
            minutes = time.Minute;
            seconds = time.Second;
            day = time.Day;
            month = time.Month;
            year = time.Year;
        }

        private void UpdateLocationFromGeoreference()
        {
            if (cesiumGeoreference == null)
            {
                Debug.LogError("CesiumGeoreference is required. Please assign it or enable auto-detect.");
                return;
            }

            latitude  = cesiumGeoreference.latitude;
            longitude = cesiumGeoreference.longitude;
        }

        public void SetCesiumGeoreference(CesiumGeoreference georeference)
        {
            cesiumGeoreference = georeference;
            if (georeference == null)
            {
                Debug.LogError("CesiumGeoreference cannot be null. This package requires Cesium for Unity.");
                return;
            }
            RecalculateOrigin();
        }

        private void SetDirection()
        {
            if (sunDirectionalLight == null) return;

            Vector3 angles = new Vector3();
            // Derive UTC offset from longitude: 15 degrees = 1 hour (solar time, no political boundaries)
            // This is astronomically correct for sun position calculations.
            double solarUtcOffset = longitude / 15.0;
            var utcTime = DateTime.SpecifyKind(time.AddHours(-solarUtcOffset), DateTimeKind.Utc);
            SunPosition.CalculateSunPosition(utcTime, latitude, longitude, out double azi, out double alt);
            angles.x = (float)alt * Mathf.Rad2Deg;
            angles.y = (float)azi * Mathf.Rad2Deg;

            // Use world rotation so the light direction is correct regardless of parent transform
            sunDirectionalLight.transform.rotation = Quaternion.Euler(angles);
        }

        public void RecalculateOrigin()
        {
            UpdateLocationFromGeoreference();
            SetDirection();
        }
    }
}
