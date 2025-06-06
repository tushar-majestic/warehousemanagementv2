using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateWiseProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ⚠️ Drop only if it exists (keeps deployment idempotent)
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.PRC_GET_DATE_WISE_DATA', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PRC_GET_DATE_WISE_DATA;
");

            // 🔄 Create the new version
            migrationBuilder.Sql(@"
CREATE PROCEDURE [dbo].[PRC_GET_DATE_WISE_DATA] (  
    @PDISBURSE_DATES  VARCHAR(500) OUT,  
    @PSUPPLY_DATES    VARCHAR(500) OUT,  
    @PDISBURSE_COUNT  VARCHAR(500) OUT,  
    @PSUPPLY_COUNT    VARCHAR(500) OUT,  
    @PCODE            VARCHAR(2)   OUT,  
    @PDESC            VARCHAR(1000)OUT,  
    @PMSG             VARCHAR(2)   OUT)  
AS  
BEGIN  
    SET NOCOUNT ON;

    DECLARE @StartDate DATETIME  = DATEADD(YEAR, -2, DATEFROMPARTS(YEAR(GETDATE()), 1, 1));
    DECLARE @EndDate   DATETIME  = GETDATE();

    BEGIN TRY
        /* ─────────────── Disbursement ─────────────── */
        SELECT
            @PDISBURSE_DATES = STUFF((
                SELECT ', ' + FORMAT(MIN(ReqReceivedAt), 'MMM yyyy')
                FROM  DisbursementRequest
                WHERE ReqReceivedAt BETWEEN @StartDate AND @EndDate
                GROUP BY YEAR(ReqReceivedAt), MONTH(ReqReceivedAt)
                ORDER BY YEAR(ReqReceivedAt), MONTH(ReqReceivedAt)
                FOR XML PATH('')), 1, 2, ''),
            @PDISBURSE_COUNT = STUFF((
                SELECT ', ' + CAST(COUNT(DisbursementRequestId) AS VARCHAR)
                FROM  DisbursementRequest
                WHERE ReqReceivedAt BETWEEN @StartDate AND @EndDate
                GROUP BY YEAR(ReqReceivedAt), MONTH(ReqReceivedAt)
                ORDER BY YEAR(ReqReceivedAt), MONTH(ReqReceivedAt)
                FOR XML PATH('')), 1, 2, '');

        /* ─────────────── Supply ─────────────── */
        SELECT
            @PSUPPLY_DATES = STUFF((
                SELECT ', ' + FORMAT(MIN(ReceivedAt), 'MMM yyyy')
                FROM  Supply
                WHERE ReceivedAt BETWEEN @StartDate AND @EndDate
                GROUP BY YEAR(ReceivedAt), MONTH(ReceivedAt)
                ORDER BY YEAR(ReceivedAt), MONTH(ReceivedAt)
                FOR XML PATH('')), 1, 2, ''),
            @PSUPPLY_COUNT = STUFF((
                SELECT ', ' + CAST(COUNT(SupplyId) AS VARCHAR)
                FROM  Supply
                WHERE ReceivedAt BETWEEN @StartDate AND @EndDate
                GROUP BY YEAR(ReceivedAt), MONTH(ReceivedAt)
                ORDER BY YEAR(ReceivedAt), MONTH(ReceivedAt)
                FOR XML PATH('')), 1, 2, '');

        /* ─────────────── Status ─────────────── */
        SET @PCODE = '100';
        SET @PDESC = 'OK';
        SET @PMSG  = 'Y';
    END TRY
    BEGIN CATCH
        SET @PCODE = '99';
        SET @PDESC = CONVERT(varchar,ERROR_NUMBER()) + ' : ' + ERROR_MESSAGE();
        SET @PMSG  = 'N';
    END CATCH
END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert to the previous version (or drop)
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.PRC_GET_DATE_WISE_DATA', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PRC_GET_DATE_WISE_DATA;
");

        }
    }
}
