namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that describes an object's properties, such as IsBlocking.
    /// </summary>
    public class ObjectProperties
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="id"></param>
        public ObjectProperties(ushort id)
        {
            this.ID = id;
        }

        /// <summary>
        /// The object's ID.
        /// </summary>
        public ushort ID { get; private set; }
        /// <summary>
        /// The set of flags that serves as properties.
        /// </summary>
        public uint Flags { get; set; }

        /// <summary>
        /// Checks whether this object has a given property or properties.
        /// </summary>
        /// <param name="flag">The property or properties to check for.</param>
        /// <returns></returns>
        public bool HasFlag(Enums.ObjectPropertiesFlags flag)
        {
            return (this.Flags & (uint)flag) == (uint)flag;
        }
        /// <summary>
        /// Adds one or more properties to this object.
        /// </summary>
        /// <param name="flag">The property or properties to add.</param>
        public void AddFlag(Enums.ObjectPropertiesFlags flag)
        {
            if (!HasFlag(flag)) this.Flags += (uint)flag;
        }
        /// <summary>
        /// Removes one or more properties from this object.
        /// </summary>
        /// <param name="flag">The property or properties to remove.</param>
        public void RemoveFlag(Enums.ObjectPropertiesFlags flag)
        {
            if (this.HasFlag(flag)) this.Flags -= (uint)flag;
        }
    }
}
