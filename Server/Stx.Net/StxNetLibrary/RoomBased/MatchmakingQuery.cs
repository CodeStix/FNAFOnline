using System;
using System.Collections.Generic;
using System.Text;
using Stx.Utilities;
using Stx.Serialization;

namespace Stx.Net.RoomBased
{
    public class MatchmakingQuery : IBytifiable<MatchmakingQuery>
    {
        public MatchmakingQuery()
        { }

        public MatchmakingQuery(string roomCode)
        {
            RequiredRoomCode = roomCode;
        }

        [DoNotSerialize]
        public static MatchmakingQuery MatchAll { get; set; } = new MatchmakingQuery();
        [DoNotSerialize]
        public static MatchmakingQuery Default { get; set; } = new MatchmakingQuery();
        [DoNotSerialize]
        public static MatchmakingQuery NotInGame { get; } = new MatchmakingQuery()
        {
            GameState = RoomBased.GameState.InLobby
        };
        [DoNotSerialize]
        public static MatchmakingQuery NotInGameNotFull { get; } = new MatchmakingQuery()
        {
            OnlyNotFull = true,
            GameState = RoomBased.GameState.InLobby
        };
        [DoNotSerialize]
        public static MatchmakingQuery NotInGameNotLocked { get; } = new MatchmakingQuery()
        {
            OnlyUnlocked = true,
            GameState = RoomBased.GameState.InLobby
        };
        [DoNotSerialize]
        public static MatchmakingQuery NotInGameNotLockedNotFull { get; } = new MatchmakingQuery()
        {
            OnlyUnlocked = true,
            OnlyNotFull = true,
            GameState = RoomBased.GameState.InLobby
        };

        /// <summary>
        /// Get the query for the next page of matched rooms.
        /// </summary>
        /// <param name="previousPage">The previous query, used to create the query for the next page.</param>
        public MatchmakingQuery(MatchmakingQuery previousPage, short pageIncrement = 1)
        {
            if (previousPage == null)
                return;

            GameState = previousPage.GameState;
            MatchedName = previousPage.MatchedName;
            MatchedID = previousPage.MatchedID;
            RequiredRoomCode = previousPage.RequiredRoomCode;
            RequiredRoomTag = previousPage.RequiredRoomTag;
            OnlyNotFull = previousPage.OnlyNotFull;
            OnlyUnlocked = previousPage.OnlyUnlocked;

            ResultsPerPage = previousPage.ResultsPerPage;
            Page = (ushort)(previousPage.Page + pageIncrement);
        }

        /// <summary>
        /// The max amount of rooms a single matchmaking query request can return.
        /// This is to avoid slow serialization of room objects.
        /// </summary>
        public const ushort MaxResults = 30;

        public GameState? GameState { get; set; } = null;
        public string MatchedName { get; set; } = null;
        public string MatchedID { get; set; } = null;
        public string RequiredRoomCode { get; set; } = null;
        public string RequiredRoomTag { get; set; } = null;
        public bool OnlyNotFull { get; set; } = false;
        public bool OnlyUnlocked { get; set; } = false;

        /// <summary>
        /// The number of rooms to give per query result. Max per request is <see cref="MaxResults"/>
        /// </summary>
        public ushort ResultsPerPage { get; set; } = 16;
        /// <summary>
        /// If there were more room that matched the query than <see cref="ResultsPerPage"/>, 
        /// use this number to offset to the next collection of matches.
        /// </summary>
        public ushort Page { get; set; } = 0;

        public override string ToString()
        {
            List<string> content = new List<string>();

            content.Add($"{ ResultsPerPage } results, page { Page }");

            if (GameState != null)
                content.Add(GameState.ToString());
            if (OnlyNotFull)
                content.Add("Not full");
            if (OnlyUnlocked)
                content.Add("Unlocked");
            if (!string.IsNullOrWhiteSpace(MatchedName))
                content.Add("Name: " + MatchedName);
            if (!string.IsNullOrWhiteSpace(MatchedID))
                content.Add("ID: " + MatchedID);
            if (!string.IsNullOrWhiteSpace(RequiredRoomCode))
                content.Add("#" + RequiredRoomCode);
            if (!string.IsNullOrWhiteSpace(RequiredRoomTag))
                content.Add("Tag: " + RequiredRoomTag);

            return $"({ string.Join(", ", content) })";
        }

        public void FromBytes(byte[] from)
        {
            Stack<byte[]> stack = ByteUtil.ToSegmentStack(from);

            int gs = BitConverter.ToInt32(stack.Pop(), 0);
            if (gs < 0)
                GameState = null;
            else
                GameState = (GameState)gs;
            MatchedName = Encoding.ASCII.GetString(stack.Pop());
            MatchedID = Encoding.ASCII.GetString(stack.Pop());
            RequiredRoomCode = Encoding.ASCII.GetString(stack.Pop());
            RequiredRoomTag = Encoding.ASCII.GetString(stack.Pop());
            OnlyNotFull = BitConverter.ToBoolean(stack.Pop(), 0);
            OnlyUnlocked = BitConverter.ToBoolean(stack.Pop(), 0);
            ResultsPerPage = BitConverter.ToUInt16(stack.Pop(), 0);
            Page = BitConverter.ToUInt16(stack.Pop(), 0);
        }

        public byte[] ToBytes()
        {
            Stack<byte[]> stack = new Stack<byte[]>();

            stack.Push(BitConverter.GetBytes(GameState == null ? -1 : (int)GameState));
            stack.Push(Encoding.ASCII.GetBytes(MatchedName == null ? string.Empty : MatchedName));
            stack.Push(Encoding.ASCII.GetBytes(MatchedID == null ? string.Empty : MatchedID));
            stack.Push(Encoding.ASCII.GetBytes(RequiredRoomCode == null ? string.Empty : RequiredRoomCode));
            stack.Push(Encoding.ASCII.GetBytes(RequiredRoomTag == null ? string.Empty : RequiredRoomTag));
            stack.Push(BitConverter.GetBytes(OnlyNotFull));
            stack.Push(BitConverter.GetBytes(OnlyUnlocked));
            stack.Push(BitConverter.GetBytes(ResultsPerPage));
            stack.Push(BitConverter.GetBytes(Page));

            return ByteUtil.FromSegmentStack(stack);
        }
    }
}