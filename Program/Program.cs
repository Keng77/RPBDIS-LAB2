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
        string comment1 = "Вставка нового предприятия";

        // Создание нового объекта предприятия с заранее заданными значениями
        var newEnterprise = new Enterprise
        {
            Name = "AMAZON",
            OwnershipType = "OAO",
            Address = "KONECHAYA 1",
            DirectorName = "DJEF BEZOS",
            DirectorPhone = "71234567890"
        };

        // Добавление нового предприятия в контекст
        db.Enterprises.Add(newEnterprise);

        // Сохранение изменений в базе данных
        db.SaveChanges();

        // Вывод информации о новом предприятии
        Print(comment1, new List<Enterprise> { newEnterprise });

        // Вывод последних 5 предприятий, включая только что добавленное

        string comment2 = "Запись успешно вставлена. Вывожу послежние записи в таблице Предприятия:";

        // Получение последних 5 предприятий из базы данных
        var lastFiveEnterprises = db.Enterprises
            .OrderByDescending(e => e.EnterpriseId) // Сортировка по убыванию ID
            .Take(5) // Выборка первых 5 записей
            .ToList();

        Print(comment2, lastFiveEnterprises); // Передаем список последних 5 предприятий
    }


    static void InsertIntoManySide(InspectionsDbContext db)
    {
        string comment1 = "Вставка данных в таблицу (многие)";

        // Пример данных для вставки
        var inspection = new Inspection
        {
            InspectorId = 1, // Предполагается, что инспектор с ID 1 существует
            EnterpriseId = 5, // Предполагается, что предприятие с ID 5 существует
            InspectionDate = DateOnly.FromDateTime(DateTime.Now),
            ProtocolNumber = "PR_2024_001",
            ViolationTypeId = 2, // Предполагается, что тип нарушения с ID 2 существует
            ResponsiblePerson = "Keng32",
            PenaltyAmount = 5002,
            PaymentDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
            CorrectionDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(60)),
            PaymentStatus = "Не оплачено",
            CorrectionStatus = "Не исправлено"

        };

        // Добавление новой проверки в контекст
        db.Inspections.Add(inspection);

        // Сохранение изменений в базе данных
        db.SaveChanges();

        // Печать информации о новой проверке
        Print(comment1, new List<Inspection> { inspection });

        var comment2 = "Запись успешно вставлена. Вывожу послежние записи в таблице Проверок";
        var lastFiveInspections = db.Inspections
        .OrderByDescending(i => i.InspectionId) // Сортировка по убыванию ID
        .Take(5) // Выборка первых 5 записей
        .ToList();

        Print(comment2, lastFiveInspections); // Печать последних 5 проверок
    }

    static void DeleteFromOneSide(InspectionsDbContext db)
    {
        // Получение последнего предприятия
        var lastEnterprise = db.Enterprises
            .OrderByDescending(e => e.EnterpriseId) // Сортировка по убыванию ID
            .FirstOrDefault(); // Получение первого результата (последнее предприятие)

        if (lastEnterprise == null)
        {
            throw new Exception("Нет предприятий для удаления.");           
        }

        // Удаление последнего предприятия из контекста
        db.Enterprises.Remove(lastEnterprise);
        db.SaveChanges(); // Сохранение изменений в базе данных

        // Вывод информации о удаленном предприятии
        Print($"Предприятие с ID {lastEnterprise.EnterpriseId} успешно удалено.", new List<Enterprise> { lastEnterprise });
    }

    static void DeleteFromManySide(InspectionsDbContext db)
    {
        // Получение последней проверки
        var lastInspection = db.Inspections
            .OrderByDescending(i => i.InspectionId) // Сортировка по убыванию ID
            .FirstOrDefault(); // Получение первого результата (последняя проверка)

        if (lastInspection == null)
        {
            throw new Exception("Нет проверок для удаления.");
        }

        // Удаление последней проверки из контекста
        db.Inspections.Remove(lastInspection);
        db.SaveChanges(); // Сохранение изменений в базе данных

        // Вывод информации о удаленной проверке
        Print($"Проверка с ID {lastInspection.InspectionId} успешно удалена.", new List<Inspection> { lastInspection });
    }

    static void UpdateRecords(InspectionsDbContext db)
    {
        // Максимальное значение для сравнения
        decimal maxDebtThreshold = 400000m;

        // Получение всех предприятий с суммой долгов по проверкам
        var enterprisesWithDebts = db.Enterprises
            .Select(e => new
            {
                EnterpriseId = e.EnterpriseId,
                TotalDebt = db.Inspections
                    .Where(i => i.EnterpriseId == e.EnterpriseId)
                    .Sum(i => i.PenaltyAmount)
            })
            .ToList();

        // Фильтрация предприятий, у которых сумма долгов больше максимального значения
        var enterprisesToUpdate = enterprisesWithDebts
            .Where(e => e.TotalDebt > maxDebtThreshold)
            .ToList();

        if (enterprisesToUpdate.Count == 0)
        {
            throw new Exception("Нет предприятий с долгом для обновления.");
        }

        // Обновление записей: увеличение суммы штрафа на 10% для всех проверок данного предприятия
        foreach (var enterprise in enterprisesToUpdate)
        {
            var inspectionsToUpdate = db.Inspections
                .Where(i => i.EnterpriseId == enterprise.EnterpriseId)
                .ToList();

            foreach (var inspection in inspectionsToUpdate)
            {
                inspection.PenaltyAmount *= 1.01m; // Увеличение на 10%
            }
        }

        // Сохранение изменений в базе данных
        db.SaveChanges();

        // Вывод информации об обновленных записях
        Print($"Обновлено записей для предприятий с долгом больше {maxDebtThreshold}: {enterprisesToUpdate.Count}", enterprisesToUpdate);
    }



    static void Print(string comment, IEnumerable items)
    {
        Console.WriteLine();
        Console.WriteLine(comment);
        Console.WriteLine();
        Console.WriteLine("Записи: ");
        Console.WriteLine();
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
            Console.WriteLine();
        }
        Console.WriteLine();
        
    }
}