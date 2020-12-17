using Stx.Net.Achievements;
using Stx.Serialization;
using Stx.Utilities;

namespace Stx.Net.RoomBased
{
    public class Client : BaseClient<ClientIdentity>, INetworkedItem
    {
        /// <summary>
        /// Gets called when this client has reached an achievement's goal. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="info">Information about the granted achievement.</param>
        public delegate void AchievementGrantedDelegate(AchievementGrantedInfo info);
        public event AchievementGrantedDelegate OnAchievementGranted;

        /// <summary>
        /// Gets called when this client gets kicked from a room, either from leaving or getting kicked by someone. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        public delegate void KickedFromRoomDelegate();
        public event KickedFromRoomDelegate OnKickedFromRoom;

        /// <summary>
        /// Gets called when a client joins your room. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="joinedClient">The ID if the joined client.</param>
        public delegate void SomeoneJoinedRoomDelegate(ClientIdentity joinedClient);
        public event SomeoneJoinedRoomDelegate OnSomeoneJoinedRoom;

        /// <summary>
        /// Gets called when a client leaves your room. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="leftClient">The ID if the client who left.</param>
        public delegate void SomeoneLeftRoomDelegate(ClientIdentity leftClient);
        public event SomeoneLeftRoomDelegate OnSomeoneLeftRoom;

        /// <summary>
        /// Gets called when the game state of the current room changes. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="newGameState">The new game state.</param>
        public delegate void GameStateChangedDelegate(GameState newGameState);
        public event GameStateChangedDelegate OnGameStateChange;

        /// <summary>
        /// Gets called when someone in the room changes its room status. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="newStatus">The new status of the player.</param>
        public delegate void SomeoneChangedStatusDelegate(ClientIdentity clientWhoChanged, ClientRoomStatus newStatus);
        public event SomeoneChangedStatusDelegate OnSomeoneChangedStatus;

        /// <summary>
        /// Gets called when you receive a chat. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="chatMessage">Information about the message being send.</param>
        public delegate void ChatDelegate(ChatEntry chatMessage);
        public event ChatDelegate OnChat;

        /// <summary>
        /// Gets called when their is a new owner in your current room. Take a look at <see cref="ThreadSafeData"/> to make this event thread-safe. 
        /// </summary>
        /// <param name="promotedClient">The client that was promoted to new room owner.</param>
        public delegate void SomeonePromotedDelegate(string promotedClient);
        public event SomeonePromotedDelegate OnSomeonePromoted;

        public Client(ConnectionStartInfo connectInfo) : base(connectInfo)
        { }

        public Client(string applicationKey, string host, ushort port, string authToken, string clientID) : 
            base(applicationKey, host, port, authToken, clientID)
        { }

        public override void RegisterNetworkTypes()
        {
            base.RegisterNetworkTypes();

            Bytifier.Include<BList<Room>>();
            Bytifier.Include<BConcurrentList<Room>>();
            Bytifier.Include<MatchmakingQuery>();
            Bytifier.Include<MatchmakingQueryResult>();
            Bytifier.Include<Room>();
            Bytifier.Include<RoomTemplate>();
            Bytifier.Include<ClientIdentity>();
            Bytifier.Include<ClientInfo>();
            Bytifier.Include<ChatEntry>();
            Bytifier.Include<AchievementGrantedInfo>();
            Bytifier.IncludeEnum<ClientStatus>();
            Bytifier.IncludeEnum<ClientRoomStatus>();
            Bytifier.IncludeEnum<GameState>();
            Bytifier.IncludeEnum<ChatSourceType>();
        }

        public override void RegisterDataHandlers(DataReceiver receiver)
        {
            base.RegisterDataHandlers(receiver);

            receiver.AddHandler(new DataHandler<AchievementGrantedInfo>("_Achievement", (a, p) =>
            {
                OnAchievementGranted?.Invoke(a);
            }));
            receiver.AddHandler(new ObjectHandler("_KickedFromRoom", (a, p) =>
            {
                OnKickedFromRoom?.Invoke();
            }));
            receiver.AddHandler(new DataHandler<ClientIdentity>("_SomeoneJoinedRoom", (a, p) =>
            {
                OnSomeoneJoinedRoom?.Invoke(a);
            }));
            receiver.AddHandler(new DataHandler<ClientIdentity>("_SomeoneLeftRoom", (a, p) =>
            {
                OnSomeoneLeftRoom?.Invoke(a);
            }));
            receiver.AddHandler(new DataHandler<ClientRoomStatus>("_SomeoneChangedStatus", (a, p) =>
            {
                OnSomeoneChangedStatus?.Invoke(p.Data.Get<ClientIdentity>("WhoChanged"), a);
            }));
            receiver.AddHandler(new DataHandler<GameState>("_GameStateChanged", (a, p) =>
            {
                OnGameStateChange?.Invoke(a);
            }));
            receiver.AddHandler(new DataHandler<ChatEntry>("_Chat", (a, p) =>
            {
                OnChat?.Invoke(a);
            }));
            receiver.AddHandler(new DataHandler<string>("_SomeonePromoted", (a, p) =>
            {
                OnSomeonePromoted?.Invoke(a);
            }));
        }

        public override void ProcessAcknowledgmentPacket(Packet p, bool isFirstTimeConnecting)
        {
            base.ProcessAcknowledgmentPacket(p, isFirstTimeConnecting);
        }
    }
}