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

    using YamlDotNet.Serialization;

    /// <summary>
    /// Represents a player's group.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Gets or sets a value indicating whether group is the default.
        /// </summary>
        [YamlMember(Alias = "default")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the group inheritance.
        /// </summary>
        public List<string> Inheritance { get; set; } = new();

        /// <summary>
        /// Gets or sets the group permissions.
        /// </summary>
        public List<string> Permissions { get; set; } = new();

        /// <summary>
        /// Gets the combined permissions of the group plus all inherited groups.
        /// </summary>
        [YamlIgnore]
        [Obsolete("Nu'hu do not use this any more, look for the coolest PermisionTree. The value is still reable but writing in it do not do nothing.")]
        public List<string> CombinedPermissions { get; internal set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether the groops as <b>full permission</b>, be careful when setting it.
        /// </summary>
        [YamlIgnore]
        public bool AllMighty { get; set; } = false;

        /// <summary>
        /// Gets the permission tree of the group.
        /// </summary>
        [YamlIgnore]
        public List<PermissionNode> PermisionTree { get; internal set; } = new();
    }
}