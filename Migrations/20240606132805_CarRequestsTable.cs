using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ma3ak.Migrations
{
    /// <inheritdoc />
    public partial class CarRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number_Of_Passengers = table.Column<int>(type: "int", nullable: false),
                    When = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToWhere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarRequests");
        }
    }
}
