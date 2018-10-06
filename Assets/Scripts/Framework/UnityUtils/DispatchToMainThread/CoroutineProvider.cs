using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AW.Resources {

    /// <summary>
    /// Coroutine provider. 一次只能有一个Coroutine在工作
    /// </summary>
    public class CoroutineProvider : MonoBehaviour {
        private static CoroutineProvider _current;
        public static CoroutineProvider Instance() {
            Initialize();
            return _current;
        }

        void Awake() {
            _current = this;
            initialized = true;
        }

        private static bool initialized = false;
        static void Initialize() {
            if (!initialized) {
                if(!Application.isPlaying)
                    return;
                initialized = true;
                var g = new GameObject("Corountine Provider");

                var gobal = GameObject.FindGameObjectWithTag("Global");
                if(gobal != null) g.transform.parent = gobal.transform;
                _current = g.AddComponent<CoroutineProvider>();
                DontDestroyOnLoad(g);
            }
        }

    }

}