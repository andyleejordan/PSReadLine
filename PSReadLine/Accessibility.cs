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
            // TODO: Support other platforms per https://code.visualstudio.com/docs/configure/accessibility/accessibility
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return false;

            // The supposedly official way to check for a screen reader on
            // Windows is SystemParametersInfo(SPI_GETSCREENREADER, ...) but it
            // doesn't detect the in-box Windows Narrator and is otherwise known
            // to be problematic.
            //
            // The following is adapted from the Electron project, under the MIT License.
            // Hence this is also how VS Code detects screenreaders.
            // See: https://github.com/electron/electron/pull/39988

            // Check for Windows Narrator using the NarratorRunning mutex
            if (PlatformWindows.IsMutexPresent("NarratorRunning"))
                return true;

            // Check for various screen reader libraries
            string[] screenReaderLibraries = {
                // NVDA
                "nvdaHelperRemote.dll",
                // JAWS
                "jhook.dll",
                // Window-Eyes
                "gwhk64.dll", 
                "gwmhook.dll",
                // ZoomText
                "AiSquared.Infuser.HookLib.dll"
            };

            foreach (string library in screenReaderLibraries)
            {
                if (PlatformWindows.IsLibraryLoaded(library))
                    return true;
            }

            return false;
        }
    }
}
