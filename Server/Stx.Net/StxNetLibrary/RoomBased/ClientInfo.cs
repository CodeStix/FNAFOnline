using Stx.Utilities;
using System.Collections.Generic;
using Stx.Serialization;

namespace Stx.Net.RoomBased
{
    public class ClientInfo : IByteDefined<ClientInfo>
    {
        public string Name { get; set; } = DefaultName;
        public string AvatarUrl { get; set; } = DefaultAvatarUrl;
        public BHashtable Fields { get; set; } = new BHashtable();

        public const string DefaultName = "Freddy";
        public const string DefaultAvatarUrl = NoAvatarUrl;
        public const string NoAvatarUrl = null;

        [DoNotSerialize]
        public bool HasAvatar
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AvatarUrl);
            }
        }

        public ClientInfo()
        { }

        public void SetField(string key, object value)
        {
            Fields.Modify(key, value);
        }

        public T GetField<T>(string key, T defaultValue = default(T))
        {
            return (T)Fields.Get(key, defaultValue);
        }

        public bool IsValid()
        {
            return StringChecker.IsValidName(Name);
        }

        public override string ToString()
        {
            List<string> content = new List<string>();

            content.Add("Name: " + Name);
            content.Add("Avatar: " + AvatarUrl);
            foreach (string key in Fields)
                content.Add($"{ key }: { Fields[key] }");

            return $"({ string.Join(", ", content) })";
        }
    }
}
