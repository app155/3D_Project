using System;

namespace Project3D.UI
{
    public interface IUI
    {
        public int sortingOrder { get; set; }
        public bool inputActionEnabled { get; set; }

        public void InputAction();
        public void Show();
        public void Hide();

        public event Action onShow;
        public event Action onHide;
    }
}