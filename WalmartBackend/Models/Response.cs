namespace WalmartBackend.Models
{
    public class Response
    {
        public bool success {  get; set; }  
        public string message { get; set; }

        public Response(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }
    }

    public class LoginResponse
    {
        public bool success { get; set; }
        public string token { get; set; }

        public LoginResponse(bool success, string token)
        {
            this.success = success;
            this.token= token;
        }
    }
}
