/**
 * Icon.cs
 * 
 * Thunder Aerospace Corporation's library for the Kerbal Space Program, by Taranis Elsu
 * 
 * (C) Copyright 2013, Taranis Elsu
 * 
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 * 
 * This code is licensed under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA 3.0)
 * creative commons license. See <http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode>
 * for full details.
 * 
 * Attribution — You are free to modify this code, so long as you mention that the resulting
 * work is based upon or adapted from this code.
 * 
 * Non-commercial - You may not use this work for commercial purposes.
 * 
 * Share Alike — If you alter, transform, or build upon this work, you may distribute the
 * resulting work only under the same or similar license to the CC BY-NC-SA 3.0 license.
 * 
 * Note that Thunder Aerospace Corporation is a ficticious entity created for entertainment
 * purposes. It is in no way meant to represent a real entity. Any similarity to a real entity
 * is purely coincidental.
 */

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

        public Icon(Rect defaultPosition, string imageFilename, string noImageText, string tooltip, Action onClickHandler)
        {
            Debug.Log("TAC Icon [" + this.GetHashCode().ToString("X") + "][" + Time.time + "]: Constructor: " + imageFilename);
            this.iconId = imageFilename.GetHashCode();
            this.iconPos = defaultPosition;
            this.onClick = onClickHandler;

            var texture = Utilities.LoadImage<T>(IOUtils.GetFilePathFor(typeof(T), imageFilename));
            content = (texture != null) ? new GUIContent(texture, tooltip) : new GUIContent(noImageText, tooltip);
        }

        public void SetVisible(bool newValue)
        {
            if (newValue)
            {
                if (!visible)
                {
                    RenderingManager.AddToPostDrawQueue(3, DrawIcon);
                }
            }
            else
            {
                if (visible)
                {
                    RenderingManager.RemoveFromPostDrawQueue(3, DrawIcon);
                }
            }

            this.visible = newValue;
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
