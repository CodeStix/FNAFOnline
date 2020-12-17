using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Stx.Net.Unity
{
    [Serializable]
    public class UnityVoidEvent : UnityEvent { }
    [Serializable]
    public class UnityBoolEvent : UnityEvent<bool> { }
    [Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    [Serializable]
    public class UnityIntEvent : UnityEvent<int> { }
    [Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }
}
