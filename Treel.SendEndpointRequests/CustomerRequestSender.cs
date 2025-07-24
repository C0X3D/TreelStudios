using System.Security.Cryptography;
using System.Text;
using Treel.DataModel;
using Treel.DataModel.Authentication;
using Newtonsoft.Json;
using Treel.DataModel.TransactionModels;

namespace Treel.SendEndpointRequests
{
    public class CustomerRequestSender
    {
        public CustomerRequestSender()
        {
        }

        public async Task<AuthenticationModel> SendUserAuthenticationRequest(CustomerModel customerModel, AuthenticationModel openGameModel)
        {
            AuthenticationModel authenticationModel = new AuthenticationModel();
            var body = JsonConvert.SerializeObject(openGameModel);
            var encripted = CreateHmacSha256(customerModel.Customer_Secret, body);
            using (HttpClient client = new HttpClient())
            {
                // Add the API key to the request headers
                client.DefaultRequestHeaders.Add("customer-api-key", customerModel.Customer_Api_Key);
                client.DefaultRequestHeaders.Add("customer-api-hmac", encripted);

                // Set the content type header to application/json
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Create the content for the POST request
                HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");

                // Send the POST request and get the response
                HttpResponseMessage response = await client.PostAsync(customerModel.Customer_Endpoint + customerModel.Endpoint_User_Authenticate, content);

                // Check if the response is successful
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    authenticationModel = JsonConvert.DeserializeObject<AuthenticationModel>(responseData)!;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception();
                }
                else
                {
                    throw new Exception();
                }
            }
            return authenticationModel ?? throw new Exception();
        }

        public async Task<TreelStudios_BalanceResponseModel> SendPlayerBalanceWithdrawRequestAsync(CustomerModel customerModel, TreelStudios_WithdrawBalanceRequestModel withdrawBalanceRequestModel)
        {
            TreelStudios_BalanceResponseModel authenticationModel = new TreelStudios_BalanceResponseModel();
            var body = JsonConvert.SerializeObject(withdrawBalanceRequestModel);
            var encripted = CreateHmacSha256(customerModel.Customer_Secret, body);
            using (HttpClient client = new HttpClient())
            {
                // Add the API key to the request headers
                client.DefaultRequestHeaders.Add("customer-api-key", customerModel.Customer_Api_Key);
                client.DefaultRequestHeaders.Add("customer-api-hmac", encripted);

                // Set the content type header to application/json
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Create the content for the POST request
                HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");

                // Send the POST request and get the response
                HttpResponseMessage response = await client.PostAsync(customerModel.Customer_Endpoint + customerModel.Endpoint_User_Withdraw_Balance, content);

                // Check if the response is successful
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    authenticationModel = JsonConvert.DeserializeObject<TreelStudios_BalanceResponseModel>(responseData)!;
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception();
                }
                else
                {
                    throw new Exception();
                }
            }
            return authenticationModel ?? throw new Exception();
        }

        public async Task<TreelStudios_BalanceResponseModel> SendPlayerBalanceDepositRequestAsync(CustomerModel customerModel, TreelStudios_DepositBalanceRequestModel depositBalanceRequestModel)
        {
            TreelStudios_BalanceResponseModel authenticationModel = new TreelStudios_BalanceResponseModel();
            var body = JsonConvert.SerializeObject(depositBalanceRequestModel);
            var encripted = CreateHmacSha256(customerModel.Customer_Secret, body);
            using (HttpClient client = new HttpClient())
            {
                // Add the API key to the request headers
                client.DefaultRequestHeaders.Add("customer-api-key", customerModel.Customer_Api_Key);
                client.DefaultRequestHeaders.Add("customer-api-hmac", encripted);

                // Set the content type header to application/json
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Create the content for the POST request
                HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");

                // Send the POST request and get the response
                HttpResponseMessage response = await client.PostAsync(customerModel.Customer_Endpoint + customerModel.Endpoint_User_Deposit_Balance, content);

                // Check if the response is successful
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    authenticationModel = JsonConvert.DeserializeObject<TreelStudios_BalanceResponseModel>(responseData)!;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception();
                }

                else
                {
                    throw new Exception();
                }
            }
            return authenticationModel ?? throw new Exception();
        }

        private static string CreateHmacSha256(string secret, string data)
        {
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(secret);

            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmacsha256.ComputeHash(encoding.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
