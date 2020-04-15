using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Workforce_Purple_Parrots.Models;
using Workforce_Purple_Parrots.Models.ViewModels;

namespace Workforce_Purple_Parrots.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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

        // GET: Departments
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.[Name], d.Budget, COUNT(e.DepartmentId) as EmployeeCount
                                        FROM Employee e
                                        LEFT JOIN Department d ON e.DepartmentId = d.Id
                                        GROUP BY d.Id, d.[Name], d.Budget";

                    var reader = cmd.ExecuteReader();
                    var employees = new List<DepartmentViewModel>();

                    while (reader.Read())
                    {
                        employees.Add(new DepartmentViewModel()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount"))


                        }

                    );

                    }
                    reader.Close();
                    return View(employees);
                }
            }

        }
    }
}




//        // GET: Departments/Details/1
//        public ActionResult Details(int id)
//        {
//            var Department = GetDepartmentById(id);
//            return View(Department);
//        }

//        // GET: Departments/Create
//        public ActionResult Create()
//        {
//            var cohortOptions = GetCohortOptions();
//            var viewModel = new DepartmentEditViewModel()
//            {
//                CohortOptions = cohortOptions
//            };
//            return View(viewModel);
//        }

//        // POST: Departments/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(DepartmentEditViewModel department)
//        {
//            try
//            {
//                using (SqlConnection conn = Connection)
//                {
//                    conn.Open();
//                    using (SqlCommand cmd = conn.CreateCommand())
//                    {
//                        cmd.CommandText = @"INSERT INTO Department (Name, Budget)
//                                            OUTPUT INSERTED.Id
//                                            VALUES (@Name, @Budget)";

//                        cmd.Parameters.Add(new SqlParameter("@Name", Department.Name));
//                        cmd.Parameters.Add(new SqlParameter("@Budget", Department.Budget));
//                        

//                        

//                        var id = (int)cmd.ExecuteScalar();
//                        department.DepartmentId = id;

//                        return RedirectToAction(nameof(Index));
//                    }
//                }


//            }
//            catch (Exception ex)
//            {
//                return View();
//            }
//        }

//        // GET: Departments/Edit/5
//        public ActionResult Edit(int id)
//        {
//            var Department = GetDepartmentById(id);
//            var cohortOptions = GetCohortOptions();
//            var viewModel = new DepartmentEditViewModel()
//            {
//                DepartmentId = Department.Id,
//                FirstName = Department.FirstName,
//                LastName = Department.LastName,
//                CohortId = Department.CohortId,
//                SlackHandle = Department.SlackHandle,
//                CohortOptions = cohortOptions
//            };
//            return View(viewModel);
//        }

//        // POST: Departments/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(int id, [FromForm] DepartmentEditViewModel Department)
//        {
//            try
//            {
//                using (SqlConnection conn = Connection)
//                {
//                    conn.Open();
//                    using (SqlCommand cmd = conn.CreateCommand())
//                    {
//                        cmd.CommandText = @"UPDATE Department 
//                                            SET FirstName = @firstName, 
//                                                LastName = @lastName, 
//                                                SlackHandle = @slackHandle, 
//                                                CohortId = @cohortId
//                                            WHERE Id = @id";

//                        cmd.Parameters.Add(new SqlParameter("@firstName", Department.FirstName));
//                        cmd.Parameters.Add(new SqlParameter("@lastName", Department.LastName));
//                        cmd.Parameters.Add(new SqlParameter("@slackHandle", Department.SlackHandle));
//                        cmd.Parameters.Add(new SqlParameter("@cohortId", Department.CohortId));
//                        cmd.Parameters.Add(new SqlParameter("@id", id));

//                        var rowsAffected = cmd.ExecuteNonQuery();

//                        if (rowsAffected < 1)
//                        {
//                            return NotFound();
//                        }
//                    }
//                }

//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        // GET: Departments/Delete/5
//        public ActionResult Delete(int id)
//        {
//            var Department = GetDepartmentById(id);
//            return View(Department);
//        }

//        // POST: Departments/Delete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(int id, Department Department)
//        {
//            try
//            {
//                using (SqlConnection conn = Connection)
//                {
//                    conn.Open();
//                    using (SqlCommand cmd = conn.CreateCommand())
//                    {
//                        cmd.CommandText = "DELETE FROM Department WHERE Id = @id";
//                        cmd.Parameters.Add(new SqlParameter("@id", id));

//                        cmd.ExecuteNonQuery();
//                    }
//                }

//                return RedirectToAction(nameof(Index));
//            }
//            catch (Exception ex)
//            {
//                return View();
//            }
//        }

//        private List<SelectListItem> GetCohortOptions()
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = "SELECT Id, Name FROM Cohort";

//                    var reader = cmd.ExecuteReader();
//                    var options = new List<SelectListItem>();

//                    while (reader.Read())
//                    {
//                        var option = new SelectListItem()
//                        {
//                            Text = reader.GetString(reader.GetOrdinal("Name")),
//                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
//                        };

//                        options.Add(option);

//                    }
//                    reader.Close();
//                    return options;
//                }
//            }
//        }

//        private Department GetDepartmentById(int id)
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"SELECT d.Id, d.[Name], d.Budget, e.FirstName, e.LastName 
//FROM Department d 
//LEFT JOIN Employee e ON d.Id = e.DepartmentId
//WHERE Id = @id";

//                    cmd.Parameters.Add(new SqlParameter("@id", id));

//                    var reader = cmd.ExecuteReader();
//                    Department Department = null;

//                    if (reader.Read())
//                    {
//                        Department = new Department()
//                        {
//                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                            Name = reader.GetString(reader.GetOrdinal("Name")),
//                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
//                           
//                        };

//                    }
//                    reader.Close();
//                    return Department;
//                }
//            }
//        }
//    }
//}
