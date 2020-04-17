SELECT * FROM Computer

SELECT c.Id, e.Id, e.ComputerId FROM Computer c
LEFT JOIN Employee e ON e.ComputerId = c.Id


SELECT c.Id, c.Make, c.Model, c.PurchaseDate, c.DecomissionDate, e.FirstName, e.LastName FROM Computer c 
LEFT JOIN Employee e ON c.Id = e.ComputerId


SELECT Id, COALESCE(FirstName + ' ' + LastName, 'N/A') as EmployeeName FROM Employee


