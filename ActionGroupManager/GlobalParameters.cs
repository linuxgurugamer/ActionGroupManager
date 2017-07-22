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

using KSP.Localization;

namespace ActionGroupManager
{
    public class Settings : GameParameters.CustomParameterNode
    {
        // #autoLOC_AGM_060 = Enable Career Mode
        // #autoLOC_AGM_114 = Action Groups may only be modified if either the VAB or SPH have been upgraded to the correct tier.
        [GameParameters.CustomParameterUI("#autoLOC_AGM_060", toolTip = "#autoLOC_AGM_114",gameMode = GameParameters.GameMode.CAREER, autoPersistance = true)]
        public bool enableCareer = true;

        // #autoLOC_AGM_066 = Basic Logging
        // #autoLOC_AGM_115 = Enable to show code event logs in the console and output file.
        [GameParameters.CustomParameterUI("#autoLOC_AGM_066", toolTip = "#autoLOC_AGM_115", autoPersistance = true)]
        public bool basicLogging = false;

        // #autoLOC_AGM_066 = Verbose Logging
        // #autoLOC_AGM_116 = Enable to show code loop logs in the console and output file. WARNING: This creates a large number of log messages!
        [GameParameters.CustomParameterUI("#autoLOC_AGM_067", toolTip = "#autoLOC_AGM_116", autoPersistance = true)]
        public bool verboseLogging = false;

        public override GameParameters.GameMode GameMode
        {
            get { return GameParameters.GameMode.ANY; }
        }

        public override string Section
        {
            get { return "AGM"; }
        }

        public override string DisplaySection
        {
            get
            {
                // #autoLOC_AGM_004 = Action Group Manager
                return Localizer.GetStringByTag("#autoLOC_AGM_004");
            }
        }

        public override int SectionOrder
        {
            get { return 1; }
        }

        public override string Title
        {
            get
            {
                // #autoLOC_AGM_002 = Settings
                return Localizer.GetStringByTag("#autoLOC_AGM_002");
            }
        }

        public override bool HasPresets
        {
            get { return false; }
        }

        public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters)
        {
            if (member.Name == "verboseLogging")
                return basicLogging;

            return true;
        }
    }
}
