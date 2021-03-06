﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a Home Assistant user.
    /// </summary>
    public class User : RegistryEntryBase
    {
        private readonly ModifiableProperty<string> name = new ModifiableProperty<string>(nameof(Name));

        private readonly ModifiablePropertyCollection<HashSet<string>, string> groupIds = new ModifiablePropertyCollection<HashSet<string>, string>(nameof(GroupIds));

        /// <summary>
        /// The System Administrator group id constant.
        /// </summary>
        public const string SYSADMIN_GROUP_ID = "system-admin";

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <summary>
        /// Gets the ID of this user.
        /// </summary>
        [JsonProperty]
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the name of this user.
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get => this.name.Value;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException($"'{nameof(this.Name)}' cannot be null or whitespace.");
                }

                this.name.Value = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is owner of the system. In this case, the user will have full access to everything.
        /// </summary>
        [JsonProperty]
        public bool IsOwner { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is active.
        /// </summary>
        [JsonProperty]
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is administrator.
        /// </summary>
        [JsonIgnore]
        public bool IsAdministrator
        {
            get => this.GroupIds?.Contains(SYSADMIN_GROUP_ID) == true;
            set
            {
                if (value)
                {
                    this.GroupIds.Add(SYSADMIN_GROUP_ID);
                }
                else
                {
                    this.GroupIds.Remove(SYSADMIN_GROUP_ID);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user has been generated automatically by the system.
        /// </summary>
        [JsonProperty]
        public bool SystemGenerated { get; private set; }

        /// <summary>
        /// Gets a set of group ids where the user is included.
        /// </summary>
        [JsonProperty]
        public HashSet<string> GroupIds
        {
            get => this.groupIds.Value;
        }

        /// <summary>
        /// Gets the credentials of this user.
        /// </summary>
        [JsonProperty]
        public JRaw Credentials { get; private set; }

        [JsonConstructor]
        private User()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="groupIds">The group ids where the user is included.</param>
        public User(string name, IEnumerable<string> groupIds = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
            if (groupIds != null)
            {
                foreach (var item in groupIds)
                {
                    this.groupIds.Value.Add(item);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="isAdministrator">A value indicating is the user will be included in the <see cref="SYSADMIN_GROUP_ID"/>.</param>
        public User(string name, bool isAdministrator)
            : this(name)
        {
            this.IsAdministrator = isAdministrator;
        }

        // Used for testing purposes.
        internal static User CreateUnmodified(string name, bool isOwner)
        {
            var result = new User(name, isOwner)
            {
                IsOwner = isOwner,
            };
            result.SaveChanges();

            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            yield return this.name;
            yield return this.groupIds;
        }

        internal void SetIsActive(bool value)
        {
            this.IsActive = value;
        }

        /// <summary>
        /// Method used by the serializer to determine if the <see cref="GroupIds"/> property should be serialized.
        /// </summary>
        /// <returns><see langword="true"/> if the property should be serialized; otherwise, <see langword="false"/>.</returns>
        protected bool ShouldSerializeGroupIds() => this.GroupIds?.Count > 0;

        /// <inheritdoc />
        public override string ToString() => $"{nameof(User)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is User user &&
                   this.Id == user.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }
    }
}
