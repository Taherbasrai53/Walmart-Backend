using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using WalmartBackend.Data;
using WalmartBackend.Helpers;
using WalmartBackend.Models;

namespace WalmartBackend.Repositories
{
    public interface ITrollyRepo
    {
        Task<Object> ScanTrolly(int id, int userId);
    }
    public class TrollyRepo: ITrollyRepo
    {
        ApplicationDbContext _dbContext;
        ICommonHelper _commonHelper;

        #region 
        const string PROC_TROLLY_SCAN = "dbo.Proc_Trolly_Scan";
        #endregion
        public TrollyRepo(ApplicationDbContext dbContext, ICommonHelper commonHelper)
        {
            _dbContext = dbContext;
            _commonHelper = commonHelper;
        }

        public async Task<Object> ScanTrolly(int id, int userId)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(_commonHelper.GetConnectionString());
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();
                sqlComm.CommandText = PROC_TROLLY_SCAN;
                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@p_TrollyId", id);
                sqlComm.Parameters.AddWithValue("@p_UserId", userId);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                //sqlComm.ExecuteNonQuery();
                SqlDataReader reader = sqlComm.ExecuteReader();

                int returnValue = returnParameter.Value != DBNull.Value && returnParameter.Value != null ? (int)returnParameter.Value : 0; // Use a default value like -1 for null


                //int returnValue = (int)returnParameter.Value;
                Console.WriteLine("returnValue "+returnValue);
                if (returnValue == 1)
                {
                    return new Response(false, "Trolly already engaged");
                }

                if (returnValue == -1)
                {
                    return new Response(false, "Trolly Does not exist");
                }

                
                TrollyResponse response = new TrollyResponse();
                Console.WriteLine("rows "+reader.HasRows);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response.TrollyId = reader.IsDBNull(reader.GetOrdinal("TrollyId")) ? 0 : reader.GetInt32(reader.GetOrdinal("TrollyId"));
                        Console.WriteLine(response.TrollyId);
                        response.StoreId= reader.IsDBNull(reader.GetOrdinal("StoreId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StoreId"));
                        Console.WriteLine(response.StoreId);
                        response.StoreName = reader.IsDBNull(reader.GetOrdinal("StoreName")) ? null : reader.GetString(reader.GetOrdinal("StoreName"));
                        Console.WriteLine(response.StoreName);
                        response.OrderId= reader.IsDBNull(reader.GetOrdinal("OrderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("OrderId"));
                    }
                }

                return response;
            
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sqlConn != null)
                {
                    try
                    {
                        Console.WriteLine("closing");
                        sqlConn.Close();
                    }
                    catch { }
                }
            }
        }
    }
}
