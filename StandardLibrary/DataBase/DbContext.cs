//using Microsoft.Analytics.Interfaces;
//using Microsoft.Analytics.Types.Sql;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WBPlatform.Config;
using WBPlatform.Logging;
using WBPlatform.TableObject;
using WBPlatform.Database.Connection;

namespace WBPlatform.DataBase_ng
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext() : base() { }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DBConnectionBuilder.Connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BusReport>(e =>
            {
                e.HasIndex(x => x.ObjectId);
            });
            builder.Entity<StudentObject>(e =>
            {
                e.HasIndex(x => x.ObjectId);
            });
            builder.Entity<UserChangeRequest>(e =>
            {
                e.HasIndex(x => x.ObjectId);
            });
            builder.Entity<UserObject>(e =>
            {
                e.HasIndex(x => x.ObjectId);
            });
            builder.Entity<ClassObject>(e =>
            {
                e.HasIndex(x => x.ObjectId);
            });
            builder.Entity<SchoolBusObject>(e =>
            {
                e.HasIndex(x => x.ObjectId);
            });
            builder.Entity<ServerConfig.ServerConfigCollection.ConfigObject>(e =>
            {
                e.HasIndex(x => x.ObjectId);
            });
            builder.Entity<NotificationObject>(e =>
            {
                e.HasIndex(x => x.ObjectId);
            });
        }

        public static void EnsureCreated(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<DataBaseContext>();
            db.Database.EnsureCreated();
            L.I("Database Should be created!");
        }
        public DbSet<StudentObject> Students { get; set; }
        public DbSet<SchoolBusObject> SchoolBuses { get; set; }
        public DbSet<ClassObject> Classes { get; set; }
        public DbSet<UserObject> Users { get; set; }
        public DbSet<NotificationObject> Notifications { get; set; }
        public DbSet<BusReport> BusReports { get; set; }
        public DbSet<ServerConfig.ServerConfigCollection.ConfigObject> Configs { get; set; }
        public DbSet<UserChangeRequest> ChangeRequests { get; set; }

    }
}