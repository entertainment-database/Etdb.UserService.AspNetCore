﻿using System;
using Etdb.ServiceBase.Domain.Abstractions.Documents;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Etdb.UserService.Domain.Entities
{
    public class Email : IDocument<Guid>
    {
        public Email(Guid id, Guid userId, string address, bool isPrimary, bool isExternal)
        {
            this.Id = id;
            this.UserId = userId;
            this.Address = address;
            this.IsPrimary = isPrimary;
            this.IsExternal = isExternal;
        }

        public Guid Id { get; private set; }
        
        public Guid UserId { get; private set; }

        public string Address { get; private set; }

        public bool IsPrimary { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool IsExternal { get; private set; }
    }
}