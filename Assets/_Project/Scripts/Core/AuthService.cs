// 로그인 입력(accountId)을 검증하고 현재 로그인 계정을 보관합니다.
namespace Project.Core
{
    public class AuthService
    {
        public string CurrentAccountId { get; private set; }

        public bool Login(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return false;
            }

            CurrentAccountId = accountId.Trim();
            return true;
        }

        public void Logout()
        {
            CurrentAccountId = null;
        }
    }
}
