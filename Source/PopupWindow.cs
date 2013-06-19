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
        private Action<int> callback;

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
                popupPos = GUILayout.Window(windowId, popupPos, DrawPopupContents, "");
            }
        }

        private void DrawPopupContents(int windowId)
        {
            GUI.BringWindowToFront(windowId);

            if (Event.current.rawType == EventType.MouseUp)
            {
                showPopup = false;
            }

            callback(windowId);
        }

        public static void Draw(string buttonText, Rect windowPos, Action<int> popupDrawCallback)
        {
            PopupWindow pw = PopupWindow.GetInstance();

            var content = new GUIContent(buttonText);
            var rect = GUILayoutUtility.GetRect(content, GUI.skin.button);
            if (GUI.Button(rect, content))
            {
                pw.showPopup = true;
                // pw.popupPos = new Rect(windowPos.x + rect.xMin, windowPos.y + rect.yMax + 1, 10, 10);
                pw.popupPos = new Rect(windowPos.x + 40, windowPos.y + 40, 10, 10);
                pw.callback = popupDrawCallback;
            }

            if (Event.current.rawType == EventType.MouseUp)
            {
                pw.showPopup = false;
            }
        }
    }
}
