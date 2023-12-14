using System;
using UnityEngine;

namespace Project3D.UI
{
    public abstract class UIMonobehaviour : MonoBehaviour, IUI
    {
        public int sortingOrder
        {
            get => canvas.sortingOrder;
            set => canvas.sortingOrder = value;
        }

        public bool inputActionEnabled { get; set; }

        public event Action onShow;
        public event Action onHide;

        protected Canvas canvas;

        public virtual void Init()
        {
            canvas = GetComponent<Canvas>();
            UIManager.instance.Register(this);
        }

        public abstract void InputAction();

        public void Show()
        {
            UIManager.instance.Push(this);
            gameObject.SetActive(true);
            onShow?.Invoke();
        }

        public void Hide()
        {
            UIManager.instance.Pop();
            gameObject.SetActive(false);
            onHide?.Invoke();
        }
    }
}