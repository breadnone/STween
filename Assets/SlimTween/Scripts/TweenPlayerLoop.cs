using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using System;
using System.Linq;
using Breadnone.Extension;

namespace TweenLoop
{
    [Serializable]
    struct AwaitUpdate { }
    [Serializable]
    public sealed class TweenPlayerLoop
    {
        public static TweenPlayerLoop tweenLoop;
        public static int stExecutionOrder 
        {
            get
            {
                if(PlayerPrefs.HasKey("st-hn-nh-order-mode"))
                {
                    return PlayerPrefs.GetInt("st-hn-nh-order-mode");
                }
                else
                {
                    PlayerPrefs.SetInt("st-hn-nh-order-mode", 0);
                    return 0;
                }
            }
            set
            {
                PlayerPrefs.SetInt("st-hn-nh-order-mode", value);
            }
        }
        public TweenPlayerLoop()
        {
            InitUpdate();
        }
        void InitUpdate()
        {
            Application.wantsToQuit += OnQuit;
            AssignPlayerLoop(true);
        }
        void AssignPlayerLoop(bool addElseRemove)
        {
            if (addElseRemove)
            {
                PlayerLoop.SetPlayerLoop(InjectCustomUpdate(PlayerLoop.GetCurrentPlayerLoop(), true));
            }
            else
            {
                PlayerLoop.SetPlayerLoop(InjectCustomUpdate(PlayerLoop.GetCurrentPlayerLoop(), false));
            }
        }
        bool OnQuit()
        {
            AssignPlayerLoop(false);
            Application.wantsToQuit -= OnQuit;
            tweenLoop = null;
            return true;
        }

        PlayerLoopSystem InjectCustomUpdate(PlayerLoopSystem root, bool addCustomUpdateElseClear)
        {
            int index = 0;

            for (int i = 0; i < root.subSystemList.Length; i++)
            {
                if (root.subSystemList[i].type == typeof(Update))
                {
                    index = i;
                    break;
                }
            }

            var tmp = root.subSystemList[index].subSystemList.ToList();

            for (int i = tmp.Count; i-- > 0;)
            {
                if (tmp[i].type == typeof(AwaitUpdate))
                {
                    tmp.Remove(tmp[i]);
                }
            }

            if (addCustomUpdateElseClear)
            {
                var sys = new PlayerLoopSystem()
                {
                    updateDelegate = TweenManager.TweenWorkerUpdate,
                    type = typeof(AwaitUpdate)
                };

                int idx = tmp.FindIndex(x => x.type == typeof(Update.ScriptRunBehaviourUpdate));
                var beforeIndex = idx--;
                var afterIndex = idx++;

                if (idx == 0)
                {
                    beforeIndex = 0;
                    afterIndex = 2;
                }

                if(stExecutionOrder == 0)
                {
                    tmp.Insert(beforeIndex, sys);
                }
                else
                {
                    tmp.Insert(afterIndex, sys);
                }
            }

            root.subSystemList[index].subSystemList = tmp.ToArray();
            return root;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            TweenManager.isPlayMode = true;
            tweenLoop = new TweenPlayerLoop();
        }

        public static void RetriggerUpdateLoop()
        {
            if (TweenManager.mono != null)
            {
                Init();
            }
        }
    }
}