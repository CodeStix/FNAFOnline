using Stx.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FNAFOnline.Shared
{
    public class GameSetup : IByteDefined<GameSetup>
    {
        public GameMode GameMode { get; set; }
        public MoveTimings MoveTimerTimings { get; set; }
        public float Overtime { get; set; } = 0;
        public float StartingPower { get; set; } = 100f;

        public GameSetup()
        { }

        public GameSetup (GameMode gameMode, MoveTimings moveTimerTimings, float startingPower = 100f, float overtime = 0f)
        {
            GameMode = gameMode;
            MoveTimerTimings = moveTimerTimings;
            Overtime = overtime;
            StartingPower = startingPower;
        }

        //public GameSetup(GameMode gameMode, MoveTimings moveTimerTimings, float startingPower = 100f, float overtime = 0f)
        //{
        //    GameMode = gameMode;
        //    MoveTimerTimings = moveTimerTimings;
        //    Overtime = overtime;
        //    StartingPower = startingPower;
        //}

        //public void SetPlayerRoles(string guardClientID, string aftonClientID)
        //{
        //    GuardClientID = guardClientID;
        //    AftonClientID = aftonClientID;
        //}

        public static void RegisterNetworkTypes()
        {
            Bytifier.IncludeManually<GameSetup>(100);
            Bytifier.IncludeManually<GameEndInfo>(101);
            Bytifier.IncludeManually<MoveTimings>(102);
            Bytifier.IncludeManually<GameEndCause>(103);
            Bytifier.IncludeManually<MonsterType>(104);
            Bytifier.IncludeManually<GameMode>(105);
        }
    }
}
