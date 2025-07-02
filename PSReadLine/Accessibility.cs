/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/

using System.Runtime.InteropServices;

namespace Microsoft.PowerShell.Internal
{
    internal class Accessibility
    {
        internal static bool IsScreenReaderActive()
        {
            bool returnValue = false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // NOTE: This API can detect if a third-party screen reader is active, such as NVDA, but not the in-box Windows Narrator.
                PlatformWindows.SystemParametersInfo(PlatformWindows.SPI_GETSCREENREADER, 0, ref returnValue, 0);
            } // TODO: Support other platforms per https://code.visualstudio.com/docs/configure/accessibility/accessibility

            return returnValue;
        }
    }
}
