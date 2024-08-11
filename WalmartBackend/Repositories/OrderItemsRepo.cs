using Microsoft.Data.SqlClient;
using WalmartBackend.Data;
using WalmartBackend.Helpers;
using WalmartBackend.Models;

namespace WalmartBackend.Repositories
{
    public interface IOrderItemsRepo
    {
        Task<List<OrderItemsResponse>> GetMy(int id);
        Task<Response> AddOrderItem(OrderItems req);
    }
    public class OrderItemsRepo:IOrderItemsRepo
    {
        ApplicationDbContext _dbContext;
        ICommonHelper _commonHelper;

        #region 
        const string PROC_ORDERITEMS_ADD = "dbo.Proc_OrderItems_Add";
        const string PROC_ORDERITEMS_GETMY = "dbo.Proc_OrderItems_GetMy";
        #endregion

        public OrderItemsRepo(ApplicationDbContext dbContext, ICommonHelper commonHelper)
        {
            _dbContext = dbContext;
            _commonHelper = commonHelper;
        }

        public async Task<List<OrderItemsResponse>> GetMy(int id)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(_commonHelper.GetConnectionString());
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();
                sqlComm.CommandText = PROC_ORDERITEMS_GETMY;
                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@p_OrderId", id);                                

                SqlDataReader reader = sqlComm.ExecuteReader();


                List<OrderItemsResponse> response = new List<OrderItemsResponse>();
                Console.WriteLine("rows " + reader.HasRows);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        OrderItemsResponse curr= new OrderItemsResponse();
                        curr.OrderId = reader.IsDBNull(reader.GetOrdinal("OrderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("OrderId"));                        
                        curr.Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id"));                        
                        curr.ProductId = reader.IsDBNull(reader.GetOrdinal("ProductId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProductId"));                        
                        curr.Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity"));
                        curr.ProductImage = reader.IsDBNull(reader.GetOrdinal("ProductImage")) ? "" : reader.GetString(reader.GetOrdinal("ProductImage"));
                        curr.ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? "" : reader.GetString(reader.GetOrdinal("ProductName"));
                        curr.ProductPrice = reader.IsDBNull(reader.GetOrdinal("ProductPrice")) ? 0 : reader.GetFloat(reader.GetOrdinal("ProductPrice"));

                        response.Add(curr);
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

        public async Task<Response> AddOrderItem(OrderItems req)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(_commonHelper.GetConnectionString());
                sqlConn.Open();
                SqlCommand sqlComm = sqlConn.CreateCommand();
                sqlComm.CommandText = PROC_ORDERITEMS_ADD;
                sqlComm.CommandType = System.Data.CommandType.StoredProcedure;
                
                sqlComm.Parameters.AddWithValue("@p_OrderId", req.OrderId);
                sqlComm.Parameters.AddWithValue("@p_ProductId", req.ProductId);
                sqlComm.Parameters.AddWithValue("@p_Quantity", req.Quantity);


                SqlParameter returnParameter = new SqlParameter();
                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlComm.Parameters.Add(returnParameter);

                sqlComm.ExecuteNonQuery();

                int returnValue = returnParameter.Value != DBNull.Value && returnParameter.Value != null ? (int)returnParameter.Value : -1; // Use a default value like -1 for null
                
                if(returnValue == 1)
                {
                    return new Response(false, "Please enter a valid Order Id");
                }
                else if(returnValue == 2) 
                {
                    return new Response(false, "Please enter a valid Product Id");
                }

                return new Response(true, "Added to Cart successfully");

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
