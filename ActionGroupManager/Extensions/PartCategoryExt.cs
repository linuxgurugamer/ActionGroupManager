using UnityEngine;

namespace ActionGroupManager.Extensions
{
    static public class PartCategoryExt
    {
        /// <summary>
        /// Returns the texture associated with this part category
        /// </summary>
        /// <param name="c">The part category to get the texture for.</param>
        /// <returns>The texture associated with the part category.</returns>
        public static Texture GetTexture(this PartCategories c)
        {
            return GameDatabase.Instance.GetTexture(global::ActionGroupManager.ActionGroupManager.ModPath + "Resources/" + c.ToString(), false);
        }
    }
}
