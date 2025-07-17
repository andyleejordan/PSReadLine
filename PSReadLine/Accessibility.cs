/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.PowerShell.Internal
{
    internal class Accessibility
    {
        internal static bool IsScreenReaderActive()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return IsAnyWindowsScreenReaderEnabled();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return IsVoiceOverEnabled();
            }

            // TODO: Support Linux per https://code.visualstudio.com/docs/configure/accessibility/accessibility
                return false;
        }

        private static bool IsAnyWindowsScreenReaderEnabled()
        {
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

        private static bool IsVoiceOverEnabled()
        {
            try
            {
                // Use the 'defaults' command to check if VoiceOver is enabled
                // This checks the com.apple.universalaccess preference for voiceOverOnOffKey
                ProcessStartInfo startInfo = new()
                {
                    FileName = "defaults",
                    Arguments = "read com.apple.universalaccess voiceOverOnOffKey",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using Process process = Process.Start(startInfo);
                process.WaitForExit(250);
                if (process.HasExited && process.ExitCode == 0)
                {
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    // VoiceOver is enabled if the value is 1
                    return output == "1";
                }
            }
            catch
            {
                // If we can't determine the status, assume VoiceOver is not enabled
            }

            return false;
        }
    }
}
