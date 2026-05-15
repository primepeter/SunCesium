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
using UnityEngine.Rendering.Universal;

namespace PrimePeter.CesiumSun
{
    /// <summary>
    /// Dynamically adjusts shadow distance based on camera height to ensure proper shadow rendering
    /// at both low and high altitudes. Compatible with both standard and Cesium scenes.
    /// </summary>
    public class DynamicShadowDistance : MonoBehaviour
    {
        UniversalRenderPipelineAsset universalRenderPipelineAsset;

        [SerializeField]
        private float range = 6.5f;

        [SerializeField]
        private float minShadowDistance = 100;
		[SerializeField]
		private float maxShadowDistance = 4000;

		[Tooltip("Will default to main camera transform")]
		[SerializeField]
		private Transform referenceTransform;

		[Tooltip("Enable to use camera height relative to terrain/Cesium globe")]
		[SerializeField]
		private bool useCameraSeparationFromGround = false;

		[SerializeField]
		private float groundLevelOffset = 0f;

		private void Awake()
		{
			if (!referenceTransform) referenceTransform = Camera.main.transform;
		}

		void Update()
		{
			SetShadowDistanceOnCurrentRenderPipeline();
		}

		private void OnDisable()
		{
			// Restore default shadow distance on disable
			SetShadowDistanceOnCurrentRenderPipeline();
		}

		/// <summary>
		/// Calculates shadow distance based on transform height.
		/// This allows drawing shadows even when the camera is far above the world, and still have sharp shadows at close range.
		/// Accounts for Cesium globe rendering and camera altitude.
		/// </summary>
		private void SetShadowDistanceOnCurrentRenderPipeline()
		{
			if (!referenceTransform) return;

			float heightReference = referenceTransform.position.y;

			// If using camera separation from ground, clamp to minimum to avoid excessive shadow distances at very high altitudes
			if (useCameraSeparationFromGround)
			{
				heightReference = Mathf.Max(heightReference - groundLevelOffset, 1f);
			}

			var dynamicShadowDistance = Mathf.Clamp(heightReference * range, minShadowDistance, maxShadowDistance);
			ApplyMaxShadowDistance(dynamicShadowDistance);
		}
		
		/// <summary>
		/// Sets max shadow distance in quality settings and active Render Pipeline asset if available.
		/// Ensures consistency between quality settings and URP settings to prevent conflicts.
		/// </summary>
		/// <param name="dynamicShadowDistance">Maximum shadow distance</param>
		private void ApplyMaxShadowDistance(float dynamicShadowDistance)
		{
			// Apply to quality settings
			QualitySettings.shadowDistance = dynamicShadowDistance;

			// Apply to URP asset if available
			if (!QualitySettings.renderPipeline)
			{
				return;
			}

			if (universalRenderPipelineAsset != QualitySettings.renderPipeline)
				universalRenderPipelineAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;

			if (universalRenderPipelineAsset != null)
				universalRenderPipelineAsset.shadowDistance = dynamicShadowDistance;
		}

		/// <summary>
		/// Manually set the ground level offset for height calculations.
		/// Useful for Cesium scenes where you need to account for ellipsoid height.
		/// </summary>
		public void SetGroundLevelOffset(float offset)
		{
			groundLevelOffset = offset;
		}

		/// <summary>
		/// Get the current calculated shadow distance
		/// </summary>
		public float GetCurrentShadowDistance()
		{
			return QualitySettings.shadowDistance;
		}
	}
}