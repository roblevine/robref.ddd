using Microsoft.EntityFrameworkCore.Migrations;
using RobRef.DDD.Domain.Users;

#nullable disable

namespace RobRef.DDD.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: $"nvarchar({UserId.Length})", maxLength: UserId.Length, nullable: false),
                    Email = table.Column<string>(type: $"nvarchar({Email.MaxLength})", maxLength: Email.MaxLength, nullable: false),
                    Title = table.Column<string>(type: $"nvarchar({Title.MaxLength})", maxLength: Title.MaxLength, nullable: true),
                    FirstName = table.Column<string>(type: $"nvarchar({FirstName.MaxLength})", maxLength: FirstName.MaxLength, nullable: false),
                    LastName = table.Column<string>(type: $"nvarchar({LastName.MaxLength})", maxLength: LastName.MaxLength, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
