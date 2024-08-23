using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WalmartBackend.Data;
using WalmartBackend.Helpers;
using WalmartBackend.Models;

namespace WalmartBackend.Repositories
{
    public interface ISessionRepo
    {                
        Task<Response> EndSession(int userId, EndSessionModel req);
        Task<List<CheckSessionResponse>> Check(int orderId, int userId);
    }
    public class SessionRepo:ISessionRepo
    {
        ApplicationDbContext _dbContext;
        ICommonHelper _commonHelper;

        #region 
        const string PROC_SESSION_END = "dbo.Proc_Session_End";
        const string PROC_SESSION_CHECK = "Proc_Session_Check";
        #endregion

        public SessionRepo(ApplicationDbContext dbContext, ICommonHelper commonHelper)
        {
            _dbContext = dbContext;
            _commonHelper = commonHelper;
        }

        public async Task<List<CheckSessionResponse>> Check(int orderId, int userId)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(_commonHelper.GetConnectionString());
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();
                sqlComm.CommandText = PROC_SESSION_CHECK;
                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@p_UserId", userId);
                sqlComm.Parameters.AddWithValue("@p_OrderId", orderId);

                SqlDataReader reader = sqlComm.ExecuteReader();

                List<CheckSessionResponse> res=new List<CheckSessionResponse>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CheckSessionResponse curr = new CheckSessionResponse();
                        curr.ObjectName = reader.IsDBNull(reader.GetOrdinal("ObjectName")) ? "" : reader.GetString(reader.GetOrdinal("ObjectName"));
                        curr.Quantity = reader.IsDBNull(reader.GetOrdinal("quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("quantity"));
                        curr.DetectedCount = reader.IsDBNull(reader.GetOrdinal("DetectedCount")) ? 0 : reader.GetInt32(reader.GetOrdinal("DetectedCount"));
                        Console.WriteLine(curr+" "+curr.ObjectName);
                        //ExcessProducts now = new ExcessProducts();
                        

                        //now.Quantity=curr.DetectedCount-curr.Quantity;

                        res.Add(curr);
                    }
                }
                

                return res;
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

        public async Task<Response> EndSession(int userId, EndSessionModel req)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(_commonHelper.GetConnectionString());
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();
                sqlComm.CommandText = PROC_SESSION_END;
                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;
                
                sqlComm.Parameters.AddWithValue("@p_UserId", userId);
                sqlComm.Parameters.AddWithValue("@p_OrderItems", req.OrderItems);

                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                sqlComm.ExecuteNonQuery();
                
                return new Response(true, "Session ended successfully");

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
