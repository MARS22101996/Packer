﻿using System;
using UserService.DAL.Entities;
using UserService.DAL.Interfaces;
using UserService.DAL.Repositories;

namespace UserService.DAL.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Lazy<IRepository<User>> _userRepository;

        private readonly Lazy<IRepository<Role>> _roleRepository;

        public UnitOfWork(IDbContext context)
        {
            var db = context;
            _userRepository = new Lazy<IRepository<User>>(() => new CommonRepository<User>(db));
            _roleRepository = new Lazy<IRepository<Role>>(() => new CommonRepository<Role>(db));
        }

        public IRepository<User> Users => _userRepository.Value;

        public IRepository<Role> Roles => _roleRepository.Value;
    }
}