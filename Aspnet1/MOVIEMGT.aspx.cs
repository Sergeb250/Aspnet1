using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Globalization;
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



        protected void editBtn_Click(object sender, EventArgs e)
        {

        }

        protected void deleteBtn_Click(object sender, EventArgs e)
        {

        }

        private void ClearFields()
        {
            nameBox.Text = "";
            directorBox.Text = "";
            descriptionBox.Text = "";
            ratingBox.Text = "";
            ViewState["MovieID"] = null;
        }


        //LOAD MOVIES 


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


     
        protected void movieGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // edit command

            if (e.CommandName == "EditView")
            {
                string movieId = e.CommandArgument.ToString();

                //display movie details in form


                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Movies WHERE MovieId=@movieId", conn);
                    cmd.Parameters.AddWithValue("@movieId", movieId);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    SqlDataReader reader;
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            movieIdlbl.Text = reader["MovieId"].ToString();
                            nameBox.Text = reader["name"].ToString();
                            directorBox.Text = reader["director"].ToString();
                            descriptionBox.Text = reader["description"].ToString();
                            ratingBox.Text = reader["rating"].ToString();
                        }
                    }


                }
                catch (Exception ex)
                {
                    errorlbl.Text = "Error loading movies: " + ex.Message;
                    errorlbl.Visible = true;
                }





            }


            //delete command

            if (e.CommandName == "DeleteMovie")
            {
                try
                {
                    string movieId = e.CommandArgument.ToString();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Movies WHERE MovieId=@movieId", conn);
                    cmd.Parameters.AddWithValue("@movieId", movieId);
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
                    errorlbl.Text = "Error deleting movie: " + ex.Message;
                    errorlbl.Visible = true;
                }

            }




        }

        protected void updateBtn_Click(object sender, EventArgs e)
        {
            try
            {

                


                

                string sqlQuery = "UPDATE Movies SET name=@name, director=@director, description=@description, rating=@rating WHERE MovieId=@movieId";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@name", nameBox.Text.Trim());
                cmd.Parameters.AddWithValue("@director", directorBox.Text.Trim());
                cmd.Parameters.AddWithValue("@description", descriptionBox.Text.Trim());
                cmd.Parameters.AddWithValue("@rating", ratingBox.Text.Trim());
                cmd.Parameters.AddWithValue("@movieId", movieIdlbl.Text.Trim());
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
                errorlbl.Text = "Error: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }

       
    }
}
