namespace Secyud.Ugf.HexMap
{
    /// <summary>
    ///     Hexagonal direction, pointy side up, which is north.
    /// </summary>
    public enum HexDirection : sbyte
    {
        InValid = -1,
        Ne = 0,//Northeast.
        E = 1, //East.
        Se = 2,//Southeast.
        SW = 3, //Southwest.
        W = 4,//West.
        Nw = 5 //Northwest.
    }

    /// <summary>
    ///     Extension methods for <see cref="HexDirection" />.
    /// </summary>
    public static class HexDirectionExtensions
    {
        /// <summary>
        ///     Get the opposite of a hex direction.
        /// </summary>
        /// <param name="direction">A given direction.</param>
        /// <returns>The opposite direction.</returns>
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? direction + 3 : direction - 3;
        }

        /// <summary>
        ///     Get the previous direction, rotating counter-clockwise.
        /// </summary>
        /// <param name="direction">A given direction.</param>
        /// <returns>A direction rotated one step counter-clockwise.</returns>
        public static HexDirection Previous(this HexDirection direction)
        {
            return direction == HexDirection.Ne ? HexDirection.Nw : direction - 1;
        }

        /// <summary>
        ///     Get the next direction, rotating clockwise.
        /// </summary>
        /// <param name="direction">A given direction.</param>
        /// <returns>A direction rotated one step clockwise.</returns>
        public static HexDirection Next(this HexDirection direction)
        {
            return direction == HexDirection.Nw ? HexDirection.Ne : direction + 1;
        }

        /// <summary>
        ///     Get the same result as invoking <see cref="Previous" /> twice.
        /// </summary>
        /// <param name="direction">A given direction.</param>
        /// <returns>A direction rotated two steps counter-clockwise.</returns>
        public static HexDirection Previous2(this HexDirection direction)
        {
            direction -= 2;
            return direction >= HexDirection.Ne ? direction : direction + 6;
        }

        /// <summary>
        ///     Get the same result as invoking <see cref="Next" /> twice.
        /// </summary>
        /// <param name="direction">A given direction.</param>
        /// <returns>A direction rotated two steps clockwise.</returns>
        public static HexDirection Next2(this HexDirection direction)
        {
            direction += 2;
            return direction <= HexDirection.Nw ? direction : direction - 6;
        }
    }
}