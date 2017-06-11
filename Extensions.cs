//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System;
using System.Collections.Generic;

namespace ActionGroupManager
{
    static class Extensions
    {
        static Dictionary<KSPActionGroup, string> dic = new Dictionary<KSPActionGroup, string>()
        {
            {KSPActionGroup.None, "None"},
            {KSPActionGroup.Stage, "St"},
            {KSPActionGroup.Gear, "Ge"},
            {KSPActionGroup.Light, "Li"},
            {KSPActionGroup.RCS, "RC"},
            {KSPActionGroup.SAS, "SA"},
            {KSPActionGroup.Brakes, "Br"},
            {KSPActionGroup.Abort, "Ab"},
            {KSPActionGroup.Custom01, "01"},
            {KSPActionGroup.Custom02, "02"},
            {KSPActionGroup.Custom03, "03"},
            {KSPActionGroup.Custom04, "04"},
            {KSPActionGroup.Custom05, "05"},
            {KSPActionGroup.Custom06, "06"},
            {KSPActionGroup.Custom07, "07"},
            {KSPActionGroup.Custom08, "08"},
            {KSPActionGroup.Custom09, "09"},
            {KSPActionGroup.Custom10, "10"},
        };

        public static string ToShortString(this KSPActionGroup ag)
        {
            return dic[ag];
        }

        public static bool IsInActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            return bA == null ? false : (bA.actionGroup & aG) == aG;
        }

        public static void AddActionToAnActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            if ((bA.actionGroup & aG) == aG)
                return;

            bA.actionGroup |= aG;
        }

        public static void RemoveActionToAnActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            if ((bA.actionGroup & aG) != aG)
                return;

            bA.actionGroup ^= aG;
        }
    }

    enum FilterModification
    {
        Category,
        ActionGroup,
        Search,
        Stage,
        Part,
        BaseAction,
        All
    };

    class FilterEventArgs : EventArgs
    {
        public FilterModification Modified { get; set; }

        public object Object { get; set; }
    }

}
