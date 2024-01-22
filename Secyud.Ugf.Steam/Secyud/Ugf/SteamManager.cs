using System;
using Secyud.Ugf.Logging;
using Steamworks;
using UnityEngine;

//
// The SteamManager provides a base implementation of Steamworks.NET on which you can build upon.
// It handles the basics of starting up and shutting down the SteamAPI for use.
//
namespace Secyud.Ugf
{
    public class SteamManager : MonoBehaviour
    {
        private static SteamManager _instance;

        public static SteamManager Instance => _instance ??=
            UgfGameManager.Instance.gameObject.AddComponent<SteamManager>();

        [AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
        private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
        {
            UgfLogger.LogWarning(pchDebugText);
        }

#if UNITY_2019_3_OR_NEWER
        // In case of disabled Domain Reload, reset static members before entering Play Mode.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitOnPlayMode()
        {
        }
#endif
        public void Awake()
        {
            if (!Packsize.Test())
            {
                UgfLogger.LogError(
                    "[Steamworks.NET] Packsize Test returned false, " +
                    "the wrong version of Steamworks.NET is being run " +
                    "in this platform.");
            }

            if (!DllCheck.Test())
            {
                UgfLogger.LogError(
                    "[Steamworks.NET] DllCheck Test returned false, " +
                    "One or more of the Steamworks binaries seems to " +
                    "be the wrong version.");
            }

            try
            {
                // If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
                // Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.
                // Note that this will run which ever version you have installed in steam. Which may not be the precise executable
                // we were currently running.

                // Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
                // remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
                // See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
                if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
                {
                    UgfLogger.Log(
                        "[Steamworks.NET] Shutting down because " +
                        "RestartAppIfNecessary returned true. Steam will " +
                        "restart the application.");

                    Application.Quit();
                    return;
                }
            }
            catch (DllNotFoundException e)
            {
                // We catch this exception here, as it will be the first occurrence of it.
                UgfLogger.LogError(
                    "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. " +
                    "It's likely not in the correct location. " +
                    "Refer to the README for more details.\n" +
                    e);

                Application.Quit();
                return;
            }

            // Initializes the Steamworks API.
            // If this returns false then this indicates one of the following conditions:
            // [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
            // [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
            // [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
            // [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
            // [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
            // Valve's documentation for this is located here:
            // https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
            if (!SteamAPI.Init())
            {
                UgfLogger.LogError(
                    "[Steamworks.NET] SteamAPI_Init() failed. " +
                    "Refer to Valve's documentation or the comment " +
                    "above this line for more information.");

                return;
            }

            // Set up our callback to receive warning messages from Steam.
            // You must launch with "-debug_steamapi" in the launch args to receive warnings.
            SteamClient.SetWarningMessageHook(SteamAPIDebugTextHook);
        }

        private void Update()
        {
            SteamAPI.RunCallbacks();
        }

        public void OnDestroy()
        {
            SteamAPI.Shutdown();
        }
    }
}