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
                  cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, d.[Name] AS DeptName, c.Make, c.Model, e.Email, e.IsSupervisor, tp.Name, tp.StartDate,tp.EndDate
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

                        employee.TrainingProgram.Add(new TrainingProgram());
                        {
                        Name = reader.GetString(reader.GetOrdinal("Name"));
                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                        EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
                        }
                    );



                    };

                    }
                  reader.Close();
                  return View(employee);
              }
          }
      }

    //    // GET: Employees/Create
    //    public ActionResult Create()
    //    {
    //        var cohortOptions = GetCohortOptions();
    //        var viewModel = new EmployeeEditViewmodel()
    //        {
    //            CohortOptions = cohortOptions
    //        };
    //        return View(viewModel);
    //    }

    //    // POST: Employees/Create
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Create(Employee employee)
    //    {
    //        try
    //        {
    //            using (SqlConnection conn = Connection)
    //            {
    //                conn.Open();
    //                using (SqlCommand cmd = conn.CreateCommand())
    //                {
    //                    cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, SlackHandle, CohortId, Specialty)
    //                                        OUTPUT INSERTED.Id
    //                                        VALUES (@firstName, @lastName, @slackHandle, @cohortId, @specialty)";

    //                    cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
    //                    cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
    //                    cmd.Parameters.Add(new SqlParameter("@slackHandle", employee.SlackHandle));
    //                    cmd.Parameters.Add(new SqlParameter("@cohortId", employee.CohortId));
    //                    cmd.Parameters.Add(new SqlParameter("@specialty", employee.Specialty));

    //                    var id = (int)cmd.ExecuteScalar();
    //                    employee.Id = id;

    //                    return RedirectToAction(nameof(Index));
    //                }
    //            }


    //        }
    //        catch (Exception ex)
    //        {
    //            return View();
    //        }
    //    }

    //    // GET: Employees/Edit/5
    //    public ActionResult Edit(int id)
    //    {
    //        var employee = GetEmployeeById(id);
    //        var cohortOptions = GetCohortOptions();
    //        var viewModel = new EmployeeEditViewmodel()
    //        {
    //            EmployeeId = employee.Id,
    //            FirstName = employee.FirstName,
    //            LastName = employee.LastName,
    //            Specialty = employee.Specialty,
    //            CohortId = employee.CohortId,
    //            SlackHandle = employee.SlackHandle,
    //            CohortOptions = cohortOptions
    //        };
    //        return View(viewModel);
    //    }

    //    // PUT: Employees/Edit/5
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Edit(int id, Employee employee)
    //    {
    //        try
    //        {
    //            using (SqlConnection conn = Connection)
    //            {
    //                conn.Open();
    //                using (SqlCommand cmd = conn.CreateCommand())
    //                {
    //                    cmd.CommandText = @"UPDATE Employee 
    //                                        SET FirstName = @firstName, 
    //                                            LastName = @lastName, 
    //                                            SlackHandle = @slackHandle, 
    //                                            CohortId = @cohortId,
    //                                            Specialty = @specialty
    //                                        WHERE Id = @id";

    //                    cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
    //                    cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
    //                    cmd.Parameters.Add(new SqlParameter("@slackHandle", employee.SlackHandle));
    //                    cmd.Parameters.Add(new SqlParameter("@cohortId", employee.CohortId));
    //                    cmd.Parameters.Add(new SqlParameter("@specialty", employee.Specialty));
    //                    cmd.Parameters.Add(new SqlParameter("@id", id));

    //                    var rowsAffected = cmd.ExecuteNonQuery();

    //                    if (rowsAffected < 1)
    //                    {
    //                        return NotFound();
    //                    }
    //                }
    //            }

    //            return RedirectToAction(nameof(Index));
    //        }
    //        catch (Exception ex)
    //        {
    //            return View();
    //        }
    //    }

    //    // GET: Employees/Delete/5
    //    public ActionResult Delete(int id)
    //    {
    //        var employee = GetEmployeeById(id);
    //        return View(employee);
    //    }

    //    // POST: Employees/Delete/5
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Delete(int id, Employee employee)
    //    {
    //        try
    //        {
    //            using (SqlConnection conn = Connection)
    //            {
    //                conn.Open();
    //                using (SqlCommand cmd = conn.CreateCommand())
    //                {
    //                    cmd.CommandText = "DELETE FROM Employee WHERE Id = @id";
    //                    cmd.Parameters.Add(new SqlParameter("@id", id));

    //                    cmd.ExecuteNonQuery();
    //                }
    //            }

    //            return RedirectToAction(nameof(Index));
    //        }
    //        catch (Exception ex)
    //        {
    //            return View();
    //        }
    //    }

    //    private List<SelectListItem> GetCohortOptions()
    //    {
    //        using (SqlConnection conn = Connection)
    //        {
    //            conn.Open();
    //            using (SqlCommand cmd = conn.CreateCommand())
    //            {
    //                cmd.CommandText = "SELECT Id, Name FROM Cohort";

    //                var reader = cmd.ExecuteReader();
    //                var options = new List<SelectListItem>();

    //                while (reader.Read())
    //                {
    //                    var option = new SelectListItem()
    //                    {
    //                        Text = reader.GetString(reader.GetOrdinal("Name")),
    //                        Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
    //                    };

    //                    options.Add(option);

    //                }
    //                reader.Close();
    //                return options;
    //            }
    //        }
    //    }
    //    private Employee GetEmployeeById(int id)
    //    {
    //        using (SqlConnection conn = Connection)
    //        {
    //            conn.Open();
    //            using (SqlCommand cmd = conn.CreateCommand())
    //            {
    //                cmd.CommandText = "SELECT Id, FirstName, LastName, CohortId, SlackHandle, Specialty FROM Employee WHERE Id = @id";

    //                cmd.Parameters.Add(new SqlParameter("@id", id));

    //                var reader = cmd.ExecuteReader();
    //                Employee employee = null;

    //                if (reader.Read())
    //                {
    //                    employee = new Employee()
    //                    {
    //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
    //                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
    //                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
    //                        SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
    //                        CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
    //                        Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
    //                    };

    //                }
    //                reader.Close();
    //                return employee;
    //            }
    //        }
    //    }

    }
}
