using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class UpReturnItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InspectionNotes",
                table: "ReturnRequestItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

<<<<<<< HEAD
=======

>>>>>>> 54fa3a534ef979c31b1e182e1a5589b56b682438
            // migrationBuilder.AddColumn<string>(
            //     name: "RecommendedAction",
            //     table: "ReturnRequestItems",
            //     type: "nvarchar(max)",
            //     nullable: true);
<<<<<<< HEAD
=======

>>>>>>> 54fa3a534ef979c31b1e182e1a5589b56b682438
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectionNotes",
                table: "ReturnRequestItems");

<<<<<<< HEAD
            // migrationBuilder.DropColumn(
            //     name: "RecommendedAction",
            //     table: "ReturnRequestItems");
=======

            // migrationBuilder.DropColumn(
            //     name: "RecommendedAction",
            //     table: "ReturnRequestItems");

>>>>>>> 54fa3a534ef979c31b1e182e1a5589b56b682438
        }
    }
}
