// -----------------------------------------------------------------------
// <copyright file="Group.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Features
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a node in the permission tree.
    /// <para>
    /// In the follwong permision config: 'a.b.c'
    /// 'a' is the root node, 'b' is a child of 'a' and 'c' is a child of 'b'.
    /// </para>
    /// </summary>
    public class PermissionNode
    {
        private const string AllPermisionString = "*";

        // Also permission with sapce is valid, maybe not a good idea but valid.
        // It was the case before the regex was added.
        private static readonly Regex PermisionLigneRegex = new(@"
^                                       # assert position at the beginning of the string
(?<perm>[^.]+)                          # captur in group perm any string until a dot (.)
(?:.(?<perm>[^.]+))*                    # captur in group perm any string starting by a dot (.)
$                                       # assert position at the end of the string
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);

        private readonly List<PermissionNode> childs = new List<PermissionNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionNode"/> class.
        /// <see cref="Name"/> is set to an empty string and <see cref="AllPermsions"/> is set to false.
        /// </summary>
        public PermissionNode()
            : this(string.Empty, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionNode"/> class.
        /// </summary>
        /// <param name="name">The name of the permission.</param>
        /// <param name="allPermsion">
        /// If set to <see langword="true"/> that equivalent to having '*' in the config.
        /// </param>
        public PermissionNode(string name, bool allPermsion)
        {
            Name = name;
            AllPermsions = allPermsion;
        }

        /// <summary>
        /// Gets or sets the name of the permission.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the permission is also incarnating all sub-permissions.
        /// </summary>
        public bool AllPermsions { get; set; }

#pragma warning disable SA1629 // Documentation text should end with a period, I FINISH THE LIGNE WITH "!"
        /// <summary>
        /// Builds a permission tree from a list of permissions. Any invalid permission will be ignored.
        /// <para>
        /// <b>Anything that follow '*' is ignored and treated as if it was full permission.
        /// Putting space at the start or the end is ignored, else it will be considerated as part of the permission!</b>
        /// </para>
        /// <para>
        /// Ex: 'a.b.c' is a permission with 'a' an root node of <paramref name="tree"/>, 'b' as a child of 'a' and 'c' as a child of 'b'.
        /// A new ligne with this same shema will be a new element in the resulting tree. Any common node will be merged.
        /// </para>
        /// <para>
        /// Ex: 'a.*' is a permission with 'a' an root node of <paramref name="tree"/> with <see cref="AllPermsions"/> set to <see langword="true"/>.
        /// </para>
        /// <para>
        /// Ex: '*' mean that <paramref name="allMighty"/> will be set to <see langword="true"/> .
        /// </para>
        /// </summary>
        /// <param name="permissions">The string that represent a list of permission.</param>
        /// <param name="tree">The resulting tree.</param>
        /// <param name="allMighty"><see langword="true"/> if the user as full power.</param>
#pragma warning restore SA1629 // Documentation text should end with a period
        public static void BuildTree(List<string> permissions, out List<PermissionNode> tree, out bool allMighty)
        {
            tree = new List<PermissionNode>();
            allMighty = false;
            if (permissions is null || permissions.Count == 0)
                return;

            foreach (string rawPermission in permissions)
            {
                Match match = PermisionLigneRegex.Match(rawPermission.Trim());
                if (!match.Success || match.Groups["perm"].Captures.Count == 0)
                    continue;

                PermissionNode branch = null;
                foreach (System.Text.RegularExpressions.Group permission in match.Groups["perm"].Captures)
                {
                    if (branch is null)
                    {
                        if (permission.Value == AllPermisionString)
                        {
                            allMighty = true;
                            break;
                        }

                        branch = tree.Find(node => node.Name == permission.Value);
                        if (branch is null)
                        {
                            branch = new PermissionNode(permission.Value, false);
                            tree.Add(branch);
                        }
                    }
                    else
                    {
                        if (permission.Value == AllPermisionString)
                        {
                            branch.AllPermsions = true;
                            break;
                        }

                        PermissionNode leaf = branch.childs.Find(node => node.Name == permission.Value);
                        if (leaf is not null)
                        {
                            branch = leaf;
                            continue;
                        }

                        leaf = new PermissionNode(permission.Value, false);
                        branch.childs.Add(leaf);
                    }
                }
            }
        }

        /// <summary>
        /// Merges two permission trees.
        /// </summary>
        /// <param name="inTo">The tree to merge into, new element will be added.</param>
        /// <param name="toMerge">The tree to merge.</param>
        public static void MergeTrees(List<PermissionNode> inTo, List<PermissionNode> toMerge)
        {
            foreach (PermissionNode node in toMerge)
            {
                PermissionNode existingNode = inTo.Find(n => n.Name == node.Name);
                if (existingNode is null)
                {
                    inTo.Add(node);
                    continue;
                }

                if (node.AllPermsions)
                    existingNode.AllPermsions = true;

                MergeTrees(existingNode.childs, node.childs);
            }
        }

        /// <summary>
        /// Gets the node with the specified <paramref name="name"/>, if it exists.
        /// </summary>
        /// <param name="name">The name of the target node.</param>
        /// <param name="node">The resulting node, <see langword="null"/> if returned <see langword="false"/>.</param>
        /// <returns>Return <see langword="true"/> if the node was found else <see langword="false"/>.</returns>
        public bool TryGetSubNode(string name, out PermissionNode node)
        {
            foreach (PermissionNode child in childs)
            {
                if (child.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    node = child;
                    return true;
                }
            }

            node = null;
            return false;
        }

        /// <summary>
        /// Adds a sub-node to this node.
        /// The added node can't be this node or one of its children.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <exception cref="Exception">Throw if a circular reference is created.</exception>
        public void AddSubNode(PermissionNode node)
        {
            if (!SubNodeValidity(node))
                throw new Exception("Can't add a node as a child of itself or one of its childrens.");

            childs.Add(node);
        }

        /// <summary>
        /// Adds all the permissions of <paramref name="node"/> to this node.
        /// </summary>
        /// <param name="node">The node that get is childs merged with <see langword="this"/> node.</param>
        public void MergeSubNode(PermissionNode node)
        {
            if (node.AllPermsions)
                AllPermsions = true;

            foreach (PermissionNode child in node.childs)
            {
                if (TryGetSubNode(child.Name, out PermissionNode existingChild))
                    existingChild.MergeSubNode(child);
                else
                    AddSubNode(child);
            }
        }

        private bool SubNodeValidity(PermissionNode node)
        {
            return node.childs.TrueForAll(child => child != this && child.SubNodeValidity(this));
        }
    }
}