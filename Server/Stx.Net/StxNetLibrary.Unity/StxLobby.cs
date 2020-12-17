using UnityEngine;

namespace Stx.Net.Unity.UI
{
    public class StxLobby : MonoBehaviour
    {
        public void LeaveAndMainScene()
        {
            StxUnityClient.F.LeaveRoomAsync((state, e) =>
            {
                StxUnityClient.Instance.SceneSwitchMain();
            });
        }
    }
}
