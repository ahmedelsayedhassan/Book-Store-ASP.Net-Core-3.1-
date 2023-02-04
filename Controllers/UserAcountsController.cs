using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Microsoft.Data.SqlClient;

namespace Library.Controllers
{
    public class UserAcountsController : Controller
    {
        private readonly LibraryContext _context;
         
        public UserAcountsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: UserAcounts
        public async Task<IActionResult> Index()
        {
              return _context.UserAcount != null ? 
                          View(await _context.UserAcount.ToListAsync()) :
                          Problem("Entity set 'LibraryContext.UserAcount'  is null.");
        }

        
        // GET: UserAcounts/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult login()
        {
            return View();
        }


        [HttpPost, ActionName("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(string na, string pa)
        {
            SqlConnection conn1 = new SqlConnection(@"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Ahmed Sarhan\\Documents\\NewLib.mdf;Integrated Security=True;Connect Timeout=30");
            string sql;                           ///Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="C:\Users\Ahmed Sarhan\Documents\NewLib.mdf";Integrated Security=True;Connect Timeout=30
            sql = "SELECT * FROM UserAcount where name ='" + na + "' and  pass ='" + pa + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                string role = (string)reader["role"];
                string id = Convert.ToString((int)reader["Id"]);
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("userid", id);
                reader.Close();
                conn1.Close();
                if (role == "customer")
                    return RedirectToAction("catalogue", "books");

                else
                    return RedirectToAction("Index", "books");

            }
            else
            {
                ViewData["Message"] = "wrong user name password";
                return View();
            }
        }


        // POST: UserAcounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,pass,email")] UserAcount userAcount)
        {
            userAcount.role = "customer";
             
                _context.Add(userAcount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(login));
             
        }

        // GET: UserAcounts/Edit/5
        public async Task<IActionResult> Edit()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("userid"));

            var userAcount = await _context.UserAcount.FindAsync(id);
             
            return View(userAcount);
        }

        // POST: UserAcounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,pass,role,email")] UserAcount userAcount)
        {
            
                    _context.Update(userAcount);
                    await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(login));
             
        }

    

        private bool UserAcountExists(int id)
        {
          return (_context.UserAcount?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
