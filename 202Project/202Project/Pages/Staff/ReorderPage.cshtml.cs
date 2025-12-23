using _202Project.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff
{
    public class ReorderPageModel : PageModel
    {
        // Define classes
        public class SupplierItem
        {
            public int SupplierId { get; set; }
            public string SupplierName { get; set; } = string.Empty;
        }

        public class PurchaseOrderForm
        {
            public int MedicineId { get; set; }
            public int SupplierId { get; set; }
            public int Quantity { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime ExpectedDate { get; set; }
            public int StaffId { get; set; } 
        }

        // Properties
        public List<SupplierItem> Suppliers { get; set; } = new List<SupplierItem>();
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }

        public class StaffItem
        {
            public int StaffId { get; set; }
            public string StaffName { get; set; }
        }
        public List<StaffItem> Staff { get; set; } = new List<StaffItem>();

        [BindProperty]
        public PurchaseOrderForm PurchaseOrder { get; set; } = new PurchaseOrderForm();

        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;TrustServerCertificate=True";

        private void LoadMedicineInfo(int medicineId)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                    SELECT m.MedicineId, m.CommercialName, 
                           ISNULL(SUM(b.QuantityRecieved - ISNULL(s.CurrentValueAlert, 0)), 0) as CurrentStock
                    FROM Medicine m
                    LEFT JOIN Batch b ON m.MedicineId = b.MedicineId
                    join StockAlert as s on m.MedicineId=s.MedicineId
                    WHERE m.MedicineId = @medicineId
                    GROUP BY m.MedicineId, m.CommercialName";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@medicineId", medicineId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            MedicineId = reader.GetInt32(0);
                            MedicineName = reader.GetString(1);
                            CurrentStock = reader.GetInt32(2);
                        }
                    }
                }
            }
        }

        private void LoadSuppliers(int medicineId)
        {
            Suppliers = new List<SupplierItem>();

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                // DEBUG: First check if there are any batches for this medicine
                string debugQuery = "SELECT COUNT(*) FROM Batch WHERE MedicineId = @medicineId";
                using (var debugCmd = new SqlCommand(debugQuery, con))
                {
                    debugCmd.Parameters.AddWithValue("@medicineId", medicineId);
                    int batchCount = Convert.ToInt32(debugCmd.ExecuteScalar());
                    Console.WriteLine($"Batches found for medicine {medicineId}: {batchCount}");
                }

                // Original query with proper column names
                string query = @"
            SELECT DISTINCT s.SupplierId, s.SupplierName 
            FROM Supplier s 
            JOIN Batch b ON s.SupplierId = b.SupplierId 
            WHERE b.MedicineId = @medicineId
            ORDER BY s.SupplierName";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@medicineId", medicineId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Suppliers.Add(new SupplierItem
                            {
                                SupplierId = reader.GetInt32(0),
                                SupplierName = reader.GetString(1)
                            });
                        }
                    }
                }

                // If no suppliers found, try getting ALL suppliers
                if (Suppliers.Count == 0)
                {
                    Console.WriteLine("No suppliers found from batches, loading all suppliers");
                    string allSuppliersQuery = @"
                SELECT SupplierId, SupplierName 
                FROM Supplier 
                ORDER BY SupplierName";

                    using (var cmd = new SqlCommand(allSuppliersQuery, con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Suppliers.Add(new SupplierItem
                                {
                                    SupplierId = reader.GetInt32(0),
                                    SupplierName = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
        }
        private void LoadStaff()
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
            SELECT StaffId, FName, LName 
            FROM PharmacyStaff 
            ORDER BY FName, LName";

                using (var cmd = new SqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Staff.Add(new StaffItem
                            {
                                StaffId = reader.GetInt32(0),
                                StaffName = reader.GetString(1) + " " + reader.GetString(2)
                            });
                        }
                    }
                }
            }
        }

        public void OnGet(int id)
        {
            MedicineId = id;
            LoadMedicineInfo(id);
            LoadSuppliers(id);
            LoadStaff();

            PurchaseOrder.MedicineId = id;
        }

        public IActionResult OnPostReorder()
        {
            if (!ModelState.IsValid)
            {
                LoadMedicineInfo(PurchaseOrder.MedicineId);
                LoadSuppliers(PurchaseOrder.MedicineId);
                LoadStaff();
                return Page();
            }

            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();

                    int newPurchaseId;
                    using (var cmd = new SqlCommand("SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM PurchaseOrder", con))
                    {
                        newPurchaseId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    int newTransId;
                    using (var cmd = new SqlCommand("SELECT ISNULL(MAX(TransactionID), 0) + 1 FROM InventoryTransactions", con))
                    {
                        newTransId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    int medicinePrice = 0;
                    using (var cmd = new SqlCommand("SELECT Price FROM Medicine WHERE MedicineId = @medicineId", con))
                    {
                        cmd.Parameters.AddWithValue("@medicineId", PurchaseOrder.MedicineId);
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            medicinePrice = Convert.ToInt32(result);
                        }
                    }
                    if (medicinePrice <= 0)
                    {
                        TempData["ErrorMessage"] = "Could not retrieve medicine price. Please check medicine details.";
                        LoadMedicineInfo(PurchaseOrder.MedicineId);
                        LoadSuppliers(PurchaseOrder.MedicineId);
                        LoadStaff();
                        return Page();
                    }

                    // Calculate total amount
                    int totalAmount = medicinePrice * PurchaseOrder.Quantity;

                    // Insert into PurchaseOrder
                    string purchasequery = @"
                        INSERT INTO PurchaseOrder (PurchaseId, OrderDate, ExpectedDate, Status, TotalAmount, SupplierId, StaffId) 
                        VALUES (@purchaseId, GETDATE(), @expectedDate, 'Pending', @totalAmount, @supplierId, @staffId)";

                    using (var cmd = new SqlCommand(purchasequery, con))
                    {
                        cmd.Parameters.AddWithValue("@purchaseId", newPurchaseId);
                        cmd.Parameters.AddWithValue("@expectedDate", PurchaseOrder.ExpectedDate);
                        cmd.Parameters.AddWithValue("@totalAmount", totalAmount);
                        cmd.Parameters.AddWithValue("@supplierId", PurchaseOrder.SupplierId);
                        cmd.Parameters.AddWithValue("@staffId", PurchaseOrder.StaffId);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert into InventoryTransactions
                    string inventoryquery = @"
                        INSERT INTO InventoryTransactions (TransactionID, TransactionDate, Quantity, Referenced, PurchaseId, StaffId) 
                        VALUES (@transactionId, GETDATE(), @quantity, ABS(CHECKSUM(NEWID())), @purchaseId, @staffId)";

                    using (var cmd = new SqlCommand(inventoryquery, con))
                    {
                        cmd.Parameters.AddWithValue("@transactionId", newTransId);
                        cmd.Parameters.AddWithValue("@quantity", PurchaseOrder.Quantity);
                        cmd.Parameters.AddWithValue("@purchaseId", newPurchaseId);
                        cmd.Parameters.AddWithValue("@staffId", PurchaseOrder.StaffId);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert into InventoryTransactionType
                    string typequery = "INSERT INTO InventoryTransactionType (TransactionId, TransactionType) VALUES (@transactionId, 'Purchase')";
                    using (var cmd = new SqlCommand(typequery, con))
                    {
                        cmd.Parameters.AddWithValue("@transactionId", newTransId);
                        cmd.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = $"Purchase order #{newPurchaseId} created successfully!";
                    return RedirectToPage("/Staff/Medicines");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating purchase order: {ex.Message}";
                LoadMedicineInfo(PurchaseOrder.MedicineId);
                LoadSuppliers(PurchaseOrder.MedicineId);
                return Page();
            }
        }
    }
}