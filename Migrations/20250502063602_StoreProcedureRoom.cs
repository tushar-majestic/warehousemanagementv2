using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class StoreProcedureRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             // Create or Alter the stored procedure
            string createProcedureSql = @"
                   ALTER PROCEDURE [dbo].[PRC_GET_STORE_DATA] (  
        @PCODE      VARCHAR(2)  OUT,  
        @PDESC      VARCHAR(1000) OUT,  
        @PMSG       VARCHAR(2)  OUT)  
        AS  
        BEGIN  
        BEGIN TRY  
        SET @PCODE = '-1'  
        SET @PDESC = 'F'  
        SET @PMSG  = 'N'  
        SELECT  
        ST.StoreId,  
        ST.StoreNumber,  
        ST.[StoreName],
        ST.StoreType,
        ST.WarehouseStatus,
		U.FullName AS WarehouseManagerName,  
        U.JobNumber AS ManagerJobNum,   KU.FullName AS KeeperName,
		KU.JobNumber AS KeeperJobNum,
        isnull(ST.isActive,1) as isActive,
        RM.RoomId,  
        RM.[ROOM_NO],  
            RM.[RoomName],RM.BuildingNumber, RM.NoOfShelves, RM.RoomDesc,RM.RoomStatus,
            STRING_AGG(ISNULL(SHL.[SHELF_NO], NULL), ', ') AS SHELF_NUMBER  
        FROM  
            [dbo].[Store] ST  
		LEFT JOIN  
			[dbo].[Users] U ON ST.WarehouseManagerID = U.UserID
        LEFT JOIN  
            [dbo].[ROOMS] RM ON ST.StoreId = RM.StoreId AND RM.Ended IS NULL 
		LEFT JOIN 
			[dbo].[Users] KU ON RM.KeeperID = KU.UserID 
        LEFT JOIN  
            [dbo].[SHELVES] SHL ON RM.RoomId = SHL.RoomId AND SHL.Ended IS NULL 
            WHERE ST.Ended IS NULL
        GROUP BY  
        ST.StoreId,  
        StoreNumber,  
            ST.[StoreName],  
        ST.isActive,  
        ST.StoreType,
		U.FullName,  
        U.JobNumber,  
        ST.WarehouseStatus,
        RM.RoomId,  
        RM.[ROOM_NO],  
            RM.[RoomName] ,
			KU.FullName, KU.JobNumber,RM.BuildingNumber, RM.NoOfShelves, RM.RoomDesc,RM.RoomStatus
        ORDER BY ST.StoreId;  
        SET @PCODE = '100'  
        SET @PDESC = 'OK'  
        SET @PMSG  = 'Y'  
        END TRY  
        BEGIN CATCH  
        SET @PCODE = '99'  
        SET @PDESC = CONVERT(varchar, ERROR_NUMBER()) + ' : ' + ERROR_MESSAGE()  
        SET @PMSG = 'N'  
        END CATCH  
        END
            ";

            migrationBuilder.Sql(createProcedureSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             // Drop the stored procedure if rolling back
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[PRC_GET_STORE_DATA]");
        }
    }
}
