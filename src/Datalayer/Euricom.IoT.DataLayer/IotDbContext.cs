using System;
using Euricom.IoT.Devices.Camera;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Devices.LazyBone;
using Euricom.IoT.Devices.WallMountSwitch;
using Euricom.IoT.Devices.ZWave;
using Euricom.IoT.Models;
using Microsoft.EntityFrameworkCore;

namespace Euricom.IoT.DataLayer
{
    public class IotDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<ZWaveDevice> ZWaveDevices { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<DanaLock> DanaLocks { get; set; }
        public DbSet<WallMountSwitch> WallMountSwitches { get; set; }
        public DbSet<LazyBone> LazyBones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=euricom.iot.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Settings>().HasKey(c => c.Id);
            modelBuilder.Entity<User>().HasKey(c => c.Username);
            modelBuilder.Entity<Device>().HasKey(c => c.DeviceId);
        }
    }
}