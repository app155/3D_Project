using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Project3D.UI
{
    public class UIManager
    {
        public static UIManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UIManager();

                return _instance;
            }
        }

        private static UIManager _instance;

        private Dictionary<Type, IUI> _uis = new Dictionary<Type, IUI>();
        private Stack<IUI> _showns = new Stack<IUI>();

        public T Get<T>()
            where T : IUI
        {
            if (_uis.TryGetValue(typeof(T), out IUI ui) == false)
            {
                throw new Exception("[UIManager] - Get");
            }

            return (T)ui;
        }

        public IEnumerable<IUI> GetAllUis()
        {
            return _uis.Values;
        }

        public void Register(IUI ui)
        {
            Type type = ui.GetType();

            if (_uis.ContainsKey(type))
            {
                throw new Exception("[UIManager] - Register");
            }

            _uis.Add(type, ui);
        }

        public void Push(IUI ui)
        {
            if (_showns.Count > 0 && _showns.Peek() == ui)
                return;

            int sortingOrder = 0;

            if (_showns.Count > 0)
            {
                sortingOrder = _showns.Peek().sortingOrder + 1;
                _showns.Peek().inputActionEnabled = false;
            }

            _showns.Push(ui);
            ui.sortingOrder = sortingOrder;
            ui.inputActionEnabled = true;
        }

        public void Pop()
        {
            if (_showns.Count > 0 == false)
                return;

            _showns.Pop();

            if (_showns.Count > 0)
                _showns.Peek().inputActionEnabled = true;
        }
    }
}