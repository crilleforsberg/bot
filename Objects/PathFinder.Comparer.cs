using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class PathFinder
    {
        private class NodeComparer : IComparer<Node>
        {
            public int Compare(Node first, Node second)
            {
                if (first.CombinedTravelCost > second.CombinedTravelCost)
                    return 1;
                else if (first.CombinedTravelCost < second.CombinedTravelCost)
                    return -1;
                return 0;
            }
        }
    }
}
