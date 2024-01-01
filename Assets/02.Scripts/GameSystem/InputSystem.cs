using Project3D.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;


namespace Project3D.GameSystem
{
    public class InputSystem : MonoBehaviour
    {
        public static InputSystem instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("InputSystem").AddComponent<InputSystem>();
                }

                return _instance;
            }
        }

        private static InputSystem _instance;

        public class Map
        {
            public Dictionary<string, Action<float>> axesActions = new Dictionary<string, Action<float>>();

            
            public Dictionary<int, Action> mouseDownActions = new Dictionary<int, Action>();
            public Dictionary<KeyCode, Action> keyDownActions = new Dictionary<KeyCode, Action>();
            public Dictionary<KeyCode, Action> keyPressActions = new Dictionary<KeyCode, Action>();
            public Dictionary<KeyCode, Action> keyUpActions = new Dictionary<KeyCode, Action>();

            public void RegisterAxisAction(string axisName, Action<float> action)
            {
                if (axesActions.ContainsKey(axisName))
                    axesActions[axisName] += action;
                else
                    axesActions.Add(axisName, action);
            }

            public void RegisterMouseDownAction(int mouseButton, Action action)
            {
                if (mouseDownActions.ContainsKey(mouseButton))
                    mouseDownActions[mouseButton] += action;
                else
                    mouseDownActions.Add(mouseButton, action);
            }

            public void RegisterKeyDownAction(KeyCode keyCode, Action action)
            {
                if (keyDownActions.ContainsKey(keyCode))
                    keyDownActions[keyCode] += action;
                else
                    keyDownActions.Add(keyCode, action);
            }

            public void RegisterKeyPressAction(KeyCode keyCode, Action action)
            {
                if (keyPressActions.ContainsKey(keyCode))
                    keyPressActions[keyCode] += action;
                else
                    keyPressActions.Add(keyCode, action);
            }

            public void RegisterKeyUpAction(KeyCode keyCode, Action action)
            {
                if (keyUpActions.ContainsKey(keyCode))
                    keyUpActions[keyCode] += action;
                else
                    keyUpActions.Add(keyCode, action);
            }

            public void DoActions()
            {
                foreach (var pair in axesActions)
                {
                    pair.Value.Invoke(Input.GetAxisRaw(pair.Key));
                }

                foreach (var pair in mouseDownActions)
                {
                    if (Input.GetMouseButtonDown(pair.Key))
                        pair.Value.Invoke();
                }

                foreach (var pair in keyDownActions)
                {
                    if (Input.GetKeyDown(pair.Key))
                        pair.Value.Invoke();
                }

                foreach (var pair in keyPressActions)
                {
                    if (Input.GetKey(pair.Key))
                        pair.Value.Invoke();
                }

                foreach (var pair in keyUpActions)
                {
                    if (Input.GetKeyUp(pair.Key))
                        pair.Value.Invoke();
                }
            }
        }
        public Dictionary<string, Map> maps = new Dictionary<string, Map>();
        public string current;

        public void Init()
        {
            Map playerMap = new Map();

            playerMap.RegisterKeyDownAction(KeyCode.Tab, () =>
            {
                UI_TabUI ui = UIManager.instance.Get<UI_TabUI>();
                if (ui.gameObject.activeSelf == false)
                {
                    ui.Show();
                    ui.Refresh();
                }

            });

            playerMap.RegisterKeyUpAction(KeyCode.Tab, () =>
            {
                UI_TabUI ui = UIManager.instance.Get<UI_TabUI>();
                if (ui.gameObject.activeSelf)
                    ui.Hide();
            });

            maps.Add("Player", playerMap);
            current = "Player";
        }

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            if (maps.ContainsKey(current))
                maps[current].DoActions();
        }
    }
}