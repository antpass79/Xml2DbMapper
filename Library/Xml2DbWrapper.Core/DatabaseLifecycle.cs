﻿using Microsoft.EntityFrameworkCore;
using System;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core
{
    public class DatabaseLifecycle : IDatabaseLifecycle
    {
        #region Data Members

        private readonly FeaturesContext _context;

        #endregion

        #region Constructors

        public DatabaseLifecycle(DbContextOptions<FeaturesContext> options)
        {
            _context = new FeaturesContext(options);
        }

        #endregion

        #region IDatabaseLifecycle Interface

        public bool Created { get; private set; } = false;

        public IDatabaseLifecycle Scope()
        {
            Created = _context.Database.EnsureCreated();
            return this;
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Delete();
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Private Functions

        bool Delete()
        {
            var result = _context.Database.EnsureDeleted();
            Created = false;
            return result;
        }

        #endregion
    }
}
