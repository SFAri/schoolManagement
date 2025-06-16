using System;
using Internal;
using System.Threading.Tasks;
using Quartz;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Models
{
	public class YearEndCheckJob : IJob
	{
        private readonly SchoolContext _context;
		public YearEndCheckJob(SchoolContext context)
		{
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var currentYear = DateTime.Now.Year;
            var lastAcademicYearName = $"{currentYear - 1}-{currentYear}";

            var lastYear = await _context.AcademicYears
                .FirstOrDefaultAsync(y => y.Year == lastAcademicYearName);

            if (lastYear != null && !lastYear.IsLocked)
            {
                lastYear.IsLocked = true;
                await _context.SaveChangesAsync();

                Console.WriteLine($"[Quartz] Auto ended academic year: {lastYear.Year}");
            }
            else
            {
                Console.WriteLine($"[Quartz] Academic year {lastAcademicYearName} have ended or not exist.");
            }
        }
    }
}

