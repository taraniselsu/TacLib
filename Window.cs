/**
 * Window.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

abstract class Window
{
    private bool visible = false;
    private Rect windowPos = new Rect(60, 60, 60, 60);
    private string windowTitle;
    private int windowId;

    protected Window(string windowTitle)
    {
        this.windowTitle = windowTitle;
        this.windowId = windowTitle.GetHashCode() + new System.Random().Next(65536);
    }

    public bool IsVisible()
    {
        return visible;
    }

    public virtual void SetVisible(bool newValue)
    {
        this.visible = newValue;
    }

    public void SetSize(int width, int height)
    {
        windowPos.width = width;
        windowPos.height = height;
    }

    public virtual void Load(ConfigNode config, string subnode)
    {
        if (config.HasNode(subnode))
        {
            ConfigNode windowConfig = config.GetNode(subnode);

            bool newBool;
            if (windowConfig.HasValue("visible") && bool.TryParse(windowConfig.GetValue("visible"), out newBool))
            {
                visible = newBool;
            }

            float newFloat;
            if (windowConfig.HasValue("xPos") && float.TryParse(windowConfig.GetValue("xPos"), out newFloat))
            {
                windowPos.xMin = newFloat;
            }

            if (windowConfig.HasValue("yPos") && float.TryParse(windowConfig.GetValue("yPos"), out newFloat))
            {
                windowPos.yMin = newFloat;
            }
        }
    }

    public virtual void Save(ConfigNode config, string subnode)
    {
        ConfigNode windowConfig;
        if (config.HasNode(subnode))
        {
            windowConfig = config.GetNode(subnode);
        }
        else
        {
            windowConfig = new ConfigNode(subnode);
            config.AddNode(windowConfig);
        }

        windowConfig.AddValue("visible", visible);
        windowConfig.AddValue("xPos", windowPos.xMin);
        windowConfig.AddValue("yPos", windowPos.yMin);
    }

    public void OnGUI()
    {
        if (visible)
        {
            GUI.skin = HighLogic.Skin;
            windowPos = GUILayout.Window(windowId, windowPos, Draw, windowTitle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }
    }

    protected abstract void Draw(int windowID);
}
