using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace minilibarary
{
    public class dtaconnection
    {
        private static string connstring = "Data Source=127.0.0.1,1437;Initial Catalog=mylibdb;Integrated Security=True;";
        public static SqlConnection Newconn()
        {
            SqlConnection conn = new SqlConnection(connstring);
            return conn;

        }

        //select queries:
        public static void Showusers()
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Select * From Users",con);
            con.Open();
            SqlDataReader allusers = cmd.ExecuteReader();

            while (allusers.Read())
            {

                Console.WriteLine(allusers["UsersId"].ToString() + "|\t" + allusers["Username"] +
                    "|\t" + allusers["email"] + "|\t" + allusers["joined"]);
            }
            con.Close();
        }
        public static void Showbooks()
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Select * From Book", con);
            con.Open();
            SqlDataReader allbooks = cmd.ExecuteReader();

            while(allbooks.Read())
            {
                Console.WriteLine(allbooks["BookId"].ToString() + "|\t" + allbooks["Title"] +
                    "|\t" + allbooks["Author"] + "|\t" + allbooks["Genre"]);
            }
            con.Close();

        }
        public static void ausershelf(int userid)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Select distinct Shelf.ShelffName From Shelf" +
                " Inner Join BookShelf on Shelf.ShelfId = BookShelf.ShelfId Where Shelf.ShelfId=@ShelfId;", con);
            cmd.Parameters.AddWithValue("@ShelfId", userid);

            con.Open();
            SqlDataReader usershelf = cmd.ExecuteReader();

            while (usershelf.Read())
            {
                Console.WriteLine(usershelf["ShelffName"]);
            }

            con.Close();
        }
        public static void userbookcount(int userid)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("select Count(distinct booksnum.BookId) as booksnum" +
                " From (Select Book.BookId From Book Inner Join BookShelf on Book.BookId= BookShelf.BookId " +
                "where BookShelf.UserId=@UserID) as booksnum;", con);
            cmd.Parameters.AddWithValue("@UserID", userid);
            con.Open();
            SqlDataReader dta = cmd.ExecuteReader();

            while (dta.Read())
            {
                Console.WriteLine(dta["booksnum"]);

            }
            con.Close();

        }

        public static void alluserbooksnum(int userid)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Select Count(distinct [dbo].[BookShelf].BookId) as booknum From [dbo].[Book]" +
                " inner join [dbo].[BookShelf] on [dbo].[Book].BookId=[dbo].[BookShelf].BookId where UserId=@Userid", con);
            cmd.Parameters.AddWithValue("@Userid", userid);
            con.Open();
            SqlDataReader dta = cmd.ExecuteReader();

            while (dta.Read())
            {
                Console.WriteLine(dta["booknum"]);

            }
            con.Close();

        }
        public static void allusersbookreadornot(int status)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Select UsersId,Username,count(BookId) as booknum From [dbo].[Users] inner" +
                " join [dbo].[BookShelf] on [dbo].[Users].UsersId=[dbo].[BookShelf].UserId where BookStatus=@status " +
                "group by UsersId,Username",con);
            cmd.Parameters.AddWithValue("@status",status);
            con.Open();
            SqlDataReader dta = cmd.ExecuteReader();
            if (status == 1)
            {
                Console.WriteLine("users finished this amount of books :");
            }
            else
            {
                Console.WriteLine("users are reading this amont of books :");

            }
            while (dta.Read())
            {
                
                Console.WriteLine(dta["UsersId"]+"\t"+ dta["Username"]+"\t"+ dta["booknum"]);

            }
            con.Close();

        }

        public static void shelfbasebooknum()
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("SELECT [dbo].[Users].Username, COUNT([dbo].[BookShelf].BookId) AS total_books" +
                " FROM [dbo].[Users] LEFT JOIN [dbo].[BookShelf] ON [dbo].[Users].UsersId = BOOKSHELF.UserId" +
                " GROUP BY [dbo].[Users].Username order by total_books desc",con);
            con.Open();
            SqlDataReader dta = cmd.ExecuteReader();
            while (dta.Read())
            {
                Console.WriteLine(dta["Username"] +"\t"+ dta["total_books"]);

            }
            con.Close();

        }
        public static void usersatlastonebook()
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Select distinct Users.UsersId,Username ,Count(BookShelf.BookId)" +
                " as numbook From Users inner join BookShelf on Users.UsersId=BookShelf.UserId where BookStatus=0 " +
                "Group by Users.Usersid,Username order by numbook desc", con);
            con.Open();
            SqlDataReader dta = cmd.ExecuteReader();
            while (dta.Read())
            {
                Console.WriteLine(dta["UsersId"] + "\t" + dta["Username"]+ "\t" + dta["numbook"] );

            }
            con.Close();

        }
        public static void mostreadbooks()
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Select Top 10 Book.BookId,Title,Author,Genre,count(BookShelf.BookId) as inshelf" +
                " From Book inner join BookShelf on Book.BookId= BookShelf.BookId group by Book.BookId,Title,Author,Genre" +
                " order by inshelf desc;", con);
            con.Open();
            SqlDataReader dta = cmd.ExecuteReader();

            while (dta.Read())
            {
                Console.WriteLine(dta["BookId"] + "\t" + dta["Title"] + "\t" + dta["Author"] + "\t" +
                    dta["Genre"] + "\t" + dta["inshelf"]);

            }
            con.Close();

        }


        //  we use The above method to learn the process better way to run queries :
        public static SqlDataReader bettherway(string query)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader dta = cmd.ExecuteReader();
            return dta;
        }

        // Insert queries

        public static int Newuser(string username,string email)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Insert Into Users(Username,email,joined) Values(@Username,@Email,@Joined)",con);
            cmd.Parameters.AddWithValue("@Username",username);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Joined", DateTime.Now);

            con.Open();
            int result = cmd.ExecuteNonQuery();
            return result;
        }

        //update queries 
        
        public static int updateuser(int userid, string username)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Update Users set Username=@username where UsersId=@userid", con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@userid", userid);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            return result;
        }
        public static int updateuser(int userid,string username,string email)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Update Users set Username=@username,email=@email where UsersId=@userid", con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@userid", userid);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            return result;
        }


        //delete queries

        public static int deleteuser(int userid)
        {
            SqlConnection con = Newconn();
            SqlCommand cmd = new SqlCommand("Delete From Users where UsersId=@userid", con);
            cmd.Parameters.AddWithValue("@userid", userid);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            return result;
        }

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("users :");
            dtaconnection.Showusers();
            Console.WriteLine("Books :");
            dtaconnection.Showbooks();
            Console.WriteLine("user shelf count :");
            dtaconnection.ausershelf(1);
            Console.WriteLine("user books count :");

            dtaconnection.userbookcount(1);

            Console.WriteLine("user booksin read count :");

            dtaconnection.alluserbooksnum(1);


            //1 for finished books 0 for in progress
            Console.WriteLine("All users books reading:");

            dtaconnection.allusersbookreadornot(1);


            Console.WriteLine("users order by most finished books");

            dtaconnection.shelfbasebooknum();
            


            Console.WriteLine("users with at last one book in shelf in reading status");

            dtaconnection.usersatlastonebook();


            Console.WriteLine("top 10 most read books : ");

            dtaconnection.mostreadbooks();


            dtaconnection.Newuser("lawliet", "lawliet@gmail.com");
            Console.WriteLine(dtaconnection.updateuser(1,"sa","sa"));
            //uncomment to delete a user
            dtaconnection.deleteuser(5);
            Console.ReadLine();

        }
    }
}
