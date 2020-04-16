SELECT t.Id, t.[Name], t.StartDate, t.EndDate, t.MaxAttendees, e.Id as EmployeeId, e.FirstName, e.LastName
FROM TrainingProgram t 
LEFT JOIN EmployeeTraining et ON t.Id = et.TrainingProgramId
LEFT JOIN Employee e ON e.Id = et.EmployeeId
WHERE t.Id = 4;