using Oracle.ManagedDataAccess.Client;
using QLBG.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBG.Controllers
{
    public class AccountOracleController : Controller
    {
        private string connectionString;

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserId, string Password)
        {
            string connectionString = $"User Id={UserId};Password={Password};Data Source=localhost:1521/orcl;DBA Privilege=SYSDBA";

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    DataTable users = new DataTable();
                    DataTable privileges = new DataTable();

                    // Lấy danh sách người dùng
                    string usersQuery = "SELECT username FROM all_users ORDER BY username";
                    using (OracleCommand command = new OracleCommand(usersQuery, connection))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(users);
                        }
                    }

                    // Lấy quyền cho từng người dùng
                    string privilegesQuery = "SELECT grantee, privilege FROM dba_sys_privs ORDER BY grantee";
                    using (OracleCommand command = new OracleCommand(privilegesQuery, connection))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(privileges);
                        }
                    }

                    ViewBag.Users = users;
                    ViewBag.Privileges = privileges;
                }
                ViewBag.Message = "Đăng nhập thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Đăng nhập thất bại: " + ex.Message;
            }

            return View("GetData");
        }


        public ActionResult GetData(string UserId, string Password)
        {
            string connectionString = $"User Id={UserId};Password={Password};Data Source=localhost:1521/orcl;DBA Privilege=SYSDBA";

            DataTable users = new DataTable();
            DataTable privileges = new DataTable();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Lấy danh sách người dùng
                    string usersQuery = "SELECT username FROM all_users ORDER BY username";
                    using (OracleCommand command = new OracleCommand(usersQuery, connection))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(users);
                        }
                    }

                    // Lấy quyền cho từng người dùng
                    string privilegesQuery = "SELECT grantee, privilege FROM dba_sys_privs ORDER BY grantee";
                    using (OracleCommand command = new OracleCommand(privilegesQuery, connection))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(privileges);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Lấy dữ liệu thất bại: " + ex.Message;
                }
            }

            ViewBag.Users = users;
            ViewBag.Privileges = privileges;

            return View();
        }
        [HttpGet]
        public ActionResult ViewPrivileges(string username)
        {
            ViewBag.Username = username;
            List<string> privileges = new List<string>();
            string connectionString = $"User Id=SYS;Password=1234;Data Source=localhost:1521/orcl;DBA Privilege=SYSDBA";
            string query = $"SELECT privilege FROM dba_sys_privs WHERE grantee = '{username.ToUpper()}'";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                privileges.Add(reader.GetString(0));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }

            ViewBag.Privileges = privileges;
            return View();
        }


        [HttpGet]
        public ActionResult ListUsers()
        {
            List<string> users = new List<string>();
            string query = "SELECT username FROM all_users";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(reader.GetString(0));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }

            ViewBag.Users = users;
            return View();
        }
        [HttpGet]
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(string Username, string Password)
        {
            string connectionString = $"User Id=SYS;Password=1234;Data Source=localhost:1521/orcl;DBA Privilege=SYSDBA";
            string query = $"CREATE USER {Username} IDENTIFIED BY {Password}";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    ViewBag.Message = "User created successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }

            return RedirectToAction("GetData", new { UserId = "SYS", Password = "1234" });
        }

        [HttpPost]
        public ActionResult DeleteUser(string Username)
        {
            string connectionString = $"User Id=SYS;Password=1234;Data Source=localhost:1521/orcl;DBA Privilege=SYSDBA";
            string query = $"DROP USER {Username} CASCADE";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    ViewBag.Message = "User deleted successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }

            return RedirectToAction("GetData", new { UserId = "SYS", Password = "1234" });
        }




        [HttpGet]
        public ActionResult ManagePrivileges(string username)
        {
            ViewBag.Username = username;
            List<string> privileges = new List<string>();
            string connectionString = $"User Id=SYS;Password=1234;Data Source=localhost:1521/orcl;DBA Privilege=SYSDBA";
            string query = $"SELECT privilege FROM dba_sys_privs WHERE grantee = '{username.ToUpper()}'";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                privileges.Add(reader.GetString(0));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }

            ViewBag.Privileges = privileges;
            return View();
        }


        [HttpPost]
        public ActionResult UpdatePrivileges(string Username, string Privilege)
        {
            string connectionString = $"User Id=SYS;Password=1234;Data Source=localhost:1521/orcl;DBA Privilege=SYSDBA";
            string query = $"GRANT {Privilege} TO {Username}";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    ViewBag.Message = $"Privilege '{Privilege}' granted to user '{Username}' successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Error granting privilege '{Privilege}' to user '{Username}': {ex.Message}";
                }
            }

            return RedirectToAction("ManagePrivileges", new { username = Username });
        }


        [HttpPost]
        public ActionResult DeletePrivilege(string Username, string Privilege)
        {
            string connectionString = $"User Id=SYS;Password=1234;Data Source=localhost:1521/orcl;DBA Privilege=SYSDBA";
            string query = $"REVOKE {Privilege} FROM {Username}";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    ViewBag.Message = "Privilege revoked successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }

            return RedirectToAction("ManagePrivileges", new { username = Username });
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
