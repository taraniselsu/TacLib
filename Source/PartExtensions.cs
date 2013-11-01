/**
 * PartExtensions.cs
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
    public static class PartExtensions
    {
        public static double TakeResource(this Part part, string resourceName, double demand)
        {
            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition(resourceName);
            return TakeResource(part, resource, demand);
        }

        public static double TakeResource(this Part part, int resourceId, double demand)
        {
            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition(resourceId);
            return TakeResource(part, resource, demand);
        }

        public static double TakeResource(this Part part, PartResourceDefinition resource, double demand)
        {
            if (resource == null)
            {
                // TODO error!
                return 0.0;
            }

            switch (resource.resourceFlowMode)
            {
                case ResourceFlowMode.NO_FLOW:
                    return TakeResource_NoFlow(part, resource, demand);
                case ResourceFlowMode.ALL_VESSEL:
                    return TakeResource_AllVessel(part, resource, demand);
                case ResourceFlowMode.STACK_PRIORITY_SEARCH:
                    return TakeResource_StackPriority(part, resource, demand);
                case ResourceFlowMode.EVEN_FLOW:
                    Debug.LogWarning("TakeResource: ResourceFlowMode.EVEN_FLOW is not supported yet.");
                    return part.RequestResource(resource.id, demand);
                default:
                    Debug.LogWarning("TakeResource: Unknown ResourceFlowMode = " + resource.resourceFlowMode.ToString());
                    return part.RequestResource(resource.id, demand);
            }
        }

        public static double IsResourceAvailable(this Part part, string resourceName, double demand)
        {
            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition(resourceName);
            return IsResourceAvailable(part, resource, demand);
        }

        public static double IsResourceAvailable(this Part part, int resourceId, double demand)
        {
            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition(resourceId);
            return IsResourceAvailable(part, resource, demand);
        }

        public static double IsResourceAvailable(this Part part, PartResourceDefinition resource, double demand)
        {
            if (resource == null)
            {
                // TODO error!
                return 0.0;
            }

            switch (resource.resourceFlowMode)
            {
                case ResourceFlowMode.NO_FLOW:
                    return IsResourceAvailable_NoFlow(part, resource, demand);
                case ResourceFlowMode.ALL_VESSEL:
                    return IsResourceAvailable_AllVessel(part, resource, demand);
                case ResourceFlowMode.STACK_PRIORITY_SEARCH:
                    return IsResourceAvailable_StackPriority(part, resource, demand);
                case ResourceFlowMode.EVEN_FLOW:
                    Debug.LogWarning("IsResourceAvailable: ResourceFlowMode.EVEN_FLOW is not supported yet.");
                    return IsResourceAvailable_AllVessel(part, resource, demand);
                default:
                    Debug.LogWarning("IsResourceAvailable: Unknown ResourceFlowMode = " + resource.resourceFlowMode.ToString());
                    return IsResourceAvailable_AllVessel(part, resource, demand);
            }
        }

        private static double TakeResource_NoFlow(Part part, PartResourceDefinition resource, double demand)
        {
            // ignoring PartResourceDefinition.ResourceTransferMode

            if (part.Resources.Contains(resource.id))
            {
                PartResource partResource = part.Resources.Get(resource.id);
                if (partResource.flowMode == PartResource.FlowMode.None)
                {
                    // TODO warning!
                    return 0.0;
                }
                else if (!partResource.flowState)
                {
                    // Resource flow was shut off -- no warning needed
                    return 0.0;
                }
                else if (demand > 0.0)
                {
                    if (partResource.flowMode == PartResource.FlowMode.In)
                    {
                        // TODO warning!
                        return 0.0;
                    }

                    double taken = Math.Min(partResource.amount, demand);
                    partResource.amount -= taken;
                    return taken;
                }
                else
                {
                    if (partResource.flowMode == PartResource.FlowMode.Out)
                    {
                        // TODO warning!
                        return 0.0;
                    }

                    double given = Math.Min(partResource.maxAmount - partResource.amount, -demand);
                    partResource.amount += given;
                    return -given;
                }
            }
            else
            {
                return 0.0;
            }
        }

        private static double TakeResource_AllVessel(Part part, PartResourceDefinition resource, double demand)
        {
            // ignoring PartResourceDefinition.ResourceTransferMode

            var allPartResources = part.vessel.parts.Where(p => p.Resources.Contains(resource.id) &&
                                                                p.Resources.Get(resource.id).flowState == true &&
                                                                p.Resources.Get(resource.id).flowMode != PartResource.FlowMode.None
                                                          ).Select(p => p.Resources.Get(resource.id));

            if (demand > 0.0)
            {
                double leftOver = demand;

                // Should I change this to take an equal percentage of what's left in each part instead
                // of an equal amount from each part?
                var allNonEmptyPartResources = allPartResources.Where(p => p.amount > 0.0 && p.flowMode != PartResource.FlowMode.In);
                int count = allNonEmptyPartResources.Count();
                while (leftOver > 0.0 && count > 0)
                {
                    double takeFromEach = leftOver / count;
                    foreach (PartResource partResource in allNonEmptyPartResources)
                    {
                        double taken = Math.Min(partResource.amount, takeFromEach);
                        partResource.amount -= taken;
                        leftOver -= taken;
                    }

                    allNonEmptyPartResources = allNonEmptyPartResources.Where(p => p.amount > 0.0);
                    count = allNonEmptyPartResources.Count();
                }

                return demand - leftOver;
            }
            else
            {
                double leftOver = -demand;

                var allNonFullPartResources = allPartResources.Where(p => (p.maxAmount - p.amount) > 0.0 && p.flowMode != PartResource.FlowMode.Out);
                int count = allNonFullPartResources.Count();
                while (leftOver > 0.0 && count > 0)
                {
                    double giveToEach = leftOver / count;
                    foreach (PartResource partResource in allNonFullPartResources)
                    {
                        double given = Math.Min(partResource.maxAmount - partResource.amount, giveToEach);
                        partResource.amount += given;
                        leftOver -= given;
                    }

                    allNonFullPartResources = allNonFullPartResources.Where(p => (p.maxAmount - p.amount) > 0.0);
                    count = allNonFullPartResources.Count();
                }

                return demand + leftOver;
            }
        }

        private static double TakeResource_StackPriority(Part part, PartResourceDefinition resource, double demand)
        {
            // FIXME finish implementing
            return part.RequestResource(resource.id, demand);
        }

        private static double IsResourceAvailable_NoFlow(Part part, PartResourceDefinition resource, double demand)
        {
            if (part.Resources.Contains(resource.id))
            {
                PartResource partResource = part.Resources.Get(resource.id);

                if (partResource.flowMode == PartResource.FlowMode.None || partResource.flowState == false)
                {
                    return 0.0;
                }
                else if (demand > 0.0)
                {
                    if (partResource.flowMode != PartResource.FlowMode.In)
                    {
                        return Math.Min(partResource.amount, demand);
                    }
                }
                else
                {
                    if (partResource.flowMode != PartResource.FlowMode.Out)
                    {
                        return -Math.Min((partResource.maxAmount - partResource.amount), -demand);
                    }
                }
            }

            return 0.0;
        }

        private static double IsResourceAvailable_AllVessel(Part part, PartResourceDefinition resource, double demand)
        {
            var allPartResources = part.vessel.parts.Where(p => p.Resources.Contains(resource.id) &&
                                                                p.Resources.Get(resource.id).flowState == true &&
                                                                p.Resources.Get(resource.id).flowMode != PartResource.FlowMode.None
                                                          ).Select(p => p.Resources.Get(resource.id));

            if (demand > 0.0)
            {
                double amountAvailable = 0.0;

                var allNonInPartResources = allPartResources.Where(p => p.flowMode != PartResource.FlowMode.In);
                foreach (PartResource partResource in allNonInPartResources)
                {
                    amountAvailable += partResource.amount;

                    if (amountAvailable > demand)
                    {
                        return demand;
                    }
                }

                return amountAvailable;
            }
            else
            {
                double availableSpace = 0.0;

                var allNonOutPartResources = allPartResources.Where(p => p.flowMode != PartResource.FlowMode.Out);
                foreach (PartResource partResource in allNonOutPartResources)
                {
                    availableSpace += (partResource.maxAmount - partResource.amount);

                    if (availableSpace > -demand)
                    {
                        return demand;
                    }
                }

                return -availableSpace;
            }
        }

        private static double IsResourceAvailable_StackPriority(Part part, PartResourceDefinition resource, double demand)
        {
            // FIXME finish implementing
            return IsResourceAvailable_AllVessel(part, resource, demand);
        }
    }
}
