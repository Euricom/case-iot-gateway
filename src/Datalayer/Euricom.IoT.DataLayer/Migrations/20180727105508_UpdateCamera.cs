using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Euricom.IoT.DataLayer.Migrations
{
    public partial class UpdateCamera : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"PRAGMA foreign_keys = 0;
 
              CREATE TABLE Devices_temp AS SELECT *
                                            FROM Devices;
              
              DROP TABLE Devices;
              
              CREATE TABLE `Devices` (
	`Address`	TEXT,
	`DropboxPath`	TEXT,
	`PollingTime`	INTEGER,
	`DanaLock_PollingTime`	INTEGER,
	`Host`	TEXT,
	`IsDimmer`	INTEGER,
	`LazyBone_PollingTime`	INTEGER,
	`Port`	INTEGER,
	`WallMountSwitch_PollingTime`	INTEGER,
	`NodeId`	INTEGER,
	`DeviceId`	TEXT NOT NULL,
	`Discriminator`	TEXT NOT NULL,
	`Enabled`	INTEGER NOT NULL,
	`Name`	TEXT,
	`MotionEyeIdentifier`	TEXT,
	`Type`	INTEGER NOT NULL,
	`Locked`	INTEGER,
	`On`	INTEGER,
	`PrimaryKey`	TEXT,
	CONSTRAINT `PK_Devices` PRIMARY KEY(`DeviceId`)
);
              
              INSERT INTO Devices 
              (
                 `Address`,
	`DropboxPath`,
	`PollingTime`,
	`DanaLock_PollingTime`,
	`Host`,
	`IsDimmer`,
	`LazyBone_PollingTime`,
	`Port`,
	`WallMountSwitch_PollingTime`,
	`NodeId`,
	`DeviceId`,
	`Discriminator`,
	`Enabled`,
	`Name`,
	`Type`,
	`Locked`,
	`On`,
	`PrimaryKey`
              )
              SELECT `Address`,
	`DropboxPath`,
	`PollingTime`,
	`DanaLock_PollingTime`,
	`Host`,
	`IsDimmer`,
	`LazyBone_PollingTime`,
	`Port`,
	`WallMountSwitch_PollingTime`,
	`NodeId`,
	`DeviceId`,
	`Discriminator`,
	`Enabled`,
	`Name`,
	`Type`,
	`Locked`,
	`On`,
	`PrimaryKey`
              FROM Devices_temp;
              
              DROP TABLE Devices_temp;
              
              PRAGMA foreign_keys = 1;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotionEyeIdentifier",
                table: "Devices");

            migrationBuilder.AddColumn<int>(
                name: "MaximumDaysAzureBlobStorage",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumDaysDropbox",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaximumStorageDropbox",
                table: "Devices",
                nullable: true);
        }
    }
}
