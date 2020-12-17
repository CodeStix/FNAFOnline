using Stx.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Stx.Net.RoomBased
{
    public class MatchmakingQueryResult : IByteDefined<MatchmakingQueryResult>
    {
        public MatchmakingQueryResult()
        { }

        /// <summary>
        /// Ordered by <see cref="GameState"/> and player count.
        /// </summary>
        [DoNotSerialize]
        public IOrderedEnumerable<Room> Ordered
        {
            get
            {
                return Rooms
                    .OrderBy((r) => r.State)
                    .ThenByDescending((r) => r.PlayerCount);
            }
        }

        /// <summary>
        /// The most suitable result from the query. Only unlocked, not full rooms are returned.
        /// Ordered by <see cref="GameState"/> and player count.
        /// Returns null if no suitable room was found.
        /// </summary>
        [DoNotSerialize]
        public Room MostSuitableResult
        {
            get
            {
                if (Rooms.Count == 0)
                    return null;

                return Ordered.Where((r) => !r.IsFull && !r.Locked).FirstOrDefault();
            }
        }

        /// <summary>
        /// The actual amount of rooms that matched the query on the server. Probably not equal to object count of <see cref="Rooms"/> due to <see cref="MatchmakingQuery.ResultsPerPage"/>.
        /// </summary>
        public int MatchedRooms { get; set; }
        /// <summary>
        /// The total amount of rooms found on the server.
        /// </summary>
        public int TotalRooms { get; set; }

        public BList<Room> Rooms { get; set; } = new BList<Room>();

        public override string ToString()
        {
            List<string> content = new List<string>();

            foreach (Room r in Rooms)
                content.Add(r.ToString());
            content.Add($"Matches: { MatchedRooms }");
            content.Add($"Total: { TotalRooms }");

            return $"({ string.Join(", ", content) })";
        }
    }
}
