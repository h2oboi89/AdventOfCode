namespace AdventOfCode.Extensions
{
    internal static class RangeExtensions
    {
        public static bool ContainedIn(this Range self, Range other)
        {
            if (other.Start.Value <= self.Start.Value && self.End.Value <= other.End.Value)
            {
                return true;
            }

            return false;
        }

        public static bool Overlap(this Range self, Range other)
        {
            if (self.Start.Value >= other.Start.Value && self.Start.Value <= other.End.Value ||
                self.End.Value >= other.Start.Value && self.End.Value <= other.End.Value)
            {
                return true;
            }

            return false;
        }
    }
}
