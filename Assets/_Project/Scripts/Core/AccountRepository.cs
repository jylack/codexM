using Project.Data;
using UnityEngine;

namespace Project.Core
{
    public class AccountRepository
    {
        public SaveData Load(string accountId)
        {
            var key = Key(accountId);
            if (!PlayerPrefs.HasKey(key))
            {
                return CreateDefault(accountId);
            }

            var json = PlayerPrefs.GetString(key);
            var data = JsonUtility.FromJson<SaveData>(json);
            if (data == null)
            {
                return CreateDefault(accountId);
            }

            data.accountId = accountId;
            return data;
        }

        public void Save(SaveData saveData)
        {
            var json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(Key(saveData.accountId), json);
            PlayerPrefs.Save();
        }

        private static string Key(string accountId) => $"SAVE_{accountId}";

        private static SaveData CreateDefault(string accountId)
        {
            return new SaveData
            {
                accountId = accountId,
                gold = 100,
                premium = 10,
                currentStageId = "1"
            };
        }
    }
}
