// TMP 유무에 따라 텍스트/입력 컴포넌트를 공통 API로 다루는 추상화 레이어입니다.
using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

namespace Project.UI
{
    public sealed class UITextRef
    {
#if TMP_PRESENT
        private readonly TMP_Text _text;

        public UITextRef(TMP_Text text)
        {
            _text = text;
        }
#else
        private readonly Text _text;

        public UITextRef(Text text)
        {
            _text = text;
        }
#endif

        public string text
        {
            get => _text.text;
            set => _text.text = value;
        }

        public RectTransform rectTransform => _text.rectTransform;

        public TextAnchor alignment
        {
#if TMP_PRESENT
            get => ToTextAnchor(_text.alignment);
            set => _text.alignment = ToTmpAlignment(value);
#else
            get => _text.alignment;
            set => _text.alignment = value;
#endif
        }

#if TMP_PRESENT
        private static TextAnchor ToTextAnchor(TextAlignmentOptions alignment)
        {
            switch (alignment)
            {
                case TextAlignmentOptions.TopLeft: return TextAnchor.UpperLeft;
                case TextAlignmentOptions.Top: return TextAnchor.UpperCenter;
                case TextAlignmentOptions.TopRight: return TextAnchor.UpperRight;
                case TextAlignmentOptions.Left: return TextAnchor.MiddleLeft;
                case TextAlignmentOptions.Right: return TextAnchor.MiddleRight;
                case TextAlignmentOptions.BottomLeft: return TextAnchor.LowerLeft;
                case TextAlignmentOptions.Bottom: return TextAnchor.LowerCenter;
                case TextAlignmentOptions.BottomRight: return TextAnchor.LowerRight;
                default: return TextAnchor.MiddleCenter;
            }
        }

        private static TextAlignmentOptions ToTmpAlignment(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
                case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
                case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
                case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
                case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
                default: return TextAlignmentOptions.Center;
            }
        }
#endif
    }

    public sealed class UIInputRef
    {
#if TMP_PRESENT
        private readonly TMP_InputField _input;

        public UIInputRef(TMP_InputField input)
        {
            _input = input;
        }
#else
        private readonly InputField _input;

        public UIInputRef(InputField input)
        {
            _input = input;
        }
#endif

        public string text
        {
            get => _input.text;
            set => _input.text = value;
        }
    }
}
