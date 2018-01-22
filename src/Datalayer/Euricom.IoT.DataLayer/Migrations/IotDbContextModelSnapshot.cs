﻿// <auto-generated />
using Euricom.IoT.DataLayer;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace Euricom.IoT.DataLayer.Migrations
{
    [DbContext(typeof(IotDbContext))]
    partial class IotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Euricom.IoT.Models.Device", b =>
                {
                    b.Property<string>("DeviceId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name");

                    b.Property<int>("Type");

                    b.HasKey("DeviceId");

                    b.ToTable("Devices");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Device");
                });

            modelBuilder.Entity("Euricom.IoT.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AzureAccountName");

                    b.Property<string>("AzureIotHubUri");

                    b.Property<string>("AzureIotHubUriConnectionString");

                    b.Property<string>("AzureStorageAccessKey");

                    b.Property<string>("DropboxAccessToken");

                    b.Property<string>("GatewayDeviceKey");

                    b.Property<string>("GatewayName");

                    b.Property<int>("HistoryLog");

                    b.Property<int>("LogLevel");

                    b.Property<string>("ZWaveNetworkKey");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Euricom.IoT.Models.User", b =>
                {
                    b.Property<string>("Username")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Hash");

                    b.Property<string>("Salt");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Euricom.IoT.Devices.Camera.Camera", b =>
                {
                    b.HasBaseType("Euricom.IoT.Models.Device");

                    b.Property<string>("Address");

                    b.Property<string>("DropboxPath");

                    b.Property<int>("MaximumDaysAzureBlobStorage");

                    b.Property<int>("MaximumDaysDropbox");

                    b.Property<double>("MaximumStorageDropbox");

                    b.Property<int>("PollingTime");

                    b.ToTable("Camera");

                    b.HasDiscriminator().HasValue("Camera");
                });

            modelBuilder.Entity("Euricom.IoT.Devices.LazyBone.LazyBone", b =>
                {
                    b.HasBaseType("Euricom.IoT.Models.Device");

                    b.Property<string>("Host");

                    b.Property<bool>("IsDimmer");

                    b.Property<int>("PollingTime")
                        .HasColumnName("LazyBone_PollingTime");

                    b.Property<short>("Port");

                    b.ToTable("LazyBone");

                    b.HasDiscriminator().HasValue("LazyBone");
                });

            modelBuilder.Entity("Euricom.IoT.Devices.ZWave.ZWaveDevice", b =>
                {
                    b.HasBaseType("Euricom.IoT.Models.Device");

                    b.Property<byte>("NodeId");

                    b.ToTable("ZWaveDevice");

                    b.HasDiscriminator().HasValue("ZWaveDevice");
                });

            modelBuilder.Entity("Euricom.IoT.Devices.DanaLock.DanaLock", b =>
                {
                    b.HasBaseType("Euricom.IoT.Devices.ZWave.ZWaveDevice");

                    b.Property<bool>("Locked");

                    b.Property<int>("PollingTime")
                        .HasColumnName("DanaLock_PollingTime");

                    b.ToTable("DanaLock");

                    b.HasDiscriminator().HasValue("DanaLock");
                });

            modelBuilder.Entity("Euricom.IoT.Devices.WallMountSwitch.WallMountSwitch", b =>
                {
                    b.HasBaseType("Euricom.IoT.Devices.ZWave.ZWaveDevice");

                    b.Property<bool>("On");

                    b.Property<int>("PollingTime")
                        .HasColumnName("WallMountSwitch_PollingTime");

                    b.ToTable("WallMountSwitch");

                    b.HasDiscriminator().HasValue("WallMountSwitch");
                });
#pragma warning restore 612, 618
        }
    }
}
