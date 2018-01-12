using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Euricom.IoT.DataLayer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Address = table.Column<string>(nullable: true),
                    DropboxPath = table.Column<string>(nullable: true),
                    MaximumDaysAzureBlobStorage = table.Column<int>(nullable: true),
                    MaximumDaysDropbox = table.Column<int>(nullable: true),
                    MaximumStorageDropbox = table.Column<double>(nullable: true),
                    PollingTime = table.Column<int>(nullable: true),
                    DanaLock_PollingTime = table.Column<int>(nullable: true),
                    Host = table.Column<string>(nullable: true),
                    IsDimmer = table.Column<bool>(nullable: true),
                    LazyBone_PollingTime = table.Column<int>(nullable: true),
                    Port = table.Column<short>(nullable: true),
                    WallMountSwitch_PollingTime = table.Column<int>(nullable: true),
                    NodeId = table.Column<byte>(nullable: true),
                    DeviceId = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AzureAccountName = table.Column<string>(nullable: true),
                    AzureIotHubUri = table.Column<string>(nullable: true),
                    AzureIotHubUriConnectionString = table.Column<string>(nullable: true),
                    AzureStorageAccessKey = table.Column<string>(nullable: true),
                    DropboxAccessToken = table.Column<string>(nullable: true),
                    GatewayDeviceKey = table.Column<string>(nullable: true),
                    GatewayName = table.Column<string>(nullable: true),
                    HistoryLog = table.Column<int>(nullable: false),
                    LogLevel = table.Column<int>(nullable: false),
                    ZWaveNetworkKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
