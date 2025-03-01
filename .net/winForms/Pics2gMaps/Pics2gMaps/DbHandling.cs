using Common;
using Microsoft.EntityFrameworkCore;
using System;
using ImageHandling;

namespace Pics2gMaps;

public class DbHandling:ICommandHandlerAsync<DbHandlingCommand>
{
    private readonly AppDbContext _context = new();

    static void UpsertRecord(AppDbContext context, LatLngFileNameModel latLngFileNameModel)
    {
        using var context1 = new AppDbContext();
        context1.Database.EnsureCreated();
        var existingRecord = context1.GpsInfo.FirstOrDefault(latLngFileName => latLngFileName.FileName == latLngFileNameModel.FileName);

        if (existingRecord != null)
        {
            // Update existing record
            existingRecord.FileName = latLngFileNameModel.FileName;
            context1.Update(existingRecord);
            Console.WriteLine("Record updated.");
        }
        else
        {
            // Insert new record
            context1.Add(new LatLngFileNameModel { FileName = latLngFileNameModel.FileName
                , Latitude = latLngFileNameModel.Latitude
                , Longitude = latLngFileNameModel.Longitude
            });
            Console.WriteLine("New record inserted.");
        }

        context1.SaveChanges();
    }

    public async Task Execute(DbHandlingCommand command)
    {
        await Task.Run(() => UpsertRecord(_context, command.LatLngFileNameModel));
    }

    public class AppDbContext : DbContext
    {
        public DbSet<LatLngFileNameModel> GpsInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LatLngFileNameModel>().ToTable("GpsInfo");
        }

        public void Init() => Database.EnsureCreated();

    }

    public static async Task EnsureTableExists()
    {
        await using var dbContext = new AppDbContext();
        var tableExists = await dbContext.Database.ExecuteSqlRawAsync(
            "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Records') " +
            "CREATE TABLE Records (Id INT PRIMARY KEY, Value NVARCHAR(100))"
        );
    }

}