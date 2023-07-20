using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wobalization.Migrations
{
    /// <inheritdoc />
    public partial class RenameCultureToLocale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "culture",
                table: "translation_language",
                newName: "locale");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "locale",
                table: "translation_language",
                newName: "culture");
        }
    }
}
