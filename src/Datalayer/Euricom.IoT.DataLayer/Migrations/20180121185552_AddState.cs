using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Euricom.IoT.DataLayer.Migrations
{
    public partial class AddState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "On",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "On",
                table: "Devices");
        }
    }
}
