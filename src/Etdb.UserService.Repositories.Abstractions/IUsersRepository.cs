﻿using System;
using Etdb.ServiceBase.DocumentRepository.Abstractions;
using Etdb.UserService.Domain.Entities;

namespace Etdb.UserService.Repositories.Abstractions
{
    public interface IUsersRepository : IDocumentRepository<User, Guid>
    {
    }
}