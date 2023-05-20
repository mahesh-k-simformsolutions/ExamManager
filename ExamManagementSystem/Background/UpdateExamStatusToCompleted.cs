using ExamManagementSystem.Data;
using ExamManagementSystem.Enums;
using ExamManagementSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementSystem.Background
{
    public class UpdateExamStatusToCompleted : IHostedService
    {
        private readonly ILogger<UpdateExamStatusToStarted> _logger;
        private readonly IConfiguration _config;
        private Timer _timer;
        private readonly NotificationHub _hub;

        public UpdateExamStatusToCompleted(ILogger<UpdateExamStatusToStarted> logger, IConfiguration config, NotificationHub hub)
        {
            _logger = logger;
            _config = config;
            _hub = hub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, Common.GetDelayTime(), TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            using SqlConnection connection = new(_config.GetConnectionString("DefaultConnection"));
            try
            {
                connection.Open();
                List<Exam> examList = new();
                string query = $"SELECT Id,EndTime,ExamName,ExamCode FROM Exams WHERE ExamStatus=@examStatus";
                using SqlCommand command = new(query, connection);
                _ = command.Parameters.AddWithValue("@examStatus", (int)EnumExamStatus.Started);
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    examList.Add(new Exam
                    {
                        Id = reader.GetInt32(0),
                        EndTime = reader.GetDateTime(1),
                        ExamName = reader.GetString(2),
                        ExamCode = reader.GetString(3),
                    });
                }

                foreach (Exam exam in examList)
                {
                    TimeSpan delay = DateTime.Now - exam.EndTime;
                    if (delay.TotalMilliseconds > 0)
                    {
                        string updateQuery = "UPDATE Exams SET ExamStatus = @examStatus WHERE Id = @id";
                        using SqlCommand updateCommand = new(updateQuery, connection);
                        _ = updateCommand.Parameters.AddWithValue("@examStatus", (int)EnumExamStatus.Completed);
                        _ = updateCommand.Parameters.AddWithValue("@id", exam.Id);
                        _ = updateCommand.ExecuteNonQuery();

                        exam.ExamStatus = EnumExamStatus.Completed;
                        Task.FromResult(Common.NotifyStudents(connection, exam, _hub, _logger));

                        _logger.LogInformation($"Exam {exam.Id} updated to {exam.ExamStatus} on {DateTime.Now}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction failed: {ex.Message}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
        }
    }
}
