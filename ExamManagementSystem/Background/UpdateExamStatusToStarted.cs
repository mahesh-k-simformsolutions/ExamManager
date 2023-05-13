using ExamManagementSystem.Data;
using ExamManagementSystem.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementSystem.Background
{
    public class UpdateExamStatusToStarted : IHostedService
    {
        private readonly ILogger<UpdateExamStatusToStarted> _logger;
        private readonly IConfiguration _config;
        private Timer _timer;

        public UpdateExamStatusToStarted(ILogger<UpdateExamStatusToStarted> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, GetTime.GetDelayTime(), TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            using SqlConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            try
            {
                connection.Open();
                var examList = new List<Exam>();
                string query = $"SELECT Id,StartTime FROM Exams WHERE ExamStatus=@examStatus";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@examStatus", (int)EnumExamStatus.NotStarted);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    examList.Add(new Exam
                    {
                        Id = reader.GetInt32(0),
                        StartTime = reader.GetDateTime(1),
                    });
                }

                foreach (var exam in examList)
                {
                    TimeSpan delay = DateTime.Now - exam.StartTime;
                    if (delay.TotalMilliseconds > 0)
                    {
                        var updateQuery = "UPDATE Exams SET ExamStatus = @examStatus WHERE Id = @id";
                        using var updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@examStatus", (int)EnumExamStatus.Started);
                        updateCommand.Parameters.AddWithValue("@id", exam.Id);
                        updateCommand.ExecuteNonQuery();
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
            _timer?.Dispose();
        }
    }
}
