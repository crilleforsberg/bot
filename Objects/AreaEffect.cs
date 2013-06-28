using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// This class is most commonly used to determine if someone or something is
    /// within range of a rune or spell.
    /// </summary>
    public class AreaEffect
    {
        /// <summary>
        /// Creatures a new instance of the AreaEffect class.
        /// </summary>
        /// <param name="area">
        /// The area of which the spell or rune effects.
        /// One byte array entry per row.
        /// </param>
        /// <param name="effectType">The type of area effect.</param>
        public AreaEffect(IEnumerable<byte[]> area, EffectType effectType)
        {
            this.Area = new List<byte[]>(area);
            this.Type = effectType;
        }

        private List<byte[]> Area { get; set; }
        /// <summary>
        /// Used the determine the point of origin.
        /// </summary>
        public EffectType Type { get; private set; }

        /// <summary>
        /// Gets the area that is effected.
        /// One entry per row.
        /// <para></para>
        /// Spells should have numbers corresponding to the direction (see Enums.Direction).
        /// Tiles not effected should have a value higher than 8.
        /// </summary>
        public IEnumerable<byte[]> GetArea() { return this.Area; }

        public enum EffectType
        {
            Spell,
            Rune
        }
    }
}
