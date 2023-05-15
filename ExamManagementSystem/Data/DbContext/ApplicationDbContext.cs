using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementSystem.Data.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public bool DisposeContext { get; set; } = false;

        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<ExamToStudent> ExamToStudents { get; set; }
        public DbSet<ExamToQuestion> ExamToQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            _ = builder.Entity<ExamToStudent>().HasOne(x => x.Student).WithMany().OnDelete(DeleteBehavior.Restrict);
            _ = builder.Entity<Answer>().HasOne(x => x.Option).WithMany().OnDelete(DeleteBehavior.Restrict);
            _ = builder.Entity<ExamToQuestion>()
                .HasIndex(x => new { x.QuestionId, x.ExamId })
                .IsUnique();
        }
        public override void Dispose()
        {
            if (!DisposeContext)
            {
                return;
            }
            base.Dispose();
        }
    }
}