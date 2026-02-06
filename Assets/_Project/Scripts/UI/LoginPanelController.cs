// 로그인 패널 UI를 구성하고 로그인 버튼 이벤트를 전달합니다.
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class LoginPanelController
    {
        private readonly GameObject _panel;

        public LoginPanelController(Transform parent, Action<string> onLogin)
        {
            _panel = UIFactory.Panel(parent, "LoginPanel");
            UIFactory.Text(_panel.transform, "Login", new Vector2(0, 220));
            var input = UIFactory.Input(_panel.transform, "account id", new Vector2(0, 70));
            var button = UIFactory.Button(_panel.transform, "Login", new Vector2(0, -40), () => onLogin?.Invoke(input.text));
            button.GetComponentInChildren<Text>().text = "Login";
        }

        public void SetActive(bool active) => _panel.SetActive(active);
    }
}
