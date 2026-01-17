using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

using static System.Runtime.InteropServices.JavaScript.JSType;


namespace _202Project.Pages
{
    
    public class IndexModel : PageModel
    {
        static string connectionString="Data Source = localhost; Initial Catalog = PharmacyDatabase; Integrated Security = True; Trust Server Certificate=True";
        

        public void OnGet()
        {

        }
    }
}
