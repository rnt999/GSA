using System;
using System.Collections.Generic;
using System.Linq;
using GSA.Data;
using GSA.Model;
using Microsoft.AspNetCore.Mvc;

namespace GSA.Controllers
{
    [Route("api")]
    public class DataController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("monthly-capital")]
        public IEnumerable<CapitalDTO> GetMonthlyCapital([FromQuery] string strategies)
        {
            var strategiesArray = String.Join(",", strategies);
            var strats = _context.Strategies.Where(s => strategiesArray.Contains(s.StratName)).ToList();
            var monthlyCapitals = new List<CapitalDTO>();

            foreach (var strat in strats)
            {
                var capitals = _context.Capitals.Where(c => c.StrategyId == strat.Id).ToList();
                monthlyCapitals.AddRange(
                    capitals.Select(a => new CapitalDTO()
                    {
                        Capital = a.Value,
                        Date = a.Date,
                        Strategy = strat.StratName
                    }).ToList());
            }

            return monthlyCapitals
                .OrderBy(a => a.Date)
                .ThenBy(b => b.Strategy);
        }

        [HttpGet("cumulative-pnl")]
        public IEnumerable<PNLDTO> GetCumalativePNL([FromQuery] string startDate, [FromQuery] string region)
        {
            var strategiesIds = _context.Strategies.Where(a => a.Region == region).Select(a => a.Id).ToList();
            var date = DateTime.ParseExact(startDate, SeedData.DateType, null);
            var results = _context.PNLs
                .Where(a => strategiesIds.Contains(a.StrategyId)
                && a.Date >= date)
                .GroupBy(a => a.Date)
                .Select(a => new PNLDTO
                {
                    Region = region,
                    Date = a.First().Date,
                    CumulativePnl = a.Sum(c => c.Value)
                })
                .OrderBy(a => a.Date)
                .ToList();

            return results;
        }

        [HttpGet("compound-daily-returns/{strategy}")]
        public IEnumerable<CompoundDTO> GetCompoundDailyReturns(string strategy)
        {            
            var strat = _context.Strategies.FirstOrDefault(s => s.StratName.Equals(strategy));
            var capitals = _context.Capitals.Where(a => a.StrategyId == strat.Id).OrderBy(a => a.Date);

            var pnls = _context.PNLs.Where(p => p.StrategyId == strat.Id).OrderBy(p => p.Date).ToList();

            var compounded = new List<CompoundDTO>();
            var initialInvestment = 0;
            var investmentReturn = 0;
            var startYear = 0;
            var startMonth = 0;

            foreach (var pnl in pnls) {
                if (startYear != pnl.Date.Year && startMonth != pnl.Date.Month)
                {
                    startYear = pnl.Date.Year;
                    startMonth = pnl.Date.Month;
                    initialInvestment = capitals.FirstOrDefault(a => a.Date.Year == startYear 
                    && a.Date.Month == startMonth).Value;
                    investmentReturn = initialInvestment;
                }
                investmentReturn = investmentReturn + pnl.Value;
                decimal compoundedReturn = 0;
                if (initialInvestment != 0) {
                    compoundedReturn = Math.Round((decimal)investmentReturn / initialInvestment, 5);
                }
                
                compounded.Add(new CompoundDTO
                {
                    Strategy = strat.StratName,
                    Date = pnl.Date,
                    CompoundReturn = compoundedReturn
                });
            }
            return compounded;
        }
    }
}
