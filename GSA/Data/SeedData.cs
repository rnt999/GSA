using CsvHelper;
using CsvHelper.Configuration;
using GSA.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSA.Data
{
    public class SeedData
    {
        const string dateType = "yyyy-MM-dd";
        const string capitalFile = "\\DataStore\\capital.csv";
        const string pnlFile = "\\DataStore\\pnl.csv";
        const string propertiesFile = "\\DataStore\\properties.csv";
        const string strategyName = "Strategy";

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

                    while (csv.Read())
                    {
                        var intField = csv.GetField<DateTime>(0);
                        var stringField = csv.GetField<string>(1);
                    }
                }
                context.SaveChanges();
            }

            if (!context.PNLs.Any())
            {
                using (TextReader fileReader = File.OpenText(path + pnlFile))
                {
                    var csv = new CsvReader(fileReader);
                    foreach (dynamic records in csv.GetRecords<dynamic>().ToList())
                    {
                        var count = records.Count;
                        var date = DateTime.ParseExact(records[0], dateType, null);

                        for (var i = 1; i < count; i++)
                        {
                            var strategyId = strategies.First(s => s.StratName.Equals(strategyName + i)).Id;
                            var pnl = new PNL() { StrategyId = strategyId, Date = date, Value = records[i] };
                            context.PNLs.Add(pnl);
                        }
                    }
                }
                context.SaveChanges();
            }
 
        }
    }
}
