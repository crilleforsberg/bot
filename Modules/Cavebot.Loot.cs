namespace KarelazisBot.Modules
{
    public partial class Cavebot
    {
        /// <summary>
        /// A class that wraps some relevant values for looting, i.e. item ID and item weight.
        /// </summary>
        public class Loot
        {
            #region constructors
            /// <summary>
            /// Empty constructor for this class.
            /// </summary>
            public Loot() { }
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="id">The item ID.</param>
            /// <param name="name">The name of the item.</param>
            /// <param name="cap">The minimum capacity the player must have to loot this item.</param>
            /// <param name="destination">The destination of the item.</param>
            /// <param name="containerIndex">The minimum container number. Only applicable if destination is a container.</param>
            public Loot(ushort id, string name, ushort cap, Destinations destination, byte containerIndex = 0)
            {
                this.ID = id;
                this.Name = name;
                this.Cap = cap;
                this.Destination = destination;
                this.Index = containerIndex;
            }
            #endregion constructors

            public enum Destinations
            {
                Ground,
                EmptyContainer
            }

            #region properties
            /// <summary>
            /// Gets or sets the item ID.
            /// </summary>
            public ushort ID { get; set; }
            /// <summary>
            /// Gets or sets the name of this item.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the minimum capacity the player needs to have to loot this item.
            /// </summary>
            public uint Cap { get; set; }
            /// <summary>
            /// Gets or sets the destination.
            /// </summary>
            public Destinations Destination { get; set; }
            /// <summary>
            /// Gets or sets the minumum container number for Destination.
            /// </summary>
            public byte Index { get; set; }
            #endregion
        }
    }
}
