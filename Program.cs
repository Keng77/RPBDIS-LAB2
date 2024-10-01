using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RPBDISLAB2.Models;

public class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        // установка пути к текущему каталогу
        builder.SetBasePath(Directory.GetCurrentDirectory());
        // получаем конфигурацию из файла appsettings.json
        builder.AddJsonFile("appsettings.json");
        // создаем конфигурацию
        var config = builder.Build();
        // получаем строку подключения
        string connectionString = config.GetConnectionString("MsSqlConnection");

        var optionsBuilder = new DbContextOptionsBuilder<InspectionsDbContext>();
        var options = optionsBuilder
            .UseSqlServer(connectionString)
            .Options;

        using (InspectionsDbContext db = new InspectionsDbContext(options))
        {
            //выполнение запросов к бд
        }
        Console.Read();

    }


}