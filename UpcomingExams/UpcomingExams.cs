using System;
using System.Collections.Generic;
using System.Linq;
using ExamManagementSystem.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UpcomingExams
{
    public class UpcomingExams
    {
        private ApplicationDbContext _dbContext;
        public UpcomingExams(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Showing upcoming exams for the next 10 Days.
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        [FunctionName("ExamsOfNextTenDays")]
        public void ExamsOfNextTenDays([TimerTrigger("* * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            try
            {
                log.LogInformation("\nShowing upcoming exams for the next 10 Days.\n");

                var upcomingExams = _dbContext.Exams.Include(x => x.Questions).Include(x => x.User).ToList().Where(x => x.Date.Subtract(DateTime.Now).TotalDays < 10);
                foreach (var exam in upcomingExams)
                {
                    log.LogInformation($"\nExam: {exam.Id}"
                        + $"\nDuration: {exam.Duration} Mins."
                        + $"\nDate: {exam.Date.Add(exam.StartTime.TimeOfDay)} "
                        + $"\nMarks: {exam.Questions.Sum(x => x.Marks)} "
                        + $"\nConducted by: {exam.User.UserName} "
                        + "\n");
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Timer trigger function failed to execute \n"
                    + $"Message : {ex.Message} \nStackTrace : {ex.StackTrace}");
                throw ex;
            }
        }
    }
}
