// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using PlayerRoles;

    /// <summary>
    /// Represents a custom role.
    /// Use for API to help developers create custom roles and not mess with fully vanila focus role.
    /// </summary>
    public class CustomRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRole"/> class.
        /// </summary>
        /// <param name="baseRole">The role sync with the client.</param>
        /// <param name="handler">The role handle, containing the data and logic for the client side.</param>
        internal CustomRole(Role baseRole, ICustomRoleHandler handler)
            : base(baseRole.Base)
        {
            Handler = handler;
            BaseRole = baseRole;
            handler.Spawned();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRole"/> class.
        /// </summary>
        /// <param name="baseRole">The role sync with the client. Not keeped for long just to match the base ctor.</param>
        /// <param name="handler">The role handle, containing the data and logic for the client side.</param>
        /// <param name="roleType">The new role type when spawned.</param>
        internal CustomRole(Role baseRole, ICustomRoleHandler handler, RoleTypeId roleType)
            : base(baseRole.Base)
        {
            Handler = handler;
            BaseRole = baseRole;
            SetBaseRole(roleType);
            handler.Spawned();
        }

        /// <summary>
        /// Gets the base role.
        /// The role sync with the client.
        /// It henerite all of the property of the role.
        /// </summary>
        public Role BaseRole { get; private set; }

        /// <inheritdoc/>
        public override RoleTypeId Type => RoleTypeId.CustomRole;

        /// <summary>
        /// Gets the role id.
        /// Same role id mean, same role.
        /// </summary>
        public string CustomRoleId => Handler.RoleId;

        /// <summary>
        /// Gets the role handler.
        /// </summary>
        public ICustomRoleHandler Handler { get; }

        /// <summary>
        /// Change the base role to an other one.
        /// </summary>
        /// <param name="role">The new base role.</param>
        /// <param name="reason">The reason of this change.</param>
        public void SetBaseRole(RoleTypeId role, SpawnReason reason = Enums.SpawnReason.ForceClass)
        {
            // TODO: Check if this call trigger some spawn event
            Owner.RoleManager.ServerSetRole(role, (RoleChangeReason)reason, RoleSpawnFlags.None);
            BaseRole = Owner.Role;
            Owner.Role = this;
        }

        /// <inheritdoc/>
        public override void Set(RoleTypeId newRole, SpawnReason reason, RoleSpawnFlags spawnFlags)
        {
            base.Set(newRole, reason, spawnFlags);
            Handler.Despawned();
        }
    }
}
