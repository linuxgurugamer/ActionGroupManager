//-----------------------------------------------------------------------
// <copyright file="BaseActionManager.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a set of static methods for manipulating <see cref="BaseAction"/>.
    /// </summary>
    public static class BaseActionManager
    {
        /// <summary>
        /// Returns a list of base actions that exist for the provided part.
        /// </summary>
        /// <param name="part">The part to get base actions from.</param>
        /// <returns>The base actions available to the part.</returns>
        public static ICollection<BaseAction> FromParts(Part part)
        {
            var partList = new List<BaseAction>();
            if (part != null)
            {
                // Add BaseActions in the part
                foreach (BaseAction action in part.Actions)
                {
                    partList.Add(action);
                }

                // Add BaseActions in the part modules.
                foreach (PartModule module in part.Modules)
                {
                    foreach (BaseAction action in module.Actions)
                    {
                        partList.Add(action);
                    }
                }
            }

            return partList;
        }

        /// <summary>
        /// Returns a list of base actions that exist for the provided parts.
        /// </summary>
        /// <param name="parts">A list of part to get base actions from.</param>
        /// <returns>The base actions available to the parts.</returns>
        public static IEnumerable<BaseAction> FromParts(IEnumerable<Part> parts)
        {
            var actionList = new List<BaseAction>();

            if (parts != null)
            {
                foreach (Part part in parts)
                {
                    foreach (BaseAction action in part.Actions)
                    {
                        actionList.Add(action);
                    }

                    foreach (PartModule module in part.Modules)
                    {
                        foreach (BaseAction action in module.Actions)
                        {
                            actionList.Add(action);
                        }
                    }
                }
            }

            return actionList;
        }

        /// <summary>
        /// Returns a list of base actions that exist for the provided parts in the provided action group.
        /// </summary>
        /// <param name="parts">A list of part to get base actions from.</param>
        /// <param name="group">The action group to filter actions by.</param>
        /// <returns>The base actions available to the parts that are in the action group.</returns>
        public static ICollection<BaseAction> FromParts(IEnumerable<Part> parts, KSPActionGroup group)
        {
            IEnumerable<BaseAction> list = FromParts(parts);
            var ret = new List<BaseAction>();

            foreach (BaseAction action in list)
            {
                if (group.ContainsAction(action))
                {
                    ret.Add(action);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns a list of action groups that the provided base action belongs to.
        /// </summary>
        /// <param name="action">The base action to find actiong groups for.</param>
        /// <returns>A list of action groups the base action is assigned to.</returns>
        public static ICollection<KSPActionGroup> GetActionGroupList(BaseAction action)
        {
            var ret = new List<KSPActionGroup>();
            var groups = Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[];

            foreach (KSPActionGroup group in groups)
            {
                if (group == KSPActionGroup.None || group == KSPActionGroup.REPLACEWITHDEFAULT)
                {
                    continue;
                }

                if (group.ContainsAction(action))
                {
                    ret.Add(group);
                }
            }

            return ret;
        }
    }
}
