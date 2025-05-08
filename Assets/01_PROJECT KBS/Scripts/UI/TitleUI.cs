using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class TitleUI : MonoBehaviour
    {
        public void OnClickGameStart()
        {
            Main.Singleton.ChangeScene(SceneType.Ingame);
        }
        public void OnClickSetting()
        {

        }
        public void OnClickQuit()
        {

        }
    }
}
