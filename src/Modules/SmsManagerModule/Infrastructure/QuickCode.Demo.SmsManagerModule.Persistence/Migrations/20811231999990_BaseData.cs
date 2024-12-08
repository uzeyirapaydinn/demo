using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json.Linq;
using QuickCode.Demo.Common.Extensions;
using QuickCode.Demo.SmsManagerModule.Domain.Entities;

namespace QuickCode.Demo.SmsManagerModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BaseData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var fileList = typeof(BaseData).GetMigrationDataFiles();
            migrationBuilder.ParseJsonAsInitialData(typeof(BaseDomainEntity), fileList);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
