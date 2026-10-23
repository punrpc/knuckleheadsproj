using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace Ragdoll
{
	// Editor-only registration, stripped from the player build; values from the shipped
	// "QuestRefreshRate Android" feature asset.
#if UNITY_EDITOR
	[UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "Quest Display Refresh Rate", BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android }, Company = "Ragdoll", OpenxrExtensionStrings = "XR_FB_display_refresh_rate", Version = "1", FeatureId = "com.ragdoll.questrefreshrate")]
#endif
	public class QuestRefreshRate : OpenXRFeature
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int GetProcAddr(ulong instance, string name, out IntPtr fn);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int RequestRate(ulong session, float rate);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int GetRate(ulong session, out float rate);

		[Tooltip("Hz. Quest 2/3 accept 72, 80, 90 and 120.")]
		public float refreshRate = 90f;

		private ulong instance;

		protected override bool OnInstanceCreate(ulong xrInstance)
		{
			instance = xrInstance;
			if (!OpenXRRuntime.IsExtensionEnabled("XR_FB_display_refresh_rate"))
			{
				Debug.LogWarning("QuestRefreshRate: the runtime did not grant XR_FB_display_refresh_rate; staying at the default rate");
			}
			return true;
		}

		protected override void OnSessionBegin(ulong xrSession)
		{
			if (!OpenXRRuntime.IsExtensionEnabled("XR_FB_display_refresh_rate"))
			{
				return;
			}
			try
			{
				GetProcAddr getProc = Marshal.GetDelegateForFunctionPointer<GetProcAddr>(xrGetInstanceProcAddr);
				int found = getProc(instance, "xrRequestDisplayRefreshRateFB", out IntPtr requestFn);
				int foundGet = getProc(instance, "xrGetDisplayRefreshRateFB", out IntPtr getFn);
				if (found != 0 || requestFn == IntPtr.Zero)
				{
					Debug.LogWarning("QuestRefreshRate: xrRequestDisplayRefreshRateFB not found (" + found + ")");
					return;
				}
				int result = Marshal.GetDelegateForFunctionPointer<RequestRate>(requestFn)(xrSession, refreshRate);
				float now = -1f;
				if (foundGet == 0 && getFn != IntPtr.Zero)
				{
					Marshal.GetDelegateForFunctionPointer<GetRate>(getFn)(xrSession, out now);
				}
				Debug.Log("QuestRefreshRate: asked for " + refreshRate + " Hz, result " + result + ((now > 0f) ? (", display now " + now + " Hz") : ""));
			}
			catch (Exception ex)
			{
				Debug.LogWarning("QuestRefreshRate: " + ex.Message);
			}
		}
	}
}
