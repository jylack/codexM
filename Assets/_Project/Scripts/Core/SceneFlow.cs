// 로그인/룸/인게임 화면 전환 상태와 변경 이벤트를 관리합니다.
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
