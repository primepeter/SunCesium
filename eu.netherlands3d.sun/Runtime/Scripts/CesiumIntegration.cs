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

using UnityEngine;
using Netherlands3D.Rendering;

namespace Netherlands3D.Sun
{
    /// <summary>
    /// Helper script to integrate the Netherlands3D Sun package with Cesium for Unity.
    /// Automatically connects SunTime and DynamicShadowDistance components to CesiumGeoreference.
    /// </summary>
    [ExecuteInEditMode]
    public class CesiumIntegration : MonoBehaviour
    {
        [SerializeField] private SunTime sunTime;
        [SerializeField] private DynamicShadowDistance dynamicShadowDistance;
        [SerializeField] private Component cesiumGeoreference;
        [Tooltip("Auto-detect Cesium components on Start")]
        [SerializeField] private bool autoDetect = true;

        private void Start()
        {
            if (autoDetect)
            {
                AutoDetectCesiumComponents();
            }

            ApplyIntegration();
        }

        private void AutoDetectCesiumComponents()
        {
            // Find CesiumGeoreference
            if (cesiumGeoreference == null)
            {
                var georeference = FindObjectOfType(System.Type.GetType("CesiumForUnity.CesiumGeoreference"));
                if (georeference != null)
                {
                    cesiumGeoreference = georeference as Component;
                }
            }

            // Find SunTime if not assigned
            if (sunTime == null)
            {
                sunTime = FindObjectOfType<SunTime>();
            }

            // Find DynamicShadowDistance if not assigned
            if (dynamicShadowDistance == null)
            {
                dynamicShadowDistance = FindObjectOfType<DynamicShadowDistance>();
            }
        }

        public void ApplyIntegration()
        {
            if (cesiumGeoreference == null)
            {
                Debug.LogWarning("CesiumGeoreference not found. Please ensure Cesium for Unity is installed and CesiumGeoreference component is in the scene.");
                return;
            }

            // Connect SunTime to CesiumGeoreference
            if (sunTime != null)
            {
                sunTime.SetCesiumGeoreference(cesiumGeoreference);
                Debug.Log("Successfully integrated SunTime with CesiumGeoreference");
            }

            // Update shadow distance reference if needed
            if (dynamicShadowDistance != null)
            {
                Debug.Log("DynamicShadowDistance is active and will dynamically adjust based on camera height");
            }
        }

        /// <summary>
        /// Manually integrate with a specific CesiumGeoreference instance
        /// </summary>
        public void IntegrateWithGeoreference(Component georeference)
        {
            cesiumGeoreference = georeference;
            ApplyIntegration();
        }
    }
}
