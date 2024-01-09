using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EventManagement
{
    public partial class Payments : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection("Data Source = NUR\\SQLEXPRESS;initial catalog=eventManagement_db; integrated security=true");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Fetch the data from the "Users" table
                string query = "SELECT OrderID, UserID FROM Orders"; // Query to fetch user data

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Bind the data to the DropDownList
                    ddlOrderID.DataSource = reader;
                    ddlOrderID.DataTextField = "UserID"; // Display this field's values
                    ddlOrderID.DataValueField = "OrderID"; // Use this field's values as the selected value
                    ddlOrderID.DataBind();

                    conn.Close();
                }

                // Add an initial item (optional)
                ddlOrderID.Items.Insert(0, new ListItem("Select OrderID", ""));
            }
        }

        protected void btndelete_Click(object sender, EventArgs e)
        {
            conn.Open();
            String del = "delete from Payments where PaymentID  = '" + txtPaymentID.Text + "'";
            SqlCommand cmd = new SqlCommand(del, conn);
            cmd.ExecuteNonQuery();
            lblinfo.Text = "payment is deleted";
            conn.Close();
        }
    }
}