﻿// Copyright (c) Clickberry, Inc. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3. See License.txt in the project root for license information.

namespace MongoMigrations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MigrationLocator
    {
        protected readonly List<Assembly> Assemblies = new List<Assembly>();
        public List<MigrationFilter> MigrationFilters = new List<MigrationFilter>();

        public MigrationLocator()
        {
            MigrationFilters.Add(new ExcludeExperimentalMigrations());
        }

        public virtual void LookForMigrationsInAssemblyOfType<T>()
        {
            var assembly = typeof (T).Assembly;
            LookForMigrationsInAssembly(assembly);
        }

        public void LookForMigrationsInAssembly(Assembly assembly)
        {
            if (Assemblies.Contains(assembly))
            {
                return;
            }
            Assemblies.Add(assembly);
        }

        public virtual IEnumerable<Migration> GetAllMigrations()
        {
            return Assemblies
                .SelectMany(GetMigrationsFromAssembly)
                .OrderBy(m => m.Version);
        }

        protected virtual IEnumerable<Migration> GetMigrationsFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes()
                    .Where(t => typeof (Migration).IsAssignableFrom(t) && !t.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .OfType<Migration>()
                    .Where(m => !MigrationFilters.Any(f => f.Exclude(m)));
            }
            catch (Exception exception)
            {
                throw new MigrationException("Cannot load migrations from assembly: " + assembly.FullName, exception);
            }
        }

        public virtual Version LatestVersion()
        {
            if (!GetAllMigrations().Any())
            {
                return new Version();
            }
            return GetAllMigrations()
                .Max(m => m.Version);
        }

        public virtual IEnumerable<Migration> GetMigrationsAfter(AppliedMigration currentVersion)
        {
            var migrations = GetAllMigrations();

            if (currentVersion != null)
            {
                migrations = migrations.Where(m => m.Version > currentVersion.Version);
            }

            return migrations.OrderBy(m => m.Version);
        }
    }
}