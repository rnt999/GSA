using CsvHelper;
using GSA.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;


namespace GSA.Data
{
    public class SeedData
    {
        public const string DateType = "yyyy-MM-dd";
        const string capitalFile = "\\DataStore\\capital.csv";
        const string pnlFile = "\\DataStore\\pnl.csv";
        const string propertiesFile = "\\DataStore\\properties.csv";
        const string strategyName = "Strategy";
        const string dateKey = "Date";

        public static void Initialize(IServiceProvider serviceProvider)
        {
            string path = Directory.GetCurrentDirectory();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            if(!context.Strategies.Any())
            {
                using (TextReader fileReader = File.OpenText(path + propertiesFile))
                {
                    var csv = new CsvReader(fileReader);
                    foreach (dynamic records in csv.GetRecords<dynamic>())
                    {
                        var strategy = new Strategy() { StratName = records.StratName, Region = records.Region };
                        context.Strategies.Add(strategy);
                    }
                }

                context.SaveChanges();
            }

            var strategies = context.Strategies.ToList();

            if (!context.Capitals.Any())
            {
                using (TextReader fileReader = File.OpenText(path + capitalFile))
                {
                    var csv = new CsvReader(fileReader);

                    foreach (dynamic records in csv.GetRecords<dynamic>())
                    {
                        var date = DateTime.Now;
                        foreach (var record in records)
                        {                            
                            if (record.Key == dateKey)
                            {
                                date = DateTime.ParseExact(record.Value, DateType, null);
                            }
                            else {
                                var strategyId = strategies.First(s => s.StratName.Equals(record.Key)).Id;
                                var capital = new Capital() { StrategyId = strategyId, Date = date, Value = Int32.Parse(record.Value) };
                                context.Capitals.Add(capital);
                            }
                        }
                    }
                    context.SaveChanges();
                }
                
            }

            if (!context.PNLs.Any())
            {
                using (TextReader fileReader = File.OpenText(path + pnlFile))
                {
                    var csv = new CsvReader(fileReader);
                    foreach (dynamic records in csv.GetRecords<dynamic>().ToList())
                    {
                        var date = DateTime.Now;
                        foreach (var record in records)
                        {
                            if (record.Key == dateKey)
                            {
                                date = DateTime.ParseExact(record.Value, DateType, null);
                            }
                            else
                            {
                                var strategyId = strategies.First(s => s.StratName.Equals(record.Key)).Id;
                                var pnl = new PNL() { StrategyId = strategyId, Date = date, Value = Int32.Parse(record.Value) };
                                context.PNLs.Add(pnl);
                            }
                        }
                    }
                    context.SaveChanges();
                }                
            }
 
        }
    }
}
