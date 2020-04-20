using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Workforce_Purple_Parrots.Models;
using Microsoft.AspNetCore.Http;
using Workforce_Purple_Parrots.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Workforce_Purple_Parrots.Models.ViewModel;

namespace Workforce_Purple_Parrots.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employees
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, d.[Name] as DeptName FROM Employee e
                                       LEFT JOIN Department d ON d.Id = e.DepartmentId";

                    var reader = cmd.ExecuteReader();
                    var employees = new List<Employee>();

                    while (reader.Read())
                    {
                        var employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department
                            {
                                Name = reader.GetString(reader.GetOrdinal("DeptName"))
                            }
                        };
                        employees.Add(employee);
                    }
                    reader.Close();
                    return View(employees);
                }
            }

        }

        //GET: Employees/Details/1
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.Email, e.IsSupervisor, e.ComputerId, 
                                        d.[Name] AS DeptName, d.Budget, 
                                        c.PurchaseDate, c.DecomissionDate, c.Make, c.Model,
                                        tp.[Name], tp.StartDate, tp.EndDate
                                        FROM Employee e
                                        LEFT JOIN Department d ON d.Id = e.DepartmentId
                                        LEFT JOIN Computer c ON c.Id = e.ComputerId
                                        LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                        LEFT JOIN TrainingProgram tp ON tp.Id=et.TrainingProgramId
                                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Employee employee = null;

                    while (reader.Read())
                    {
                        if (employee == null)
                        {
                            employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Department = new Department
                                {
                                    Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                },
                                Computer = new Computer
                                {
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Model = reader.GetString(reader.GetOrdinal("Model"))
                                },
                                TrainingProgram = new List<TrainingProgram>()

                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                        {
                            employee.TrainingProgram.Add(new TrainingProgram()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
                            });
                        }
                        else
                        {
                            employee.TrainingProgram.Add(new TrainingProgram()
                            {
                                Name = null,
                                StartDate = null,
                                EndDate = null
                            });
                        }
                    }
                    reader.Close();
                    return View(employee);
                }
            }
        }

        // GET: Employees/Create
        public ActionResult Create()
        {

            var viewModel = new EmployeeFormViewModel();
            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeFormViewModel employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee  (FirstName, LastName, DepartmentId, Email, IsSupervisor, ComputerId)
                                           OUTPUT INSERTED.Id
                                           VALUES (@firstName, @lastName, @departmentId, @email, @isSupervisor, @computerId)";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));
                        cmd.Parameters.Add(new SqlParameter("@email", employee.Email));

                        var id = (int)cmd.ExecuteScalar();
                        employee.EmployeeId = id;

                        return RedirectToAction(nameof(Index));
                    }
                }


            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            var employee = GetEmployeeById(id);
            var departmentOptions = GetDepartmentOptions();
            var computerOptions = GetComputerOptions();
            var viewModel = new EmployeeFormViewModel()
            {
                EmployeeId = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                IsSupervisor = employee.IsSupervisor,
                Email = employee.Email,
                ComputerId = employee.ComputerId,
                DepartmentId = employee.Department.Id,
                DepartmentOptions = departmentOptions,
                ComputerOptions = computerOptions
            };
            return View(viewModel);
        }

        // PUT: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee 
                                                SET FirstName = @firstName, 
                                                    LastName = @lastName, 
                                                    IsSupervisor = @isSupervisor, 
                                                    Email = @email,
                                                    DepartmentId = @departmentId
                                                WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@email", employee.Email));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected < 1)
                        {
                            return NotFound();
                        }
                    }
                    if (employee.ComputerId != 0)
                    {
                        //update employee
                        //I will need, the computer.EmployeeId
                        //and I will need the computer.Id
                        UpdateComputer(employee.Id, employee.ComputerId);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        private void UpdateComputer(int employeeId, int computerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Employee
                                            SET ComputerId = @computerId
                                            WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@computerId", computerId));
                    cmd.Parameters.Add(new SqlParameter("@id", employeeId));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private List<SelectListItem> GetDepartmentOptions()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Department";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("Name")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }

        private List<SelectListItem> GetComputerOptions()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, COALESCE(Make + ' ' + Model, 'N/A') as ComputerInfo
                                        FROM Computer c
                                        LEFT JOIN Employee e ON c.Id = e.ComputerId
                                        WHERE e.Id is NULL";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("ComputerInfo")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }

        //pulls up view to assign training for employee and grabs all available training
        public ActionResult AssignTraining(int id)
        {
            var trainingOptions = GetAvaialbleTrainingOptions(id);
            var viewModel = new EmployeeTrainingViewModel()
            {
                Id = id,
                AvailableTrainingPrograms = trainingOptions
            };
            return View(viewModel);
        }

        //helper method to that gets available training for the multi-select view
        private List<SelectListItem> GetAvaialbleTrainingOptions(int? id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, tp.[Name], tp.StartDate, tp.EndDate, (tp.MaxAttendees - COUNT(et.EmployeeId)) AS AvailableSeats
                                        FROM TrainingProgram tp
                                        LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = tp.Id
                                        LEFT JOIN Employee e ON et.EmployeeId = e.Id
                                        WHERE tp.StartDate > GetDate() OR et.EmployeeId IS NULL
                                        GROUP BY tp.Id, tp.[Name], tp.StartDate, tp.EndDate, tp.MaxAttendees
                                        HAVING(tp.MaxAttendees - COUNT(et.EmployeeId)) > 0 AND tp.Id NOT IN (SELECT TrainingProgramId FROM EmployeeTraining WHERE EmployeeTraining.EmployeeId = @id)";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("Name")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
                        };

                        options.Add(option);
                    }
                    reader.Close();
                    return options;
                }
            }
        }

        //takes the selected values in EmployeeTrainingViewModel 
        //and loops over and creates and entry in EmployeeTraining for each selected training
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTraining(EmployeeTrainingViewModel employee)
        {
            bool isEmpty = !employee.TrainingProgramIds.Any();
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (isEmpty)
                        {
                            return RedirectToAction(nameof(Details), new { id = employee.Id });
                        }
                        else
                        {
                            for (int index = 0; index < employee.TrainingProgramIds.Count; index++)
                            {
                                cmd.CommandText = @"INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId)
                                                VALUES (@employeeId, @trainingProgramId)";

                                cmd.Parameters.Add(new SqlParameter("@employeeId", employee.Id));
                                cmd.Parameters.Add(new SqlParameter("@trainingProgramId", employee.TrainingProgramIds.ElementAt(index)));

                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            return RedirectToAction(nameof(Details), new { id = employee.Id });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return View(employee);
            }
        }

        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.ComputerId, d.[Name] AS DeptName, d.Id as DeptId, c.Make, c.Model, e.Email, e.IsSupervisor, tp.Name, tp.StartDate,tp.EndDate
                                     FROM Employee e
                                     LEFT JOIN Department d ON d.Id = e.DepartmentId
                                     LEFT JOIN Computer c ON c.Id = e.ComputerId
                                     LEFT JOIN EmployeeTraining et ON et.Id = e.Id
                                     LEFT JOIN TrainingProgram tp ON tp.Id = et.Id
                                     WHERE e.Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Employee employee = null;

                    while (reader.Read())
                    {
                        if (employee == null)
                        {
                            employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Department = new Department
                                {
                                    Name = reader.GetString(reader.GetOrdinal("DeptName")),
                                    Id = reader.GetInt32(reader.GetOrdinal("DeptId"))
                                },
                                Computer = new Computer
                                {

                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Model = reader.GetString(reader.GetOrdinal("Model"))
                                },
                                TrainingProgram = new List<TrainingProgram>()

                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                        {
                            employee.TrainingProgram.Add(new TrainingProgram()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
                            });
                        }
                        else
                        {
                            employee.TrainingProgram.Add(new TrainingProgram()
                            {
                                Name = null,
                                StartDate = null,
                                EndDate = null
                            });
                        }
                    }
                    reader.Close();
                    return employee;
                }
            }
        }
    } 
}
