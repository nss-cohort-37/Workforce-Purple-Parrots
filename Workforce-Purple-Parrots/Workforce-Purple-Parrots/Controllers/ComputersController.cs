using System;
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
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Make, Model, PurchaseDate, DecomissionDate FROM Computer";

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
                // TODO: Add insert logic here
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

                        return RedirectToAction(nameof(Index));
                    }
                }


            }
            catch (Exception ex)
            {
                return View();
            }
        }

        //// GET: Computer/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    var computer = GetComputerById(id);
        //    var cohortOptions = GetCohortOptions();
        //    var viewModel = new ComputerEditViewModel()
        //    {
        //        ComputerId = computer.Id,
        //        FirstName = computer.FirstName,
        //        LastName = computer.LastName,
        //        CohortId = computer.CohortId,
        //        SlackHandle = computer.SlackHandle,
        //        Specialty = computer.Specialty,
        //        CohortOptions = cohortOptions

        //    };
        //    return View(viewModel);
        //}

        //// POST: Computer/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, ComputerEditViewModel computer)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"UPDATE Computer 
        //                                    SET FirstName = @firstName, 
        //                                        LastName = @lastName, 
        //                                        SlackHandle = @slackHandle, 
        //                                        CohortId = @cohortId,
        //                                        Specialty = @Specialty
        //                                    WHERE Id = @id";

        //                cmd.Parameters.Add(new SqlParameter("@firstName", computer.FirstName));
        //                cmd.Parameters.Add(new SqlParameter("@lastName", computer.LastName));
        //                cmd.Parameters.Add(new SqlParameter("@slackHandle", computer.SlackHandle));
        //                cmd.Parameters.Add(new SqlParameter("@cohortId", computer.CohortId));
        //                cmd.Parameters.Add(new SqlParameter("@id", id));
        //                cmd.Parameters.Add(new SqlParameter("@specialty", computer.Specialty));

        //                var rowsAffected = cmd.ExecuteNonQuery();

        //                if (rowsAffected < 1)
        //                {
        //                    return NotFound();
        //                }
        //            }
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {
            var computer = GetComputerById(id);
            //click delete button
            //confirmation page comes up
            //a get that will pull the computer information
            //if FirstName is NULL
            //delete the PC
            //else
            //alert the user that is cannot be done and why

            //ways to do this
            //remove delete from anyone who has relationship to a pc

            //

            return View(computer);
        }

        // POST: Computer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Computer computer)
        {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "DELETE FROM Computer WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@id", id));

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

        //private List<SelectListItem> GetCohortOptions()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = "SELECT Id, Name FROM Cohort";

        //            var reader = cmd.ExecuteReader();
        //            var options = new List<SelectListItem>();

        //            while (reader.Read())
        //            {
        //                var option = new SelectListItem()
        //                {
        //                    Text = reader.GetString(reader.GetOrdinal("Name")),
        //                    Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
        //                };
        //                options.Add(option);
        //            }
        //            reader.Close();
        //            return options;
        //        }
        //    }
        //}
    }
}