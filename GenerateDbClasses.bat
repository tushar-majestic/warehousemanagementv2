 

dotnet ef dbcontext scaffold "Server=localhost;Database=LabMaterials;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --force --context LabDBContext --output-dir DB --context-dir DB --data-annotations --no-onconfiguring 
REM  --no-pluralize
REM dotnet new tool-manifest
REM dotnet tool install dotnet-ef
pause