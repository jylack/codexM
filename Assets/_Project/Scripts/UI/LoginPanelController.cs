// LoginScreen.prefab의 참조를 사용해 로그인/회원가입 버튼 동작을 처리합니다.
using System.Reflection;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class LoginPanelController : MonoBehaviour
    {
        [Header("LoginScreen.prefab References")]
        [SerializeField] private InputField idInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button signUpButton;
        [SerializeField] private Text statusText;

        private AuthService _authService;
        private AccountRepository _accountRepository;
        private GameState _gameState;
        private SceneFlow _sceneFlow;

        private bool _isInitialized;

        public void Initialize(AuthService authService, AccountRepository accountRepository, GameState gameState, SceneFlow sceneFlow)
        {
            _authService = authService;
            _accountRepository = accountRepository;
            _gameState = gameState;
            _sceneFlow = sceneFlow;

            if (passwordInput != null)
            {
                passwordInput.contentType = InputField.ContentType.Password;
                passwordInput.ForceLabelUpdate();
            }

            if (!_isInitialized)
            {
                loginButton?.onClick.AddListener(HandleLoginClicked);
                signUpButton?.onClick.AddListener(HandleSignUpClicked);
                _isInitialized = true;
            }

            SetStatus("로그인 정보를 입력하세요.", false);
        }

        public void SetupRuntimeFallbackUI()
        {
            var panelImage = gameObject.GetComponent<Image>() ?? gameObject.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.7f);

            var rt = gameObject.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            UIFactory.Text(transform, "Login", new Vector2(0, 220));
            idInput = UIFactory.Input(transform, "account id", new Vector2(0, 80));
            passwordInput = UIFactory.Input(transform, "password", new Vector2(0, 0));
            passwordInput.contentType = InputField.ContentType.Password;

            loginButton = UIFactory.Button(transform, "Login", new Vector2(-140, -100), HandleLoginClicked);
            signUpButton = UIFactory.Button(transform, "SignUp", new Vector2(140, -100), HandleSignUpClicked);
            statusText = UIFactory.Text(transform, string.Empty, new Vector2(0, -200));
            statusText.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        private void OnDestroy()
        {
            loginButton?.onClick.RemoveListener(HandleLoginClicked);
            signUpButton?.onClick.RemoveListener(HandleSignUpClicked);
        }

        private void HandleLoginClicked()
        {
            if (TryAuthenticate(isSignUp: false, out var normalizedId))
            {
                LoadAccountAndEnterRoom(normalizedId);
                SetStatus($"로그인 성공: {normalizedId}", true);
                return;
            }

            SetStatus("로그인 실패: ID/비밀번호를 확인하세요.", false);
        }

        private void HandleSignUpClicked()
        {
            if (TryAuthenticate(isSignUp: true, out var normalizedId))
            {
                LoadAccountAndEnterRoom(normalizedId);
                SetStatus($"회원가입(프로토타입) 완료: {normalizedId}", true);
                return;
            }

            SetStatus("회원가입 실패: ID를 확인하세요.", false);
        }

        private bool TryAuthenticate(bool isSignUp, out string normalizedId)
        {
            normalizedId = string.Empty;
            var rawId = idInput != null ? idInput.text : string.Empty;
            var password = passwordInput != null ? passwordInput.text : string.Empty;

            if (string.IsNullOrWhiteSpace(rawId) || _authService == null)
            {
                return false;
            }

            var id = rawId.Trim();
            var loginSucceeded = InvokeAuthLogin(id, password);
            if (!loginSucceeded && isSignUp)
            {
                // 프로토타입 정책: SignUp은 Login과 동일 정책을 사용합니다.
                loginSucceeded = _authService.Login(id);
            }

            if (!loginSucceeded)
            {
                return false;
            }

            normalizedId = string.IsNullOrWhiteSpace(_authService.CurrentAccountId) ? id : _authService.CurrentAccountId;
            return true;
        }

        private bool InvokeAuthLogin(string id, string password)
        {
            var authType = _authService.GetType();
            var passwordLogin = authType.GetMethod("Login", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(string) }, null);
            if (passwordLogin != null)
            {
                var result = passwordLogin.Invoke(_authService, new object[] { id, password });
                return result is bool ok && ok;
            }

            return _authService.Login(id);
        }

        private void LoadAccountAndEnterRoom(string id)
        {
            var save = _accountRepository.Load(id);
            _gameState.SetSaveData(save);
            _sceneFlow.Enter(ScreenType.Room);
        }

        private void SetStatus(string message, bool success)
        {
            if (statusText == null)
            {
                return;
            }

            statusText.text = message;
            statusText.color = success ? new Color(0.35f, 0.95f, 0.35f, 1f) : new Color(1f, 0.45f, 0.45f, 1f);
        }
    }
}
