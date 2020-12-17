using Stx.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FNAFOnline.Shared
{
    public class MoveTimings : IByteDefined<MoveTimings>
    {
        public float SecondsPerMove { get; set; } = 15f;
        public float MaxRandomness { get; set; } = 7.5f;
        public float DecreaseOverNight { get; set; } = 5f;
        public float StartWait { get; set; } = 8f;
    }
}
