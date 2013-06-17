using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    public class Icon<T>
    {
        private bool mouseDown = false;
        private bool mouseWasDragged = false;
        private int iconId;
        private Rect iconPos;
        private Action onClick;
        private GUIContent content;
        private GUIStyle iconStyle;
        private bool visible = false;

        public Icon(Rect defaultPosition, string imageFilename, string tooltip, Action onClickHandler)
        {
            Debug.Log("TAC Icon [" + this.GetHashCode().ToString("X") + "][" + Time.time + "]: Constructor: " + imageFilename);
            this.iconId = imageFilename.GetHashCode();
            this.iconPos = defaultPosition;
            this.onClick = onClickHandler;

            var texture = Utilities.LoadImage<T>(IOUtils.GetFilePathFor(typeof(T), imageFilename));
            content = (texture != null) ? new GUIContent(texture, tooltip) : new GUIContent("?", tooltip);
        }

        public void SetVisible(bool newValue)
        {
            if (newValue)
            {
                RenderingManager.AddToPostDrawQueue(3, DrawIcon);
            }
            else
            {
                RenderingManager.RemoveFromPostDrawQueue(3, DrawIcon);
            }
        }

        public bool IsVisible()
        {
            return visible;
        }

        private void DrawIcon()
        {
            GUI.skin = HighLogic.Skin;
            ConfigureStyles();

            GUI.Label(iconPos, content, iconStyle);
            HandleIconEvents();
        }

        public void Load(ConfigNode config)
        {
            iconPos.x = Utilities.GetValue(config, "icon.x", iconPos.x);
            iconPos.y = Utilities.GetValue(config, "icon.y", iconPos.y);
            iconPos = Utilities.EnsureVisible(iconPos, Math.Min(iconPos.width, iconPos.height));
            iconPos = Utilities.ClampToScreenEdge(iconPos);
        }

        public void Save(ConfigNode config)
        {
            config.AddValue("icon.x", iconPos.x);
            config.AddValue("icon.y", iconPos.y);
        }

        private void ConfigureStyles()
        {
            if (iconStyle == null)
            {
                iconStyle = new GUIStyle(GUI.skin.button);
                iconStyle.alignment = TextAnchor.MiddleCenter;
                iconStyle.padding = new RectOffset(1, 1, 1, 1);
            }
        }

        private void HandleIconEvents()
        {
            var theEvent = Event.current;
            if (theEvent != null)
            {
                if (theEvent.type == EventType.MouseDown && !mouseDown && theEvent.button == 0
                    && iconPos.Contains(theEvent.mousePosition))
                {
                    mouseDown = true;
                    theEvent.Use();
                }
                else if (theEvent.type == EventType.MouseDrag && mouseDown && theEvent.button == 0)
                {
                    mouseWasDragged = true;
                    iconPos.x += theEvent.delta.x;
                    iconPos.y += theEvent.delta.y;
                    iconPos = Utilities.EnsureVisible(iconPos, Math.Min(iconPos.width, iconPos.height));
                    theEvent.Use();
                }
                else if (theEvent.type == EventType.MouseUp && mouseDown && theEvent.button == 0)
                {
                    if (!mouseWasDragged)
                    {
                        onClick();
                    }
                    else
                    {
                        iconPos = Utilities.ClampToScreenEdge(iconPos);
                    }

                    mouseDown = false;
                    mouseWasDragged = false;
                    theEvent.Use();
                }
            }
        }
    }
}
