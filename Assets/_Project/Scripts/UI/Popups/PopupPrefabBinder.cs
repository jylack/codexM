using UnityEngine;

namespace Project.UI
{
    public static class PopupPrefabBinder
    {
        private const string PrefabRootPath = "_Project/Prefabs/UI";

        public static GameObject CreateOrFallback(Transform parent, string prefabName)
        {
            var prefab = Resources.Load<GameObject>($"{PrefabRootPath}/{prefabName}");
            if (prefab != null)
            {
                return Object.Instantiate(prefab, parent);
            }

            var panel = UIFactory.Panel(parent, prefabName);
            var block = panel.GetComponent<UnityEngine.UI.Image>();
            block.color = new Color(0f, 0f, 0f, 0.85f);
            return panel;
        }
    }
}
