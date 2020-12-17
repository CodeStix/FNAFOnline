using Stx.Logging;
using Stx.Net.RoomBased;

namespace Stx.Net.ServerOnly.RoomBased
{
    public class RoomApplication
    {
        /// <summary>
        /// The room on the server this application is bound too.
        /// </summary>
        public ServerRoom ServerRoom { get; internal set; }
        /// <summary>
        /// The room this application is bound too. Same as <see cref="ServerRoom.Underlaying"/>.
        /// </summary>
        public Room Room
        {
            get
            {
                return ServerRoom.Underlaying;
            }
        }
        /// <summary>
        /// The internal server this room application is running on. Same as <see cref="ServerRoom.Server"/>.
        /// </summary>
        public Server Server
        {
            get
            {
                return ServerRoom.Server;
            }
        }
        /// <summary>
        /// Gets the current state of this room. Same as <see cref="Room.State"/>.
        /// </summary>
        public GameState State
        {
            get
            {
                return Room.State;
            }
        }
        public ILogger Logger { get; set; } = StxNet.DefaultLogger;

        /// <summary>
        /// The default room application that basically does nothing but exist.
        /// </summary>
        public static RoomApplication Default { get; } = new RoomApplication();

        /// <summary>
        /// Gets called when the room is ready to use this application.
        /// </summary>
        public virtual void OnInitialize()
        { }

        /// <summary>
        /// Gets called when this room is ready to change its state to <see cref="GameState.InGame"/>.
        /// <para>Change the game state from here. Use <see cref="ServerRoom.TryChangeGameState(GameState)"/></para>
        /// </summary>
        public virtual void OnCanStart()
        { }

        /// <summary>
        /// Gets called when this room's game state changes to <see cref="GameState.InGame"/> .
        /// </summary>
        public virtual void OnStart()
        { }

        /// <summary>
        /// Gets called when this room's game state changes to <see cref="GameState.InLobby"/> .
        /// </summary>
        public virtual void OnEnd()
        { }

        public virtual void OnJoined(ClientData whoJoined)
        { }

        public virtual void OnLeft(ClientData whoLeft)
        { }

        /// <summary>
        /// Gets called when this room will get destroyed.
        /// </summary>
        public virtual void OnTerminate()
        { }

        /// <summary>
        /// Gets called when a player was promoted to the room owner.
        /// </summary>
        /// <param name="newOwnerID">The player that got promoted.</param>
        public virtual void OnPromoted(string newOwnerID)
        { }

        protected void RegisterHandler(string requiredKey, RequestDelegate<ClientIdentity> handler)
            => Server.CreateRoomRequestHandler(Room.ID, requiredKey, handler);
        
        protected void UnRegisterHandler(string key)
           =>  Server.RemoveRoomRequestHandler(Room.ID, key);
        
        protected void UnRegisterAllHandlers()
            => Server.RemoveAllRoomRequestHandlers(Room.ID);

        protected bool ChangeState(GameState newGameState)
            => ServerRoom.TryChangeGameState(newGameState);
        
        protected void Kick(params string[] clientIDsToKick)
            => ServerRoom.KickAsServer(clientIDsToKick);

        protected void Kick(ClientData clientToKick)
            => ServerRoom.KickAsServer(clientToKick);

        protected void KickAll()
            => ServerRoom.KickAll();

        protected void SelfDestruct()
            => Server.Matchmaking.DestroyRoom(ServerRoom);
    }
}
