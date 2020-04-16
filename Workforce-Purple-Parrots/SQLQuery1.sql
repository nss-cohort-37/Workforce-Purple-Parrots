 SELECT e.Id, e.FirstName, e.LastName, d.[Name] AS DepartmentName, c.Make, c.Model, e.Email, e.IsSupervisor, tp.Name, tp.StartDate,tp.EndDate
 FROM Employee e
 LEFT JOIN Department d ON d.Id = e.DepartmentId
 LEFT JOIN Computer c ON c.Id = e.ComputerId 
 LEFT JOIN EmployeeTraining et ON et.Id = e.Id
 LEFT JOIN TrainingProgram tp ON tp.Id = et.Id
 WHERE e.Id = @Id
 