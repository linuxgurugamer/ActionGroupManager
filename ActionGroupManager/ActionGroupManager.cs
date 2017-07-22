/*The MIT License (MIT)

ManeuverGameParams - In game settings options

Copyright (c) 2017 Kevin Seiden

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using UnityEngine;

namespace ActionGroupManager
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class ActionGroupManager : MonoBehaviour
    {
        //public static CareerParameters careerSettings;
        public static Settings settings;
        public const string ModPath = "AquilaEnterprises/ActionGroupManager/";

        public void Awake()
        {
            Debug.Log("<color=#800000ff>[Action Group Manager]</color> is Awake.");
            if (HighLogic.CurrentGame != null)
            {
                Debug.Log("<color=#800000ff>[Action Group Manager]</color> Current Game Located.");
                //careerSettings = HighLogic.CurrentGame.Parameters.CustomParams<CareerParameters>();
                settings = HighLogic.CurrentGame.Parameters.CustomParams<Settings>();
                AddDebugLog("Game Settings Loaded.");
            }
            GameEvents.OnGameSettingsApplied.Add(OnSettingsApplied);
        }

        public void OnSettingsApplied()
        {
            //careerSettings = HighLogic.CurrentGame.Parameters.CustomParams<CareerParameters>();
            settings = HighLogic.CurrentGame.Parameters.CustomParams<Settings>();
        }

        static public void AddDebugLog(string message, bool verbose = false)
        {
            if(settings != null && settings.basicLogging && (!verbose || settings.verboseLogging))
                Debug.Log("<color=#800000ff>[Action Group Manager]</color> " + message);
        }
    }
}
