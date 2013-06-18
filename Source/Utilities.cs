/**
 * Utilities.cs
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
    public class Utilities
    {
        public static Rect EnsureVisible(Rect pos, float min = 16.0f)
        {
            if ((pos.x + pos.width) < min)
            {
                pos.x = min - pos.width;
            }
            if (pos.x > (Screen.width - min))
            {
                pos.x = Screen.width - min;
            }
            if ((pos.y + pos.height) < min)
            {
                pos.y = min - pos.height;
            }
            if (pos.y > (Screen.height - min))
            {
                pos.y = Screen.height - min;
            }

            return pos;
        }

        public static Rect ClampToScreenEdge(Rect pos)
        {
            float topSeparation = Math.Abs(pos.y);
            float bottomSeparation = Math.Abs(Screen.height - pos.y - pos.height);
            float leftSeparation = Math.Abs(pos.x);
            float rightSeparation = Math.Abs(Screen.width - pos.x - pos.width);

            if (topSeparation <= bottomSeparation && topSeparation <= leftSeparation && topSeparation <= rightSeparation)
            {
                pos.y = 0;
            }
            else if (leftSeparation <= topSeparation && leftSeparation <= bottomSeparation && leftSeparation <= rightSeparation)
            {
                pos.x = 0;
            }
            else if (bottomSeparation <= topSeparation && bottomSeparation <= leftSeparation && bottomSeparation <= rightSeparation)
            {
                pos.y = Screen.height - pos.height;
            }
            else if (rightSeparation <= topSeparation && rightSeparation <= bottomSeparation && rightSeparation <= leftSeparation)
            {
                pos.x = Screen.width - pos.width;
            }

            return pos;
        }

        public static Texture2D LoadImage<T>(string filename)
        {
            if (File.Exists<T>(filename))
            {
                var bytes = File.ReadAllBytes<T>(filename);
                Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false);
                texture.LoadImage(bytes);
                return texture;
            }
            else
            {
                return null;
            }
        }

        public static bool GetValue(ConfigNode config, string name, bool currentValue)
        {
            bool newValue;
            if (config.HasValue(name) && bool.TryParse(config.GetValue(name), out newValue))
            {
                return newValue;
            }
            else
            {
                return currentValue;
            }
        }

        public static float GetValue(ConfigNode config, string name, float currentValue)
        {
            float newValue;
            if (config.HasValue(name) && float.TryParse(config.GetValue(name), out newValue))
            {
                return newValue;
            }
            else
            {
                return currentValue;
            }
        }

        public static double GetValue(ConfigNode config, string name, double currentValue)
        {
            double newValue;
            if (config.HasValue(name) && double.TryParse(config.GetValue(name), out newValue))
            {
                return newValue;
            }
            else
            {
                return currentValue;
            }
        }
    }
}
