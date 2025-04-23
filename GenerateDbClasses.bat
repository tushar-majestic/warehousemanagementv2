 
cd C:\Rizwan\Personal\Projects\KingAbdulazizStore\LabMaterials

dotnet ef dbcontext scaffold "Data Source=10.0.0.220;Initial Catalog=LabMaterials; user id=aksa_lm; pwd=Aksa@123; encrypt=no;MultipleActiveResultSets=true;" Microsoft.EntityFrameworkCore.SqlServer --force --context LabDBContext --output-dir DB --context-dir DB --data-annotations --no-onconfiguring 
REM  --no-pluralize
REM dotnet new tool-manifest
REM dotnet tool install dotnet-ef
pause