﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Workforce_Purple_Parrots.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Workforce_Purple_Parrots.Models.ViewModels;

namespace Workforce_Purple_Parrots.Controllers
{
    public class ComputersController : Controller
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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

        // GET: Computer
        public ActionResult Index(string searchString)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Make, Model, PurchaseDate, DecomissionDate FROM Computer ";

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        cmd.CommandText += @"WHERE Make LIKE @searchString OR Model LIKE @searchString";
                        cmd.Parameters.Add(new SqlParameter("@searchString", "%" + searchString + "%"));
                    }

                    var reader = cmd.ExecuteReader();
                    var computers = new List<Computer>();

                    while (reader.Read())
                    {
                        var computer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                        };


                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));


                        };

                        computers.Add(computer);
                    }
                    reader.Close();
                    return View(computers);
                }
            }

        }

        // GET: Computer/Details/5
        public ActionResult Details(int id)
        {
            var computer = GetComputerById(id);
            return View(computer);


        }

        // GET: Computer/Create
        public ActionResult Create()
        {
            var employeeOptions = GetEmployeeOptions();
            var viewModel = new ComputerFormViewModel()
            {
                PurchaseDate = DateTime.Now,
                EmployeeOptions = employeeOptions
            };
            return View(viewModel);
        }

        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerFormViewModel computer)
        {
            try
            {
                //if there is an Employee ID value
                //execute an update employee function
                //this will add the new computerId to an existing employee
                //return to indexView
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Computer (Make, Model, PurchaseDate)
                                            OUTPUT INSERTED.Id
                                            VALUES (@make, @model, @purchaseDate)";

                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@model", computer.Model));
                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));


                        var id = (int)cmd.ExecuteScalar();
                        computer.Id = id;
                        if (computer.EmployeeId != 0)
                        {
                            //update employee
                            //I will need, the computer.EmployeeId
                            //and I will need the computer.Id
                            UpdateEmployee(computer.Id, computer.EmployeeId);
                        }

                        return RedirectToAction(nameof(Index));
                    }
                }


            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {
            var computer = GetComputerById(id);

            return View(computer);
        }

        // POST: Computer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Computer WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", computer.Id));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Cannot Delete Computer that has been assigned to a user.";
                return View(computer);
            }
        }

        private List<SelectListItem> GetEmployeeOptions()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, COALESCE(FirstName + ' ' + LastName, 'N/A') as EmployeeName FROM Employee";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("EmployeeName")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }
        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Make, c.Model, c.PurchaseDate, c.DecomissionDate, e.FirstName, e.LastName FROM Computer c 
                                        LEFT JOIN Employee e ON c.Id = e.ComputerId
                                        WHERE c.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Computer computer = null;

                    if (reader.Read())
                    {
                        computer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                        {
                            computer.Employee = new Employee
                            {
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        }

                    };

                    if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                    {
                        computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));

                    };




                    reader.Close();
                    return computer;
                }
            }
        }

        private void UpdateEmployee(int computerId, int employeeId)
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
    }
}