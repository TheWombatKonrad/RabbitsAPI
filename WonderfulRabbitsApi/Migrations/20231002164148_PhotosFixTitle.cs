using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WonderfulRabbitsApi.Migrations
{
    /// <inheritdoc />
    public partial class PhotosFixTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageTitle",
                table: "Photos",
                newName: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Photos",
                newName: "ImageTitle");
        }
    }
}
