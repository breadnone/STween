using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using System;
using System.Linq;
using Breadnone.Extension;

namespace TweenLoop
{
    //Update dummy class
    sealed class AwaitUpdate { }
    //Update dummy class
    public sealed class TweenPlayerLoop
    {
        public static TweenPlayerLoop tweenLoop;
        public static PlayerLoopSystem playerLoop;
        static Action Update;

        public void RegisterUpdate(Action update)
        {
            Update += update;
        }
        public void UnregisterUpdate(Action update)
        {
            Update -= update;
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
            playerLoop = PlayerLoop.GetDefaultPlayerLoop();

            if (addElseRemove)
            {
                var copy = InjectCustomUpdate(ref playerLoop, true);
                PlayerLoop.SetPlayerLoop(copy);
            }
            else
            {
                var copy = InjectCustomUpdate(ref playerLoop, false);
                PlayerLoop.SetPlayerLoop(copy);
            }
        }
        bool OnQuit()
        {
            AssignPlayerLoop(false);
            return true;
        }

        PlayerLoopSystem CreateLoopSystem()
        {
            PlayerLoopSystem before = default;

            before = new PlayerLoopSystem()
            {
                updateDelegate = UpdateRun,
                type = typeof(AwaitUpdate)
            };

            return before;
        }

        PlayerLoopSystem InjectCustomUpdate(ref PlayerLoopSystem root, bool addCustomUpdateElseClear)
        {
            var lis = root.subSystemList.ToList();
            int? index = null;

            for (int i = 0; i < root.subSystemList.Length; i++)
            {
                Type t = typeof(Update);

                if (lis[i].type == t)
                {
                    index = i;
                    break;
                }
            }

            if (index.HasValue)
            {
                var tmp = root.subSystemList[index.Value].subSystemList.ToList();

                for (int i = tmp.Count; i-- > 0;)
                {
                    Type t = typeof(AwaitUpdate);

                    if (tmp[i].type == t)
                    {
                        tmp.Remove(tmp[i]);
                    }
                }

                if (addCustomUpdateElseClear)
                {
                    var sys = CreateLoopSystem();
                    int idx = 0;

                    idx = tmp.FindIndex(x => x.type == typeof(Update.ScriptRunBehaviourUpdate));

                    var beforeIndex = idx--;
                    var afterIndex = idx++;
 
                    if (idx == 0)
                    {
                        beforeIndex = 0;
                        afterIndex = 2;
                    }

                    tmp.Insert(beforeIndex, sys);
                }

                root.subSystemList[index.Value].subSystemList = tmp.ToArray();
            }

            return root;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if (tweenLoop == null)
            {
                TweenManager.isPlayMode = true;
                tweenLoop = new TweenPlayerLoop();
                tweenLoop.RegisterUpdate(TweenManager.TweenWorkerUpdate);
            }
            else
            {
                tweenLoop.RegisterUpdate(TweenManager.TweenWorkerUpdate);
            }
        }
 
        public static void RetriggerUpdateLoop()
        {
            if(TweenManager.mono != null)
            {            
                Init();
            }
        }

        void UpdateRun()
        {
            Update?.Invoke();
        }

        /// <summary>
        /// Finds subsystem.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="def"></param>
        /// <returns></returns>
        private static PlayerLoopSystem FindSubSystem<T>(PlayerLoopSystem def)
        {
            if (def.type == typeof(T))
            {
                return def;
            }
            if (def.subSystemList != null)
            {
                foreach (var s in def.subSystemList)
                {
                    var system = FindSubSystem<Update.ScriptRunBehaviourUpdate>(s);
                    if (system.type == typeof(T))
                    {
                        return system;
                    }
                }
            }
            return default(PlayerLoopSystem);
        }
    }
}