//-----------------------------------------------------------------------
// <copyright file="PartCategoryExt.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using UnityEngine;

    /// <summary>
    /// Defines an extension class for <see cref="PartCategories"/>.
    /// </summary>
    public static class PartCategoryExtensions
    {
        /// <summary>
        /// Returns the texture associated with this <see cref="PartCategories"/>.
        /// </summary>
        /// <param name="category">The part category to get the texture for.</param>
        /// <returns>The texture associated with the part category.</returns>
        public static Texture GetTexture(this PartCategories category)
        {
            return GameDatabase.Instance.GetTexture(Program.ModPath + "Resources/" + category.ToString(), false);
        }
    }
}
