using System.Runtime.InteropServices;
using UnityEngine.Scripting;

[Preserve]
public static class WebGLCopyAndPasteAPI
{
    [DllImport("__Internal")]
    private static extern void CopyToClipboard(string text);

    public static void CopyText(string text)
    {
        CopyToClipboard(text);
    }
}