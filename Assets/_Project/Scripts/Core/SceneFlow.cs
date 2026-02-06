using System;

namespace Project.Core
{
    public enum ScreenType
    {
        Login,
        Room,
        InGame
    }

    public class SceneFlow
    {
        public event Action<ScreenType> OnScreenChanged;
        public ScreenType CurrentScreen { get; private set; }

        public void Enter(ScreenType screen)
        {
            CurrentScreen = screen;
            OnScreenChanged?.Invoke(screen);
        }
    }
}
