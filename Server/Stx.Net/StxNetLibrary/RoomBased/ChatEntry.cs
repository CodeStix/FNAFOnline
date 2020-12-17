using Stx.Serialization;
using System;

namespace Stx.Net.RoomBased
{
    public class ChatEntry : IByteDefined<ChatEntry>
    {
        public string SenderID { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public ChatSourceType SourceType { get; set; }
        public ClientIdentity Sender { get; set; }
        [DoNotSerialize]
        public string SenderDisplayName
        {
            get
            {
                if (Sender == null)
                    return SenderID;
                else
                    return Sender.DisplayName;
            }
        }

        public static string ChatStringFormat { get; set; } = "{0}: {1}";

        public ChatEntry()
        { }

        public ChatEntry(string senderID, string message, ChatSourceType type)
        {
            SenderID = senderID;
            Sender = null;
            Message = message;
            SourceType = type;
            CreatedDateTime = DateTime.Now;
        }

        public ChatEntry(ClientIdentity sender, string message, ChatSourceType type)
        {
            SenderID = sender.NetworkID;
            Sender = sender;
            Message = message;
            SourceType = type;
            CreatedDateTime = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format(ChatStringFormat, SenderDisplayName, Message) + Environment.NewLine;
        }
    }


}
