//-----------------------------------------------------------------------
// <copyright file="IButtonBar.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    /// <summary>
    /// The interface that defines interactions for all button bar types.
    /// </summary>
    public interface IButtonBar
    {
        /// <summary>
        /// Changes the texture state.
        /// </summary>
        /// <param name="vis">Whether the texture should indicate an on or off position.</param>
        void SwitchTexture(bool vis);
    }
}
