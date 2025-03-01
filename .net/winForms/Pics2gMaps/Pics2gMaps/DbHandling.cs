using Common;
using Microsoft.EntityFrameworkCore;
using ImageHandling;
using Microsoft.Extensions.Configuration;

namespace Pics2gMaps;

public class DbHandling:ICommandHandlerAsync<DbHandlingCommand>
{
    private const string tableName = "GpsInfoGallery";

    static void UpsertRecord(AppDbContext context, LatLngFileNameModel latLngFileNameModel)
    {
        using var context1 = new AppDbContext();
        context1.Database.EnsureCreated();
        var existingRecord = context1.GpsInfoGallery.FirstOrDefault(latLngFileName => latLngFileName.FileName == latLngFileNameModel.FileName);

        if (existingRecord != null)
        {
            // Update existing record
            existingRecord.FileName = latLngFileNameModel.FileName;
            context1.Update(existingRecord);
        }
        else
        {
            // Insert new record
            context1.Add(new LatLngFileNameModel { FileName = latLngFileNameModel.FileName
                , Latitude = latLngFileNameModel.Latitude
                , Longitude = latLngFileNameModel.Longitude
            });
        }

        context1.SaveChanges();
    }

    public async Task Execute(DbHandlingCommand command)
    {
        await Task.Run(() =>
        {
            AppDbContext context = new();
            UpsertRecord(context, command.LatLngFileNameModel);
        });
    }

    public class AppDbContext : DbContext
    {
        public DbSet<LatLngFileNameModel> GpsInfoGallery { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Basisverzeichnis setzen
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            options.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LatLngFileNameModel>().ToTable(tableName);
        }

        public void Init() => Database.EnsureCreated();

    }

    public static async Task EnsureTableExists()
    {
        await using var dbContext = new AppDbContext();
        var tableExists = await dbContext.Database.ExecuteSqlRawAsync(
            $"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}') " +
            $"""
            CREATE TABLE [dbo].[{tableName}](
            	[Id] [int] IDENTITY(1,1) NOT NULL,
            	[FileName] [nvarchar](max) NULL,
            	[Latitude] [float] NULL,
            	[Longitude] [float] NULL,
            	[Location] [geography] NULL,
             CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
            (
            	[Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
            """
        );
    }

}