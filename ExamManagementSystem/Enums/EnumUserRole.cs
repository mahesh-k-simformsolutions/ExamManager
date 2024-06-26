﻿namespace ExamManagementSystem.Enums
{
    public enum EnumUserRole
    {
        Admin,
        Teacher,
        Student
    }

    public enum EnumExamStatus
    {
        Started,
        NotStarted,
        Cancelled,
        Completed
    }
    public enum EnumExamToStudentStatus
    {
        NotAppeared,
        Appeared,
        Started,
        LeftUnubmitted
    }
    public enum ExamResultStatus
    {
        Pass,
        Fail
    }
}
