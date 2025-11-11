using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Aspnet1
{
    public partial class MOVIEMGT : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["movieConnString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMovies();
            }
        }

        private void LoadMovies()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Movies", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                movieGridView.DataSource = dt;
                movieGridView.DataBind();
            }
            catch (Exception ex)
            {
                errorlbl.Text = "Error loading movies: " + ex.Message;
                errorlbl.Visible = true;
            }
        }

        protected void saveBox_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlQuery = "INSERT INTO Movies (name, director, description, rating) VALUES (@name, @director, @description, @rating)";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@name", nameBox.Text.Trim());
                cmd.Parameters.AddWithValue("@director", directorBox.Text.Trim());
                cmd.Parameters.AddWithValue("@description", descriptionBox.Text.Trim());
                cmd.Parameters.AddWithValue("@rating", ratingBox.Text.Trim());

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                int row = cmd.ExecuteNonQuery();

                if (row > 0)
                {
                    messagelbl.Visible = true;
                    messagelbl.Text = "Movie saved successfully!";
                    errorlbl.Visible = false;
                    LoadMovies();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                errorlbl.Visible = true;
                errorlbl.Text = "Error: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }

        protected void movieGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = movieGridView.SelectedRow;
            nameBox.Text = row.Cells[2].Text;       // 0: select btn, 1: ID, 2: Name
            directorBox.Text = row.Cells[3].Text;
            descriptionBox.Text = row.Cells[4].Text;
            ratingBox.Text = row.Cells[5].Text;

            ViewState["MovieID"] = movieGridView.DataKeys[row.RowIndex].Value;
        }

        protected void editBtn_Click(object sender, EventArgs e)
        {
            if (ViewState["MovieID"] == null)
            {
                errorlbl.Visible = true;
                errorlbl.Text = "Please select a movie to edit.";
                return;
            }

            try
            {
                string sqlQuery = "UPDATE Movies SET name=@name, director=@director, description=@description, rating=@rating WHERE movieId=@movieId";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@name", nameBox.Text.Trim());
                cmd.Parameters.AddWithValue("@director", directorBox.Text.Trim());
                cmd.Parameters.AddWithValue("@description", descriptionBox.Text.Trim());
                cmd.Parameters.AddWithValue("@rating", ratingBox.Text.Trim());
                cmd.Parameters.AddWithValue("@movieId", ViewState["MovieID"]);

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                int row = cmd.ExecuteNonQuery();
                if (row > 0)
                {
                    messagelbl.Visible = true;
                    messagelbl.Text = "Movie updated successfully!";
                    errorlbl.Visible = false;
                    LoadMovies();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                errorlbl.Visible = true;
                errorlbl.Text = "Error updating movie: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }

        protected void deleteBtn_Click(object sender, EventArgs e)
        {
            if (ViewState["MovieID"] == null)
            {
                errorlbl.Visible = true;
                errorlbl.Text = "Please select a movie to delete.";
                return;
            }

            try
            {
                string sqlQuery = "DELETE FROM Movies WHERE movieId=@movieId";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@movieId", ViewState["MovieID"]);

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                int row = cmd.ExecuteNonQuery();
                if (row > 0)
                {
                    messagelbl.Visible = true;
                    messagelbl.Text = "Movie deleted successfully!";
                    errorlbl.Visible = false;
                    LoadMovies();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                errorlbl.Visible = true;
                errorlbl.Text = "Error deleting movie: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }

        private void ClearFields()
        {
            nameBox.Text = "";
            directorBox.Text = "";
            descriptionBox.Text = "";
            ratingBox.Text = "";
            ViewState["MovieID"] = null;
        }
    }
}
