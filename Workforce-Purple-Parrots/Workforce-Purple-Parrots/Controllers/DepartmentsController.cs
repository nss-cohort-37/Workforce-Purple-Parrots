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
        private Department department;

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
                                        FROM Department d
                                        LEFT JOIN Employee e ON e.DepartmentId = d.Id
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






        // GET: Departments/Details/1
        public ActionResult Details(int id)
        {
            var Department = GetDepartmentById(id);
            return View(Department);
        }

        public ActionResult Create()
        {
           
            return View();
        }


        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Department (Name, Budget)
                                            OUTPUT INSERTED.Id
                                            VALUES (@Name, @Budget)";

                        cmd.Parameters.Add(new SqlParameter("@Name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@Budget", department.Budget));




                        var id = (int)cmd.ExecuteScalar();
                        department.Id = id;

                        return RedirectToAction(nameof(Index));
                    }
                }


            }
            catch (Exception ex)
            {
                return View();
            }
        }



        private Department GetDepartmentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.[Name], d.Budget, e.FirstName, e.LastName 
                                      FROM Department d 
                                      LEFT JOIN Employee e ON d.Id = e.DepartmentId
                                      WHERE d.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Department department = null;

                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                                DepartmentEmployees = new List<Employee>()
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))

                        {
                            department.DepartmentEmployees.Add(new Employee()

                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))

                            });
                        }
                        else
                        {
                            department.DepartmentEmployees.Add(new Employee()

                            {
                               
                                FirstName = null,
                                LastName = null

                            });
                        }

                    }
                    
                reader.Close();
                return department;
                }
            }
        }
    }
}
