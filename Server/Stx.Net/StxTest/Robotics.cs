using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afton
{
    interface IRelativeLocated
    {
        float RelativeX { get; set; }
        float RelativeY { get; set; }
    }

    interface IRelativeLocated3D : IRelativeLocated
    {
        float RelativeZ { get; set; }
    }

    interface ICube
    {
        float Width { get; set; }
        float Height { get; set; }
        float Depth { get; set; }

        float CalculateVolume();
        float CalculateSurface();
    }

    interface IRoom : ICube, IRelativeLocated
    {
        int WindowCount { get; }
        int DoorCount { get; }
    }

    interface IKitchen : IRoom
    {
        bool? ContainsFurnace { get; }
        int ShelfCount { get; }
    }

    interface IHall : INamedRoom
    {
        float Length { get; }
    }

    interface IGameArea : INamedRoom
    {
        bool PinballPresent { get; set; }
        bool CarrouselPresent { get; set; }
        bool BalloonBoyPresent { get; set; }
    }

    interface ILivingRoom : INamedRoom
    {
        bool HasTelevision { get; set; }
        bool HasHeater { get; set; }
    }

    interface ITimedOpenings
    {
        DateTime OpenTime { get; }
        DateTime CloseTime { get; }

        bool IsOpen();
    }

    interface IEmployee
    {
        DateTime ShiftStart { get; }
        DateTime ShiftEnd { get; }

        float MoneyPerHour { get; }
    }

    class Person : IHumanoid, INamedObject
    {
        public string Name { get; set; }
        public DateTime Born { get; set; }
        
        public Limb LeftLeg { get; }
        public Limb RightLeg { get; }
        public Limb LeftArm { get; }
        public Limb RightArm { get; }
        public Limb Spine { get; }
        public Limb Neck { get; }
        public Limb Skull { get; }
        public Mouth Mouth { get; }
        public Brain Brain { get; }

        public int Age
        {
            get
            {
                return DateTime.UtcNow.Year - Born.Year;
            }
        }

        public Person(string name)
        {
            this.Name = name;
        }

        public Person(string name, DateTime born)
        {
            this.Name = name;
            this.Born = born;
        }
    }

    class NightGuard : Person, IEmployee
    {
        public DateTime ShiftEnd { get; set; }
        public DateTime ShiftStart { get; set; }

        public float MoneyPerHour { get; private set; } = 4.0f;

        public NightGuard(string name) : base(name)
        { }

        public NightGuard(string name, DateTime born) : base(name, born)
        { }
    }

    class Janitor : Person, IEmployee
    {
        public DateTime ShiftEnd { get; set; }
        public DateTime ShiftStart { get; set; }

        public float MoneyPerHour { get; private set; } = 6.0f;

        public Janitor(string name) : base(name)
        { }

        public Janitor(string name, DateTime born) : base(name, born)
        { }
    }

    interface INamedObject
    {
        string Name { get; set; }
    }

    interface INamedRoom : IRoom, INamedObject
    { }

    class Room : INamedRoom
    {
        public float RelativeX { get; set; }
        public float RelativeY { get; set; }

        public float Depth { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }

        public int WindowCount { get; set; }
        public int DoorCount { get; set; }

        public string Name { get; set; }

        public Room(string name, float x, float y, float w, float h, float d)
        {
            Name = name;
            RelativeX = x;
            RelativeY = y;
            Width = w;
            Height = h;
            Depth = d;
        }

        public float CalculateSurface()
        {
            return Depth * Width;
        }

        public float CalculateVolume()
        {
            return Depth * Height * Width;
        }
    }

    class Establishment : ITimedOpenings, ICube
    {
        public float Depth { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }

        public IHall MainHall { get; set; }
        public ILivingRoom MainLivingRoom { get; set; }
        public List<INamedObject> Rooms { get; set; } = new List<INamedObject>();

        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }

        public bool IsOpen()
        {
            return DateTime.UtcNow >= OpenTime && DateTime.UtcNow < CloseTime;
        }

        public float CalculateSurface()
        {
            return Depth * Width;
        }

        public float CalculateVolume()
        {
            return Depth * Height * Width;
        }
    }

    class Restaurant : Establishment
    {
        public IKitchen Kitchen { get; set; }

        public bool? ContainsFurnace { get; set; } = null;
        public int ShelfCount { get; set; } = 5;
    }

    class Freddys : Restaurant
    {
        public NightGuard guard;
        public Person owner;
        public Janitor janitor;
        public HumanoidController[] animatronics;

        public Freddys()
        {
            Depth = 20.0f;
            Height = 2.0f;
            Width = 20.0f;

            Room r = new Room("1A", 6, 5, 3, 9, 4);
            Room r2 = new Room("1C", 6, 5, 3, 9, 4);

            Rooms.Add(r);
            Rooms.Add(r2);
            Rooms.Add(new Room("5", 1, 5, 3, 2, 4));
            Rooms.Add(new Room("1B", 1, 5, 15, 1, 9));
            Rooms.Add(new Room("7", 9, 6, 1, 8, 4));
            Rooms.Add(new Room("6", 4, 5, 3, 7, 5));
            Rooms.Add(new Room("3", 7, 5, 3, 2, 7));
            Rooms.Add(new Room("2A", 5, 1, 4, 3, 8));
            Rooms.Add(new Room("2B", 5, 5, 4, 2, 9));
            Rooms.Add(new Room("4A", 15, 1, 6, 5, 7));
            Rooms.Add(new Room("4B", 15, 5, 9, 5, 4));
            Rooms.Add(new Room("O", 10, 5, 3, 5, 4));

            OpenTime = DateTime.Parse("9:00");
            CloseTime = DateTime.Parse("18:00");

            owner = new Person(Identities.FreddiesOwnerName);

            guard = new NightGuard(Identities.NightGuardName);
            guard.ShiftStart = DateTime.Parse("0:00");
            guard.ShiftEnd = DateTime.Parse("6:00");
            guard.ShiftEnd.AddHours(Things.GuardOvertimeHours);

            janitor = new Janitor(Identities.JanitorName);
            janitor.ShiftStart = DateTime.Parse("6:00");
            janitor.ShiftEnd = DateTime.Parse("9:00");

            animatronics = new HumanoidController[5];
            animatronics[0] = new HumanoidController(new Animatronic(Identities.Robot1, MaterialType.Fur, MaterialColor.Yellow));
            animatronics[1] = new HumanoidController(new Animatronic(Identities.Robot2, MaterialType.Fur, MaterialColor.Purple));
            animatronics[2] = new HumanoidController(new Animatronic(Identities.Robot3, MaterialType.Fur, MaterialColor.Brown));
            animatronics[3] = new HumanoidController(new Animatronic(Identities.Robot4, MaterialType.Fur, MaterialColor.DarkPink));
            animatronics[4] = new HumanoidController(new Animatronic(Identities.Robot5, MaterialType.Fur, MaterialColor.GoldYellow));

            for(int i = 0; i < animatronics.Length - 2; i++)
            {
                if (i != 3)
                    animatronics[i].DoMove(r);
                else
                    animatronics[i].DoMove(r2);
            }
        }
    }

    enum MaterialType
    {
        Fur
    }

    enum MaterialColor
    {
        Yellow,
        Gold,
        Brown,
        Purple,
        DarkPink,
        GoldYellow
    }

    interface ISuited
    {
        MaterialType SuitMaterial { get; set; }
        MaterialColor SuitColor { get; set; }
    }

    class Limb : IRelativeLocated3D, IBiteable
    {
        public float BreakForce { get; set; } = 5000.0f;

        public IRelativeLocated3D[] Joints { get; set; }

        public float RelativeX { get; set; }
        public float RelativeY { get; set; }
        public float RelativeZ { get; set; }

        public void Break()
        { }

        public void MoveAnglePivit(int index, float angle)
        {
            IRelativeLocated3D pivit = Joints[index];
            pivit.RelativeX = (float)Math.Sin(angle) * RelativeX;
            pivit.RelativeY = (float)Math.Cos(angle) * RelativeY;
            pivit.RelativeZ =  RelativeZ;
        }
    }

    interface IBiteable
    {
        float BreakForce { get; set; }
        void Break();
    }

    interface ICanBite
    {
        float MaxBiteForce { get; set; }
        float MouthOpenHeight { get; set; }
        float MouthOpenWidth { get; set; }
        bool LimitMaxBiteForce { get; }
    }

    interface IHasTeeth
    {
        int TeethCount { get; }
    }

    class Mouth : ICanBite, IHasTeeth
    {
        public int TeethCount { get; }
        public float MaxBiteForce { get; set; }
        public float MouthOpenHeight { get; set; }
        public float MouthOpenWidth { get; set; }
        public bool LimitMaxBiteForce { get; protected set; } = true;

        public void BiteOn(IBiteable b)
        {
            if (MaxBiteForce >= b.BreakForce)
            {
                if (LimitMaxBiteForce)
                {
                    throw new Exception("That force could cause some fatal results.");
                }
                else
                {
                    b.Break();
                }
            } 
        }
    }

    interface IHumanoid : INamedObject
    {
        Limb LeftLeg { get; }
        Limb RightLeg { get; }
        Limb LeftArm { get; }
        Limb RightArm { get; }
        Limb Spine { get; }
        Limb Neck { get; }
        Limb Skull { get; }
        Mouth Mouth { get; }
        Brain Brain { get; }
    }

    class HumanoidController
    {
        public IMovableHumanoid Controlled { get; set; }

        public HumanoidController(IMovableHumanoid forHumanoid)
        {
            Controlled = forHumanoid;

            Terminal.OnReceive += Terminal_OnReceive;
        }

        private void Terminal_OnReceive(string command, string[] args)
        {
            if (args[1] != Controlled.Name)
                return;

            switch(command)
            {
                case "move":
                    DoMove(MovementCalculator.ParseLocation(args[0]));
                    break;
                    
                case "attack":
                    Attack((IHumanoid)Controlled.Brain.LookFor(args[0]));
                    break;

                case "lure":
                    Controlled.Brain.DoInterest(args[0]);
                    break;
            }
        }

        public void DoMove(IRelativeLocated newLocation)
        {
            MovementCalculator.CalculateLimbs(newLocation, 
                Controlled.LeftLeg, 
                Controlled.RightLeg, 
                Controlled.LeftArm, 
                Controlled.RightArm, 
                Controlled.Spine, 
                Controlled.Neck, 
                Controlled.Skull
            );
            MovementCalculator.MoveTowards(Controlled, newLocation);
        }

        public void Attack(IHumanoid h)
        {
            Controlled.Mouth.BiteOn(h.Skull);
        }
    }

    interface IMovableHumanoid : IHumanoid, IRelativeLocated
    { }

    class Robot : IMovableHumanoid, INamedObject
    {
        public Limb LeftLeg { get; }
        public Limb RightLeg { get; }
        public Limb LeftArm { get; }
        public Limb RightArm { get; }
        public Limb Spine { get; }
        public Limb Neck { get; }
        public Limb Skull { get; }
        public Mouth Mouth { get; }
        public Brain Brain { get; }

        public string Name { get; set; }

        public float RelativeX { get; set; }
        public float RelativeY { get; set; }
    }

    class Animatronic : Robot, ISuited
    {
        public MaterialType SuitMaterial { get; set; }
        public MaterialColor SuitColor { get; set; }

        public Animatronic(string name, MaterialType suitMat, MaterialColor suitColor)
        {
            SuitMaterial = suitMat;
            SuitColor = suitColor;
        }
    }


    //NOT SEEN BY USER

    class Things
    {
        public static int GuardOvertimeHours { get; set; } = 0;
    }

    class Identities
    {
        public const string NightGuardName = "Mike Schmidt";
        public const string JanitorName = "Unknown";
        public const string FreddiesOwnerName = "Henry & co";

        public const string Robot1 = "Chica";
        public const string Robot2 = "Bonnie";
        public const string Robot3 = "Freddy";
        public const string Robot4 = "Foxy";
        public const string Robot5 = "GoldenFreddy";
    }

    class MovementCalculator
    {
        public static void CalculateLimbs(IRelativeLocated towards, params Limb[] limbsToMove)
        { }

        public static void MoveTowards(IRelativeLocated from, IRelativeLocated to)
        { }

        public static IRelativeLocated ParseLocation(string str)
        {
            return null;
        }
    }

    class Brain
    {
        public object LookFor(string description)
        {
            return null;
        }

        public object DoInterest(string description)
        {
            return null;
        }
    }

    class Terminal
    {
        public delegate void TerminalReceiveDelegate(string command, string[] args);
        public static event TerminalReceiveDelegate OnReceive;

        public void Send(string command, string[] args)
        {
            OnReceive.Invoke(command, args);
        }
    }
}
