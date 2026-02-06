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
    }
}
