using _202Project.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace _202Project.Pages.Staff
{
    public class MedicinesModel : PageModel
    {
        public class MedicineItem
        {
            public int medicineId { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public int CurrentStock { get; set; }
            public DateTime? Expirydate { get; set; }

        }
        
        public List<MedicineItem> Medicines { get; set; } = new List<MedicineItem>();

        static string connectionString = "Data Source = localhost; Initial Catalog = PharmacyDatabase; Integrated Security = True; Trust Server Certificate=True";
        SqlConnection con = new SqlConnection(connectionString);

        public void OnGet()
        {
            string queryString = @"
        SELECT 
            m.MedicineID,
            m.CommercialName,
            c.CategoryName,
            COALESCE(SUM(b.QuantityRecieved), 0) as CurrentStock,
            MIN(b.ExpiryDate) as NearestExpiry
        FROM Medicine m
        LEFT JOIN Category c ON m.CategoryID = c.CategoryID
        LEFT JOIN Batch b ON m.MedicineID = b.MedicineID 
            AND b.ExpiryDate > GETDATE() 
            AND b.QuantityRecieved > 0
        GROUP BY m.MedicineID, m.CommercialName, c.CategoryName
        ORDER BY m.CommercialName;";

            con.Open();
            SqlCommand cmd = new SqlCommand(queryString, con);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var medicine = new MedicineItem
                {
                    medicineId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Category = reader.GetString(2),
                    CurrentStock = reader.GetInt32(3),
                    Expirydate = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                };
                Medicines.Add(medicine);
            }
            ;

            reader.Close();
            con.Close();
        }

        public IActionResult OnPostDelete( int id)
        {
            con.Open();
            string querystring = "Update Batch set MedicineId= NULL where MedicineId=@id Delete from StockAlert where MedicineId=@id Delete from Medicine where MedicineId=@id ";
            var cmd = new SqlCommand(querystring, con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return RedirectToPage();

        }
    }
}
