using Microsoft.Data.SqlClient;
using WalmartBackend.Data;
using WalmartBackend.Helpers;
using WalmartBackend.Models;

namespace WalmartBackend.Repositories
{
    public interface ISessionRepo
    {        
        Task<Response> EndSession(int userId);
    }
    public class SessionRepo:ISessionRepo
    {
        ApplicationDbContext _dbContext;
        ICommonHelper _commonHelper;

        #region 
        const string PROC_SESSION_END = "dbo.Proc_Session_End";
        #endregion

        public SessionRepo(ApplicationDbContext dbContext, ICommonHelper commonHelper)
        {
            _dbContext = dbContext;
            _commonHelper = commonHelper;
        }

        public async Task<Response> EndSession(int userId)
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
