using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RPBDISLAB2.Models;
using System.Collections;

public class Program
{
    public static string? comment { get; private set; }

    static void Main(string[] args)
    {        
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var config = builder.Build();
        string? connectionString = config.GetConnectionString("MsSqlConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'MsSqlConnection' is not found in appsettings.json.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<InspectionsDbContext>();
        var options = optionsBuilder.UseSqlServer(connectionString).Options;

        using (var db = new InspectionsDbContext(options))
        {
            ShowMenu(db);
        }
    }

    static void ShowMenu(InspectionsDbContext db)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Меню базы данных ===");
            Console.WriteLine("1. Выборка всех данных из таблицы (один)");
            Console.WriteLine("2. Фильтрация данных из таблицы (один)");
            Console.WriteLine("3. Группировка данных (многие)");
            Console.WriteLine("4. Выборка данных из двух связанных таблиц (один-ко-многим)");
            Console.WriteLine("5. Фильтрация данных из двух связанных таблиц (один-ко-многим)");
            Console.WriteLine("6. Вставка данных в таблицу (один)");
            Console.WriteLine("7. Вставка данных в таблицу (многие)");
            Console.WriteLine("8. Удаление данных из таблицы (один)");
            Console.WriteLine("9. Удаление данных из таблицы (многие)");
            Console.WriteLine("10. Обновление записей в таблице");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1": SelectAllFromOneSide(db); break;
                case "2": FilterFromOneSide(db); break;
                case "3": GroupDataFromManySide(db); break;
                case "4": SelectFromTwoTablesOneToMany(db); break;
                case "5": FilterFromTwoTablesOneToMany(db); break;
                case "6": InsertIntoOneSide(db); break;
                case "7": InsertIntoManySide(db); break;
                case "8": DeleteFromOneSide(db); break;
                case "9": DeleteFromManySide(db); break;
                case "10": UpdateRecords(db); break;
                case "0": return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                    break;
            }

            Console.WriteLine("Нажмите любую клавишу чтобы вернутся к меню...");
            Console.ReadKey();
        }
    }

    static void SelectAllFromOneSide(InspectionsDbContext db)
    {
        comment = "Выборка всех данных из таблицы (один)";
        // Реализация выборки данных
        var sqlQuery = db.Enterprises.Select(e => new
        {
            e.EnterpriseId,
            e.Name,
            e.OwnershipType,
            e.Address,
            e.DirectorName,
            e.DirectorPhone
        });

        Print(comment, sqlQuery.Take(5).ToList());
    }

    static void FilterFromOneSide(InspectionsDbContext db)
    {
        string comment = "Фильтрация данных из таблицы нарушений с штрафом больше 3000";
        // Условие: выбираем типы нарушений с штрафом больше 3000
        decimal penaltyThreshold = 3000;

        var violationTypes = db.ViolationTypes
            .Where(vt => vt.PenaltyAmount > penaltyThreshold)
            .Select(vt => new
            {
                vt.ViolationTypeId,
                vt.Name,
                vt.PenaltyAmount,
                vt.CorrectionPeriodDays
            });

        Print(comment, violationTypes.Take(5).ToList());
    }

    static void GroupDataFromManySide(InspectionsDbContext db)
    {
        string comment = "Группировка данных по типам нарушений";

        var groupedData = db.Inspections
            .GroupBy(i => i.ViolationTypeId)
            .Select(g => new
            {
                ViolationTypeId = g.Key,
                InspectionCount = g.Count(),
                TotalPenaltyAmount = g.Sum(i => i.PenaltyAmount)
            })
            .ToList();

        Print(comment, groupedData.Take(5));
    }

    static void SelectFromTwoTablesOneToMany(InspectionsDbContext db)
    {
        string comment = "Выборка данных из таблиц 'Предприятие' и 'Проверки'";

        var query = from enterprise in db.Enterprises
                    join inspection in db.Inspections on enterprise.EnterpriseId equals inspection.EnterpriseId
                    select new
                    {
                        EnterpriseName = enterprise.Name,
                        InspectionDate = inspection.InspectionDate
                    };

        var results = query.ToList();

        Print(comment, results.Take(5));
    }

    static void FilterFromTwoTablesOneToMany(InspectionsDbContext db)
    {
        string comment = "Фильтрация данных из двух связанных таблиц (один-ко-многим)";
        decimal penaltyThreshold = 50000;

        var filteredData = db.Enterprises
                .GroupJoin(db.Inspections,
                          enterprise => enterprise.EnterpriseId,
                          inspection => inspection.EnterpriseId,
                          (enterprise, inspections) => new
                          {
                              EnterpriseName = enterprise.Name,
                              InspectionCount = inspections.Count(),
                              TotalPenaltyAmount = inspections.Sum(i => i.PenaltyAmount)
                          })
                .Where(result => result.InspectionCount > 0 && result.TotalPenaltyAmount > penaltyThreshold)
                .OrderByDescending(result => result.TotalPenaltyAmount) // Сортировка по убыванию
                .ToList();

        Print(comment, filteredData.Take(5));
    }

    static void InsertIntoOneSide(InspectionsDbContext db)
    {
        comment = "Вставка данных в таблицу (один)";
        // Реализация вставки данных
    }

    static void InsertIntoManySide(InspectionsDbContext db)
    {
        comment = "Вставка данных в таблицу (многие)";
        // Реализация вставки данных
    }

    static void DeleteFromOneSide(InspectionsDbContext db)
    {
        comment = "Удаление данных из таблицы (один)";
        // Реализация удаления данных
    }

    static void DeleteFromManySide(InspectionsDbContext db)
    {
        comment = "Удаление данных из таблицы (многие)";
        // Реализация удаления данных
    }

    static void UpdateRecords(InspectionsDbContext db)
    {
        comment = "Обновление записей в таблице";
        // Реализация обновления данных
    }

    static void Print(string comment, IEnumerable items)
    {
        Console.WriteLine();
        Console.WriteLine(comment);
        Console.WriteLine();
        Console.WriteLine("Записи: ");
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.ReadKey();
    }
}