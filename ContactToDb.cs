using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace rahul_parasghar.Db
{
    public class ContactToDb
    {
        /* 
         *   Sample Connection String Are Below
         *   With password
         *   "Server=myServerName\myInstanceName;Database=myDataBase;User Id=myUsername;Password=myPassword;"
         *   
         *   With local db 1
         *   "Server=(localdb)\MyInstance;Integrated Security=true;"
         *   
         *   
         *     With local db 2
         *    "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=" + DbName + ";Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False"
         *   
         * 
         * 
         */
        private static string DbName = "Vivek";
        public static void SetConfig(string pconnectionStr)
        {

            ConnectionStr = pconnectionStr;
        }
        private static string ConnectionStr = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=" + DbName + ";Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False";
        public static bool InsertOrUpdateOrDelete(string procedureName, object model)
        {
            var result = false;
            Type t = model.GetType();
            var allproperties = t.GetProperties();
            using (SqlConnection con = new SqlConnection(ConnectionStr))
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    for (int x = 0; x < allproperties.Length; x++)
                    {
                        var prop = allproperties[x];
                        cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(model, null));


                    }
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            result = true;
            return result;
        }
        public static T GetOne<T>(string procedureName, object model=null) where T : new()
        {
            dynamic result = new T();
            var typ = result.GetType();
            var allproperties = typ.GetProperties();
            Type modelType = null;
            PropertyInfo [] Modelallproperties = null;
            

            if (model != null)
            {
                 modelType = model.GetType();
                 Modelallproperties = modelType.GetProperties();
            }
            using (SqlConnection con = new SqlConnection(ConnectionStr))
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (model != null)
                    {
                        for (int x = 0; x < Modelallproperties.Length; x++)
                        {
                            var prop = Modelallproperties[x];
                            cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(model, null));


                        }
                    }
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        for (int x = 0; x < allproperties.Length; x++)
                        {
                            var prop = allproperties[x];
                            prop.SetValue(result, dr[prop.Name]);
                        }



                    }
                }
            }

            return result;
        }
        public static List<T> GetAll<T>(string procedureName, object model=null) where T : new()
        {
            List<T> list = new List<T>();


            Type modelType = null;
            PropertyInfo[] Modelallproperties = null;
            if (model != null)
            {
                modelType = model.GetType();
                Modelallproperties = modelType.GetProperties();
            }

            using (SqlConnection con = new SqlConnection(ConnectionStr))
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (model != null)
                    {
                        for (int x = 0; x < Modelallproperties.Length; x++)
                        {
                            var prop = Modelallproperties[x];
                            cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(model, null));


                        }
                    }
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dynamic result = new T();
                            Type typ = result.GetType();
                            var allproperties = typ.GetProperties();
                            for (int x = 0; x < allproperties.Length; x++)
                            {
                            
                                var prop = allproperties[x];
                                prop.SetValue(result, dr[prop.Name]);
                            }
                            list.Add(result);
                        }



                    }
                }
            }

            return list;
        }

    }
}
