using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public static class UIFactory
    {
        public static GameObject Panel(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var image = go.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.7f);
            return go;
        }

        public static Text Text(Transform parent, string text, Vector2 anchoredPos)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.text = text;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(900, 70);
            rt.anchoredPosition = anchoredPos;
            return t;
        }

        public static Button Button(Transform parent, string label, Vector2 anchoredPos, Action onClick)
        {
            var go = new GameObject($"Btn_{label}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(250, 60);
            rt.anchoredPosition = anchoredPos;

            var txt = Text(go.transform, label, Vector2.zero);
            txt.alignment = TextAnchor.MiddleCenter;
            txt.rectTransform.sizeDelta = rt.sizeDelta;

            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() => onClick?.Invoke());
            return button;
        }

        public static InputField Input(Transform parent, string placeholder, Vector2 anchoredPos)
        {
            var go = new GameObject("Input", typeof(RectTransform), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(350, 60);
            rt.anchoredPosition = anchoredPos;
            go.GetComponent<Image>().color = Color.white;

            var textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(go.transform, false);
            var text = textObj.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleLeft;
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(10, 6);
            text.rectTransform.offsetMax = new Vector2(-10, -6);

            var placeholderObj = new GameObject("Placeholder", typeof(RectTransform), typeof(Text));
            placeholderObj.transform.SetParent(go.transform, false);
            var ph = placeholderObj.GetComponent<Text>();
            ph.font = text.font;
            ph.text = placeholder;
            ph.fontStyle = FontStyle.Italic;
            ph.color = Color.gray;
            ph.rectTransform.anchorMin = Vector2.zero;
            ph.rectTransform.anchorMax = Vector2.one;
            ph.rectTransform.offsetMin = new Vector2(10, 6);
            ph.rectTransform.offsetMax = new Vector2(-10, -6);

            var input = go.GetComponent<InputField>();
            input.textComponent = text;
            input.placeholder = ph;
            return input;
        }
    }
}
