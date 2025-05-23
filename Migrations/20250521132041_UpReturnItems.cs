using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    
    public partial class UpReturnItems : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InspectionNotes",
                table: "ReturnRequestItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectionNotes",
                table: "ReturnRequestItems");


        }
    }
}
