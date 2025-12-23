using _202Project.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using static _202Project.Pages.Staff.MedicinesModel;


namespace _202Project.Pages.Staff
{
    public class StocksModel : PageModel
    {
        static string connectionString = "Data Source = localhost; Initial Catalog = PharmacyDatabase; Integrated Security = True; Trust Server Certificate=True";
        SqlConnection con = new SqlConnection(connectionString);
        public class LowMedicine
        {
            public int medicineId { get; set; }
            public string CommercialName { get; set; }
            public string CategoryName { get; set; }
            public int Stock { get; set; }
            public string? SupplierName { get; set; }
            public string AlertType { get; set; }
            public DateTime AlertDate { get; set; }
            public DateTime? ExpiryDate { get; set; }
        }
        public List<LowMedicine> Medicines { get; set; } = new List<LowMedicine>();

        public void OnGet()
        {
            string queryString = @"SELECT 
                    sa.MedicineId, 
                    COALESCE(m.CommercialName, 'No Medicine Linked') AS MedicineCommercialName, 
                    COALESCE(c.CategoryName, 'No Category') AS MedicineCategoryName, 
                    sa.CurrentValueAlert AS Stock, 
                    COALESCE(s.SupplierName, 'No Supplier') AS SupplierName, 
                    sa.AlertType, 
                    sa.AlertDateTime, 
                    b.ExpiryDate 
                FROM StockAlert sa 
                LEFT JOIN Medicine m ON sa.MedicineId = m.MedicineId 
                LEFT JOIN Category c ON m.CategoryId = c.CategoryId 
                LEFT JOIN Batch b ON sa.BatchId = b.BatchId 
                LEFT JOIN Supplier s ON b.SupplierId = s.SupplierId 
                WHERE sa.CurrentValueAlert <= sa.ThresholdValue AND sa.Resolved = 0 
                ORDER BY sa.AlertDateTime DESC";
            con.Open();
            SqlCommand cmd = new SqlCommand(queryString, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var medicine = new LowMedicine
                {
                    medicineId = reader.GetInt32(0),
                    CommercialName = reader.GetString(1),
                    CategoryName = reader.GetString(2),
                    Stock = reader.GetInt32(3),
                    SupplierName = reader.IsDBNull(4) ? null : reader.GetString(4),
                    AlertType = reader.GetString(5),
                    AlertDate = reader.GetDateTime(6),
                    ExpiryDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                };
                Medicines.Add(medicine);
            };
            reader.Close();

            con.Close();
        }
    }
}
