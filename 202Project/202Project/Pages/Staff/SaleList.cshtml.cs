using _202Project.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using static _202Project.Pages.Staff.MedicinesModel;

namespace _202Project.Pages.Staff
{
    public class SaleListModel : PageModel
    {
        public class SaleItem
        {
            public int saleid { get; set; }
            public int customerid { get; set; }
            public string customerfname { get; set; }
            public string customerlname { get; set; }
            public int TotalAmount { get; set; }
            public string Status {  get; set; }
            public DateTime saletime { get; set; }

        }
        public class ListItem
        {
            public int ItemId { get; set; }
            public int Price { get; set; }
            public string ItemName { get; set; }
            public int Quantity { get; set; }


        }

        public List<SaleItem> Sales { get; set; } = new List<SaleItem>();
        public List<ListItem> Items { get; set; } = new List<ListItem>();

        static string connectionString = "Data Source = localhost; Initial Catalog = PharmacyDatabase; Integrated Security = True; Trust Server Certificate=True";
        SqlConnection con = new SqlConnection(connectionString);
        public void OnGet(int id)
        {
            LoadList(id);
            LoadInfo(id);
        }

        public void LoadList(int id)
        {
            string itemsquery = "SELECT  s.SaleItemId, s.Price,s.QuantitySold,m.CommercialName from SaleItem as s inner join Batch as b on s.BatchId=b.BatchId inner join Medicine as m on b.MedicineId=m.MedicineId where s.SaleId=@id";

            con.Open();
            SqlCommand cmd = new SqlCommand(itemsquery, con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var item = new ListItem
                {
                    ItemId = reader.GetInt32(0),
                    Quantity = reader.GetInt32(1),
                    Price = reader.GetInt32(2),
                    ItemName = reader.GetString(3)
                };
                Items.Add(item);
            }
            ;
            reader.Close();

            con.Close();

        }
        public void LoadInfo(int id)
        {
            string queryString = "SELECT s.SaleId, s.CustomerId, c.FName,c.LName, s.SaleTime,s.TotalAmount,s.SaleStatus from SalesTransaction as s inner join Customer as c on c.CustomerId=s.CustomerId  where s.SaleId=@id";

            con.Open();
            SqlCommand cmd = new SqlCommand(queryString, con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var sale = new SaleItem
                {
                    saleid = reader.GetInt32(0),
                    customerid = reader.GetInt32(1),
                    customerfname = reader.GetString(2),
                    customerlname = reader.GetString(3),
                    saletime = reader.GetDateTime(4),
                    TotalAmount = reader.GetInt32(5),
                    Status = reader.GetString(6)
                };
                Sales.Add(sale);
            }
            ;
            reader.Close();

            con.Close();
        }
    }
}
