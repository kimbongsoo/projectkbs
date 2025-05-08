#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace KBS
{
    public class BootStrapper
    {
        private const string BootStrapperMenuPath = "PROJECT KBS/BootStrapper/Activate BootStrapper";

        private static bool IsActiveBootStrapper
        {
            get => UnityEditor.EditorPrefs.GetBool(BootStrapperMenuPath, false);
            set
            {
                UnityEditor.EditorPrefs.SetBool(BootStrapperMenuPath, value);
                UnityEditor.Menu.SetChecked(BootStrapperMenuPath, value);
            }
        }


        [UnityEditor.MenuItem(BootStrapperMenuPath, false)]

        private static void ActivateBootStrapper()
        {
            IsActiveBootStrapper = !IsActiveBootStrapper;
            UnityEditor.Menu.SetChecked(BootStrapperMenuPath, IsActiveBootStrapper);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

        public static void SystemBoot()
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();
            if (IsActiveBootStrapper && false == activeScene.name.Equals("Main"))
            {
                InternalBoot();
            }
        }

        private static void InternalBoot()
        {
            Main.Singleton.Initialize();
        }
    }
}
#endif