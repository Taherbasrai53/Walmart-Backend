namespace WalmartBackend.Helpers
{

    public interface ICommonHelper
    {
        string GetConnectionString();
    }

    public class CommonHelper:ICommonHelper
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public CommonHelper(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("StagingConnection");
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

    }
}
