/**
 * Window.cs
 * 
 * Thunder Aerospace Corporation's Atomic Clock for the Kerbal Space Program, by Taranis Elsu
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
using System.Threading.Tasks;
using UnityEngine;

abstract class Window
{
    private bool visible = false;
    private Rect windowPos = new Rect(60, 60, 60, 60);
    private string windowTitle;
    private PartModule partModule;
    private int windowId;

    protected Window(string windowTitle, PartModule partModule)
    {
        this.windowTitle = windowTitle;
        this.partModule = partModule;
        this.windowId = windowTitle.GetHashCode() + new System.Random().Next(65536);
    }

    public bool IsVisible()
    {
        return visible;
    }

    public virtual void SetVisible(bool newValue)
    {
        if (newValue)
        {
            if (!visible)
            {
                RenderingManager.AddToPostDrawQueue(3, new Callback(CreateWindow));
            }
        }
        else
        {
            if (visible)
            {
                RenderingManager.RemoveFromPostDrawQueue(3, new Callback(CreateWindow));
            }
        }

        this.visible = newValue;
    }

    public void SetSize(int width, int height)
    {
        windowPos.width = width;
        windowPos.height = height;
    }

    public virtual void Load(ConfigNode config, string subnode)
    {
        Debug.Log("TAC Atomic Clock [" + Time.time + "]: Load " + subnode);
        if (config.HasNode(subnode))
        {
            ConfigNode windowConfig = config.GetNode(subnode);

            float newValue;
            if (windowConfig.HasValue("xPos") && float.TryParse(windowConfig.GetValue("xPos"), out newValue))
            {
                windowPos.xMin = newValue;
            }

            if (windowConfig.HasValue("yPos") && float.TryParse(windowConfig.GetValue("yPos"), out newValue))
            {
                windowPos.yMin = newValue;
            }
        }
    }

    public virtual void Save(ConfigNode config, string subnode)
    {
        Debug.Log("TAC Atomic Clock [" + Time.time + "]: Save " + subnode);

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

        windowConfig.AddValue("xPos", windowPos.xMin);
        windowConfig.AddValue("yPos", windowPos.yMin);
    }

    protected virtual void CreateWindow()
    {
        try
        {
            if (partModule.part.State != PartStates.DEAD && partModule.vessel.isActiveVessel)
            {
                GUI.skin = HighLogic.Skin;
                windowPos = GUILayout.Window(windowId, windowPos, Draw, windowTitle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            else
            {
                SetVisible(false);
            }
        }
        catch
        {
            SetVisible(false);
        }
    }

    protected abstract void Draw(int windowID);
}
