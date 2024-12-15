using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameAssets.Scripts.Utils.DeepLinking
{
    public static class DeepLinkChecker
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern IntPtr GetDeepLink();
#endif

        public static string GetDeepLinkArgument()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            IntPtr strPtr = GetDeepLink();
            return Marshal.PtrToStringUTF8(strPtr);
#else
            return "";
#endif
        }

        public static bool HasDeepLinkArgument()
        {
            return !string.IsNullOrEmpty(GetDeepLinkArgument());
        }
    }
}