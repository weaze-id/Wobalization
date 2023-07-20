using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wobalization.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_translation_value_language_language_id",
                table: "translation_value");

            migrationBuilder.DropTable(
                name: "language");

            migrationBuilder.RenameColumn(
                name: "language_id",
                table: "translation_value",
                newName: "translation_language_id");

            migrationBuilder.RenameIndex(
                name: "ix_translation_value_language_id",
                table: "translation_value",
                newName: "ix_translation_value_translation_language_id");

            migrationBuilder.AddColumn<long>(
                name: "translation_key_id",
                table: "translation_value",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "translation_language",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    app_id = table.Column<long>(type: "INTEGER", nullable: false),
                    culture = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    updated_at = table.Column<long>(type: "INTEGER", nullable: true),
                    deleted_at = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_translation_language", x => x.id);
                    table.ForeignKey(
                        name: "fk_translation_language_app_app_id",
                        column: x => x.app_id,
                        principalTable: "app",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_translation_value_translation_key_id",
                table: "translation_value",
                column: "translation_key_id");

            migrationBuilder.CreateIndex(
                name: "ix_translation_language_app_id",
                table: "translation_language",
                column: "app_id");

            migrationBuilder.AddForeignKey(
                name: "fk_translation_value_translation_key_translation_key_id",
                table: "translation_value",
                column: "translation_key_id",
                principalTable: "translation_key",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_translation_value_translation_language_translation_language_id",
                table: "translation_value",
                column: "translation_language_id",
                principalTable: "translation_language",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_translation_value_translation_key_translation_key_id",
                table: "translation_value");

            migrationBuilder.DropForeignKey(
                name: "fk_translation_value_translation_language_translation_language_id",
                table: "translation_value");

            migrationBuilder.DropTable(
                name: "translation_language");

            migrationBuilder.DropIndex(
                name: "ix_translation_value_translation_key_id",
                table: "translation_value");

            migrationBuilder.DropColumn(
                name: "translation_key_id",
                table: "translation_value");

            migrationBuilder.RenameColumn(
                name: "translation_language_id",
                table: "translation_value",
                newName: "language_id");

            migrationBuilder.RenameIndex(
                name: "ix_translation_value_translation_language_id",
                table: "translation_value",
                newName: "ix_translation_value_language_id");

            migrationBuilder.CreateTable(
                name: "language",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    app_id = table.Column<long>(type: "INTEGER", nullable: false),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    culture = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    deleted_at = table.Column<long>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_language", x => x.id);
                    table.ForeignKey(
                        name: "fk_language_app_app_id",
                        column: x => x.app_id,
                        principalTable: "app",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_language_app_id",
                table: "language",
                column: "app_id");

            migrationBuilder.AddForeignKey(
                name: "fk_translation_value_language_language_id",
                table: "translation_value",
                column: "language_id",
                principalTable: "language",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
