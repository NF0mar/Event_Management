using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EventManagement
{
    public partial class Login : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnlogin_Click(object sender, EventArgs e)
        {
            string enteredEmail = txtemail.Text.Trim();
            string enteredPassword = txtpassword.Text;

            // Hash and salt the entered password here if your passwords are stored securely.

            conn.Open();

            // Create the SQL query for authentication.
            string query = "SELECT UserID FROM Users WHERE Email = @Email AND Password = @Password";

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                // Add parameters to the query to prevent SQL injection.
                command.Parameters.AddWithValue("@Email", enteredEmail); // Use enteredEmail variable
                command.Parameters.AddWithValue("@Password", enteredPassword); // Use enteredPassword variable

                // Execute the query and check if any rows were returned.
                object result = command.ExecuteScalar();

                if (result != null) // User authenticated successfully.
                {
                    // Redirect to a secure page or perform any other actions.
                    Response.Redirect("index.aspx");
                }
                else
                {
                    // Inform the user that their login credentials are incorrect.
                    lblinfo.Text = "Invalid email or password. Please try again.";
                }
            }
        }
    }
}