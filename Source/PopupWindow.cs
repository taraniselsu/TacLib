using System;
using UnityEngine;

namespace Tac
{
    public class PopupWindow : MonoBehaviour
    {
        private static GameObject go;
        private static PopupWindow instance;
        private int windowId;
        private bool showPopup;
        private Rect popupPos;
        private Func<int, object, bool> callback;
        private object parameter;

        private static PopupWindow GetInstance()
        {
            if (go == null)
            {
                go = new GameObject("TacPopupWindow");
                instance = go.AddComponent<PopupWindow>();
            }
            return instance;
        }

        void Awake()
        {
            instance = this;
            windowId = "Tac.PopupWindow".GetHashCode();
            showPopup = false;
        }

        void OnGUI()
        {
            if (showPopup)
            {
                GUI.skin = HighLogic.Skin;
                popupPos = Utilities.EnsureCompletelyVisible(popupPos);
                popupPos = GUILayout.Window(windowId, popupPos, DrawPopupContents, "");
            }
        }

        private void DrawPopupContents(int windowId)
        {
            GUI.BringWindowToFront(windowId);

            bool shouldClose = callback(windowId, parameter);

            if (shouldClose)
            {
                showPopup = false;
            }
        }

        public static void Draw(string buttonText, Rect windowPos, Func<int, object, bool> popupDrawCallback, GUIStyle buttonStyle, object parameter, params GUILayoutOption[] options)
        {
            PopupWindow pw = PopupWindow.GetInstance();

            var content = new GUIContent(buttonText);
            var rect = GUILayoutUtility.GetRect(content, buttonStyle, options);
            if (GUI.Button(rect, content, buttonStyle))
            {
                pw.showPopup = true;

                // pw.popupPos = new Rect(windowPos.x + rect.xMin, windowPos.y + rect.yMax + 1, 10, 10);
                var mouse = Input.mousePosition;
                pw.popupPos = new Rect(mouse.x - 10, Screen.height - mouse.y - 10, 10, 10);

                pw.callback = popupDrawCallback;
                pw.parameter = parameter;
            }

            if (Event.current.rawType == EventType.MouseUp)
            {
                pw.showPopup = false;
            }
        }
    }
}
