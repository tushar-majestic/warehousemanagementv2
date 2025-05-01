global using System.Reflection;
global using System.Diagnostics;
global using Lib;
using LabMaterials.AppCode;
using Newtonsoft.Json;
using LabMaterials.DB;
using GemBox.Spreadsheet;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        static TextLogWriter logWriter;
        public static Dictionary<string, Dictionary<string,string>> Translations { get; set; }

        public static IHostEnvironment HostingEnv { get; set; }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container ...
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddRazorPages()
                       .AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<LabDBContext>(options =>
            options.UseSqlServer("Server=localhost;Database=LabMaterials;Trusted_Connection=True;TrustServerCertificate=True;"));

            var app = builder.Build();

            

            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
                await next();
            });
            

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            Configuration = app.Configuration;
            HostingEnv = app.Environment;
            string translationsText = File.ReadAllText(app.Environment.ContentRootPath + @"\Translations.json");
            Translations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(translationsText);

            logWriter = new TextLogWriter();
            logWriter.InitializeLogWriter(app.Environment.ContentRootPath + "/Logs", "Lab", true, true, true, true);

            DefaultData defaultData = new DefaultData();
            defaultData.InsertDefaultData();
            //generateTranslationInExcel();
            app.Run();
        }

        private static void generateTranslationInExcel()
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            SpreadsheetInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;
            ExcelFile workbook = new ExcelFile();
            ExcelWorksheet worksheet = workbook.Worksheets.Add("Translation");

            // Define header values
            worksheet.Cells[0, 0].Value = "FieldName";
            worksheet.Cells[0, 1].Value = "Value_Eng";
            worksheet.Cells[0, 2].Value = "VAlue_Ara";

            // Write deserialized values to a sheet
            int row = 0;
            foreach (var translation in Translations)
            {
                worksheet.Cells[++row, 0].Value = translation.Key;
                worksheet.Cells[row, 1].Value = translation.Value["en"];
                worksheet.Cells[row, 2].Value = translation.Value["ar"];
            }

            // Save excel file
            workbook.Save("LABMaterials_Translations.xlsx");
        }
    }
}
