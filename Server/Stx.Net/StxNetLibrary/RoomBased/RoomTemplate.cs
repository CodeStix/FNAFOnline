using Stx.Serialization;
using Stx.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Net.RoomBased
{
    public class RoomTemplate : IByteDefined<RoomTemplate>
    {
        public string Name { get; set; } = DefaultRoomName;
        public int MaxPlayers { get; set; } = 4;
        public string Password { get; set; } = null;
        public bool Hidden { get; set; } = false;

        [DoNotSerialize]
        public bool Locked
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Password);
            }
        }

        [DoNotSerialize]
        public static string DefaultRoomName { get; set; } = "Default Room";
        [DoNotSerialize]
        public static int MinMaxPlayers { get; set; } = 2;
        [DoNotSerialize]
        public static int MaxMaxPlayers { get; set; } = 16;

        public RoomTemplate()
        { }

        /// <summary>
        /// Creates a room template, used to create a room on the server.
        /// </summary>
        /// <param name="name">The name of the room.</param>
        /// <param name="maxPlayers">The maximum amount of players the room should hold.</param>
        /// <param name="password">The password used to lock this room, null if no password.</param>
        /// <param name="hidden">True if you want to hide the room from matchmaking queries.</param>
        public RoomTemplate(string name, int maxPlayers, string password = null, bool hidden = false)
        {
            Name = name?.Trim();
            MaxPlayers = maxPlayers;
            Password = password?.Trim();
            Hidden = hidden;
        }

        /// <summary>
        /// Creates a room template with random name, used to create a room on the server.
        /// </summary>
        /// <param name="maxPlayers">The maximum amount of players the room should hold.</param>
        /// <param name="password">The password used to lock this room, null if no password.</param>
        /// <param name="hidden">True if you want to hide the room from matchmaking queries.</param>
        public RoomTemplate(int maxPlayers, string password = null, bool hidden = false)
        {
            Name = "Room" + new Random().Next(1000);
            MaxPlayers = maxPlayers;
            Password = password?.Trim();
            Hidden = hidden;
        }

        public bool IsValid()
        {
            return StringChecker.IsValidFullName(Name)
                && (StringChecker.IsValidShortPassword(Password) || !Locked)
                && MaxPlayers >= MinMaxPlayers && MaxPlayers <= MaxMaxPlayers;
        }

        public override string ToString()
        {
            return $"({ Name }, { MaxPlayers } max, { (Locked ? "LOCKED, " : "") })";
        }
    }
}
