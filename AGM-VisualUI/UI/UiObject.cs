//-----------------------------------------------------------------------
// <copyright file="UiObject.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    /// <summary>
    /// The base class that defines the interactions for all interface objects.
    /// </summary>
    public abstract class UiObject
    {
        /// <summary>
        /// Indicates whether the object is visible to the user.
        /// </summary>
        private bool visible;

        /// <summary>
        /// Gets or sets a value indicating whether the object is visible to the user.
        /// </summary>
        public virtual bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                if (value && !this.visible)
                {
                    VisualUi.AddToPostDrawQueue(this.Paint);
                }
                else if (!value & this.visible)
                {
                    VisualUi.AddToPostDrawQueue(this.Paint);
                }

                this.visible = value;
            }
        }

        /// <summary>
        /// Disposes of the UI object.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Performs UI actions per frame.
        /// </summary>
        public abstract void Paint();
    }
}
