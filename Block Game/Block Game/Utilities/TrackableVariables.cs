///Represents a variable that can be tracked using event handlers
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Block_Game.Utilities
{
    /// <summary>
    /// Represents a variable which can be tracked by event handlers
    /// </summary>
    public class TrackableVariable
    {
        object val; //private value acsessor
        /// <summary>
        /// Gets or sets the object value of this variable
        /// </summary>
        public object Value
        {
            get { return val; }
            set
            {
                val = value;
                if (valueChanged != null)
                    valueChanged.Invoke(new ObjectiveEventArgs(val));
            }
        }

        /// <summary>
        /// The event handler that is evoked when the value changes
        /// </summary>
        public ObjectiveEventHandler valueChanged;
    }

    /// <summary>
    /// Represents an event handler specialized for variable changes
    /// </summary>
    /// <param name="e"></param>
    public delegate void ObjectiveEventHandler(ObjectiveEventArgs e);

    /// <summary>
    /// Represents the event arguments when a trackable variable value changes
    /// </summary>
    public class ObjectiveEventArgs : EventArgs
    {
        /// <summary>
        /// The new value
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Creates a new ObjectiveEventArgs
        /// </summary>
        /// <param name="value">The new value of the trackable variables' value</param>
        public ObjectiveEventArgs(object value)
        {
            this.Value = value;
        }
    }
}
