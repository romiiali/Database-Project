using _202Project.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using static _202Project.Pages.Staff.MedicinesModel;

namespace _202Project.Pages.Staff
{
    public class SaleModel : PageModel
    {
        public class SaleItem
        {
            public int saleid { get; set; }
            public int customerid { get; set; }
            public string customerfname { get; set; }
            public string customerlname { get; set; }
            public DateTime saletime { get; set; }

        }

        public List<SaleItem> Sales { get; set; } = new List<SaleItem>();

        static string connectionString = "Data Source = localhost; Initial Catalog = PharmacyDatabase; Integrated Security = True; Trust Server Certificate=True";
        SqlConnection con = new SqlConnection(connectionString);


        public void OnGet()
        {
            string queryString = "SELECT s.SaleId, s.CustomerId, c.FName,c.LName, s.SaleTime from SalesTransaction as s inner join Customer as c on c.CustomerId=s.CustomerId";
            con.Open();
            SqlCommand cmd = new SqlCommand(queryString, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var sale = new SaleItem
                {
                    saleid = reader.GetInt32(0),
                    customerid = reader.GetInt32(1),
                    customerfname = reader.GetString(2),
                    customerlname = reader.GetString(3),
                    saletime = reader.GetDateTime(4)
                };
                Sales.Add(sale);
            }
            ;
            reader.Close();

            con.Close();
        }

        public IActionResult OnPostDelete(int id)
        {
            con.Open();
            string querystring = "Delete from SaleItem where Saleid=@saleid delete from SalesTransaction where SaleId= @saleid ";
            var cmd = new SqlCommand(querystring, con);
            cmd.Parameters.AddWithValue("@saleid", id);
            cmd.ExecuteNonQuery();
            return RedirectToPage();

        }


    }
}
