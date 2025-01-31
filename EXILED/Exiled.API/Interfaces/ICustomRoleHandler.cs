// -----------------------------------------------------------------------
// <copyright file="ICustomRoleHandler.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.API.Interfaces
{
    /// <summary>
    /// Handle the spawn, despawn and role id of a custom role.
    /// Use it with <see cref="Features.Roles.CustomRole"/>.
    /// </summary>
    public interface ICustomRoleHandler
    {
        /// <summary>
        /// Gets the role id of the custom role.
        /// Must be unique foreach custom role.
        /// </summary>
        string RoleId { get; }

        /// <summary>
        /// Called when the custom role get spawned.
        /// </summary>
        void Spawned();

        /// <summary>
        /// Called when the custom role get despawned.
        /// </summary>
        void Despawned();
    }
}
