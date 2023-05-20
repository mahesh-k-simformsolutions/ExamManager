using ExamManagementSystem.Data;
using ExamManagementSystem.Hubs;
using Microsoft.Data.SqlClient;

namespace ExamManagementSystem.Background
{
    public static class Common
    {
        public static TimeSpan GetDelayTime()
        {
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new(now.Year, now.Month, now.Day, 7, 30, 0);
            if (scheduledTime < now)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }
            TimeSpan timeUntilScheduledTime = scheduledTime - now;
            TimeSpan timeUntilNext1MinuteInterval = TimeSpan.FromMinutes(1) - TimeSpan.FromMinutes(timeUntilScheduledTime.TotalMinutes % 1);
            return timeUntilNext1MinuteInterval;
        }

        public static async Task NotifyStudents(SqlConnection connection, Exam exam, NotificationHub hub, ILogger logger)
        {
            string query = $"SELECT u.Email FROM ExamToStudents as eTos JOIN AspNetUsers as u ON eTos.StudentId=u.Id WHERE ExamId=@examId";
            using SqlCommand getStudentsCommand = new(query, connection);
            getStudentsCommand.Parameters.AddWithValue("@examId", exam.Id);
            using SqlDataReader readStudents = getStudentsCommand.ExecuteReader();
            while (readStudents.Read())
            {
                string studentEmail = readStudents.GetString(0);

                await hub.NotifyExamStatus(studentEmail, exam);

                logger.LogInformation($"Notified student {studentEmail} on {DateTime.Now}");
            }
        }
    }
}
