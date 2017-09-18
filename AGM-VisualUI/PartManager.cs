//-----------------------------------------------------------------------
// <copyright file="PartManager.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a specific type of filter.
    /// </summary>
    internal enum FilterModification
    {
        /// <summary>
        /// Represents a change in the category filter.
        /// </summary>
        Category,

        /// <summary>
        /// Represents a change in the action group filter.
        /// </summary>
        ActionGroup,

        /// <summary>
        /// Represents a change in the string filter.
        /// </summary>
        Search,

        /// <summary>
        /// Represents a change in the stage filter.
        /// </summary>
        Stage,

        /// <summary>
        /// Represents a change in the part filter.
        /// </summary>
        Part,

        /// <summary>
        /// Represents a change in the action filter.
        /// </summary>
        BaseAction,

        /// <summary>
        /// Represents clearing all filters.
        /// </summary>
        All
    }

    /// <summary>
    /// Defines a class to maintain a subset of parts based on filter parameters selected by the user.
    /// </summary>
    internal class PartManager
    {
        /// <summary>
        /// Contains a list of parts that meet the current selection of filters.
        /// </summary>
        private List<Part> filteredParts = new List<Part>();

        /// <summary>
        /// Contains a dictionary of how many parts are in each <see cref="PartCategories"/>.
        /// </summary>
        private SortedList<PartCategories, int> partCounts;

        /// <summary>
        /// A value indicating that the filter has changed but the part list has not been rebuilt for that change.
        /// </summary>
        private bool dirty = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartManager"/> class.
        /// </summary>
        public PartManager()
        {
            VesselManager.Instance.DatabaseUpdated += this.Manager_DatabaseUpdated;
        }

        /// <summary>
        /// Gets the currently selected part category filter.
        /// </summary>
        public PartCategories CurrentPartCategory { get; private set; } = PartCategories.none;

        /// <summary>
        /// Gets the currently selected action group filter.
        /// </summary>
        public KSPActionGroup CurrentActionGroup { get; private set; } = KSPActionGroup.None;

        /// <summary>
        /// Gets the current text filter.
        /// </summary>
        public string CurrentSearch { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the current stage filter.
        /// </summary>
        public int CurrentStage { get; private set; } = int.MinValue;

        /*
        /// <summary>
        /// Gets the current part filter.
        /// </summary>
        private Part CurrentPart { get; set; }

        /// <summary>
        /// Gets the current part action filter.
        /// </summary>
        private BaseAction CurrentAction { get; set; }
        */

        /// <summary>
        /// Gets a list of <see cref="Part"/> matching the current filter.
        /// </summary>
        /// <returns>A list of filtered <see cref="Part"/> on the <see cref="Vessel"/>.</returns>
        public ICollection<Part> FilteredParts
        {
            get
            {
                if (this.dirty)
                {
                    this.filteredParts.Clear();

                    IEnumerable<Part> baseList = VesselManager.Instance.Parts;

                    if (this.CurrentPartCategory != PartCategories.none)
                    {
                        baseList = baseList.Where(this.FilterCategory);
                    }

                    if (this.CurrentActionGroup != KSPActionGroup.None)
                    {
                        baseList = baseList.Where(this.FilterActionGroup);
                    }

                    if (!string.IsNullOrEmpty(this.CurrentSearch))
                    {
                        baseList = baseList.Where(this.FilterString);
                    }

                    if (this.CurrentStage != int.MinValue)
                    {
                        baseList = baseList.Where(this.FilterStage);
                    }

                    this.filteredParts.AddRange(baseList);
                    this.dirty = false;
                }

                return this.filteredParts;
            }
        }

        /// <summary>
        /// Gets a dictionary containing the number of parts in each <see cref="PartCategories"/>.
        /// </summary>
        public IDictionary<PartCategories, int> PartCountByCategory
        {
            get
            {
                if (this.dirty)
                {
                    this.partCounts = new SortedList<PartCategories, int>();

                    foreach (PartCategories category in Enum.GetValues(typeof(PartCategories)) as PartCategories[])
                    {
                        this.partCounts.Add(category, 0);
                    }

                    foreach (Part part in VesselManager.Instance.Parts)
                    {
                        this.partCounts[part.partInfo.category] += 1;
                    }
                }

                return this.partCounts;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="KSPActionGroup"/> that the <see cref="Part"/> has actions assigned to.
        /// </summary>
        /// <param name="part">The <see cref="Part"/> to get the action group assignments for.</param>
        /// <returns>A collection of <see cref="KSPActionGroup"/> that the <see cref="Part"/> has actions assigned to.</returns>
        public static List<KSPActionGroup> GetActionGroupAttachedToPart(Part part)
        {
            var ret = new List<KSPActionGroup>();

            foreach (KSPActionGroup group in Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[])
            {
                if (group != KSPActionGroup.None)
                {
                    foreach (PartModule module in part.Modules)
                    {
                        foreach (BaseAction action in module.Actions)
                        {
                            if (group.ContainsAction(action) && !ret.Contains(group))
                            {
                                ret.Add(group);
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets a collection of <see cref="BaseAction"/> assigned to a <see cref="KSPActionGroup"/>
        /// </summary>
        /// <param name="group">The <see cref="KSPActionGroup"/> to get the action assignments for.</param>
        /// <returns>A collection of <see cref="BaseAction"/> assigned to the <see cref="KSPActionGroup"/>.</returns>
        public static List<BaseAction> GetBaseActionAttachedToActionGroup(KSPActionGroup group)
        {
            var ret = new List<BaseAction>();

            foreach (Part part in VesselManager.Instance.Parts)
            {
                foreach (BaseAction action in part.Actions)
                {
                    if (group.ContainsAction(action))
                    {
                        ret.Add(action);
                    }
                }

                foreach (PartModule module in part.Modules)
                {
                    foreach (BaseAction action in module.Actions)
                    {
                        if (group.ContainsAction(action))
                        {
                            ret.Add(action);
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Handles the <see cref="MainUi.FilterChanged"/> event.
        /// </summary>
        /// <param name="sender">The object calling the event.</param>
        /// <param name="args">The <see cref="FilterEventArgs"/> for the event.</param>
        public void ViewFilterChanged(object sender, FilterEventArgs args)
        {
            switch (args.Modified)
            {
                case FilterModification.Category:
                    this.CurrentPartCategory = (PartCategories)args.Object;
                    break;
                case FilterModification.ActionGroup:
                    this.CurrentActionGroup  = (KSPActionGroup)args.Object;
                    break;
                case FilterModification.Search:
                    this.CurrentSearch       = args.Object as string;
                    break;
                case FilterModification.Stage:
                    this.CurrentStage        = (int)args.Object;
                    break;
                /*
                case FilterModification.Part:
                    this.CurrentPart         = args.Object as Part;
                    break;
                case FilterModification.BaseAction:
                    this.CurrentAction       = args.Object as BaseAction;
                    break;
                */
                case FilterModification.All:
                    this.filteredParts.Clear();
                    this.CurrentPartCategory = PartCategories.none;
                    this.CurrentActionGroup  = KSPActionGroup.None;
                    this.CurrentSearch       = string.Empty;
                    this.CurrentStage        = int.MinValue;
                    break;
                default:
                    break;
            }

            this.dirty = true;
        }

        /// <summary>
        /// Handles an event that occurs when the vessel part database is rebuilt.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> calling the event.</param>
        /// <param name="e">Unused event arguments.</param>
        private void Manager_DatabaseUpdated(object sender, EventArgs e)
        {
            this.dirty = true;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Part"/> name contains the current search text.
        /// </summary>
        /// <param name="p">The <see cref="Part"/> to test.</param>
        /// <returns>True if the <see cref="Part"/> name contains the search string.</returns>
        private bool FilterString(Part p)
        {
            return string.IsNullOrEmpty(this.CurrentSearch) || p.partInfo.title.IndexOf(this.CurrentSearch, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Part"/> name is in the currently selected stage.
        /// </summary>
        /// <param name="p">The <see cref="Part"/> to test.</param>
        /// <returns>True if the <see cref="Part"/> is in the currently selected stage.</returns>
        private bool FilterStage(Part p)
        {
            return p.inverseStage == this.CurrentStage;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Part"/> is in the currently selected part category.
        /// </summary>
        /// <param name="part">The <see cref="Part"/> to test.</param>
        /// <returns>True if the <see cref="Part"/> is in the selected part category.</returns>
        private bool FilterCategory(Part part)
        {
            return part.partInfo.category == this.CurrentPartCategory;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Part"/> has any action in the currently selected action group.
        /// </summary>
        /// <param name="part">The <see cref="Part"/> to test.</param>
        /// <returns>True if the <see cref="Part"/> has an action in the selected action group.</returns>
        private bool FilterActionGroup(Part part)
        {
            foreach (BaseAction action in part.Actions)
            {
                if (this.CurrentActionGroup.ContainsAction(action))
                {
                    return true;
                }
            }

            foreach (PartModule module in part.Modules)
            {
                foreach (BaseAction action in module.Actions)
                {
                    if (this.CurrentActionGroup.ContainsAction(action))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Event arguments for the <see cref="MainUi.FilterChanged"/> event.
    /// </summary>
    internal class FilterEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating what filter was altered.
        /// </summary>
        public FilterModification Modified { get; set; }

        /// <summary>
        /// Gets or sets an object associated with the changed filter.
        /// </summary>
        public object Object { get; set; }
    }
}
