using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

using static System.Runtime.InteropServices.JavaScript.JSType;


namespace _202Project.Pages
{
    
    public class IndexModel : PageModel
    {
        static string connectionString="Data Source = localhost; Initial Catalog = PharmacyDatabase; Integrated Security = True; Trust Server Certificate=True";
        
        public int useCount { get; set; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
             SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string queryS = "Select Count(*) from Customer";
            SqlCommand countCommand = new SqlCommand(queryS, con);

            useCount = (int)countCommand.ExecuteScalar();
            con.Close();

        }
    }
}
