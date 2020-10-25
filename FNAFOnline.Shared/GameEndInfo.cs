using Stx.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FNAFOnline.Shared
{
    public class GameEndInfo : IByteDefined<GameEndInfo>
    {
        public string EndingMessage { get; set; }
        public GameEndCause EndCause { get; set; }
        public string WhoEnded { get; set; }

        public GameEndInfo()
        { }

        public GameEndInfo(GameEndCause cause, string whoEnded)
        {
            this.EndCause = cause;
            this.WhoEnded = whoEnded;
        }

        public override string ToString()
        {
            return $"(Cause: { EndCause.ToString() }, Message: { EndingMessage })";
        }
    }

    public enum GameEndCause
    {
        None,
        SomeoneLeft,
        NightOver,
        KilledByBonnie,
        KilledByChica,
        KilledByFreddy,
        KilledByFoxy,
        KilledByGoldenFreddy
    }
}
