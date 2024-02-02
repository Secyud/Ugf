using System;
using Secyud.Ugf.Modularity;
using UnityEditor;
using UnityEngine;

namespace Secyud.Ugf
{
    public abstract class UgfGameManager : MonoBehaviour
    {
        protected abstract Type StartUpModule { get; }
        protected abstract PlugInSourceList PlugInSourceList { get; }
        [field:SerializeField]public Camera Camera { get; private set; }
        [field:SerializeField]public Canvas Canvas { get; private set; }
        private UgfApplicationFactory Factory { get;  set; }
        public static UgfGameManager Instance { get; private set; }
        public IUgfApplication Application => Factory.Application;

        public virtual void Awake()
        {
            if (Instance)
            {
                throw new Exception("Cannot exist two game manager;");
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Factory = new UgfApplicationFactory(StartUpModule, PlugInSourceList);
            Factory.Configure();
        }

        public void Shutdown()
        {
            Factory.Shutdown();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}