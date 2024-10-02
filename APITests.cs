using Newtonsoft.Json;
using RestSharp;


namespace PlaywrightTests
{
    [TestClass]
    public class APITests
    {
        private readonly string _token = "Basic VGVzdFVzZXI0MzU6VDVIS31KRms9J31j";
        private readonly string _baseUrl = "https://wmxrwq14uc.execute-api.us-east-1.amazonaws.com/Prod/api";

        [TestMethod]
        public async Task Test_CreateEmployee_ReturnsOK()
        {
            // Arrange
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/employees", Method.Post);
            request.AddHeader("Authorization", _token);

            var timestamp = DateTime.Now.ToString("HHmmss");
            var newEmployee = new Employee_Post
            {
                firstName = "John Doe" + timestamp,
                lastName = "Developer" + timestamp,
                dependants = 1
            };

            var jsonBody = JsonConvert.SerializeObject(newEmployee);
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);


            // Act
            var response = await client.ExecuteAsync(request);


            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Expected status code OK");  // Should be 201 actually
            var receivedEmployee = JsonConvert.DeserializeObject<Employee_Get>(response.Content);
            Assert.IsNotNull(receivedEmployee, "Response content should not be null");
            Assert.AreEqual(newEmployee.firstName, receivedEmployee.firstName, "Employee firstName should match");
            Assert.AreEqual(newEmployee.lastName, receivedEmployee.lastName, "Employee lastName should match");
            Assert.IsNotNull(receivedEmployee.id, "Employee id should not be null");
            Assert.IsNotNull(receivedEmployee.salary, "Employee id should not be null");
            Assert.IsNotNull(receivedEmployee.benefitsCost, "Employee id should not be null");
        }

        [TestMethod]
        public async Task Test_GetEmployeeList_ReturnsOK()
        {
            // Arrange
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/employees", Method.Get);
            request.AddHeader("Authorization", _token);

            // Act
            var response = await client.ExecuteAsync(request);


            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Expected status code 200 OK");

            var employees = JsonConvert.DeserializeObject<List<Employee_Get>>(response.Content);

            Assert.IsNotNull(employees, "Response content should not be null");
            Assert.IsInstanceOfType(employees, typeof(List<Employee_Get>), "Expected a list of Employee objects");
            Assert.IsTrue(employees.Count > 0, "Expected at least one employee in the list");
        }

        [TestMethod]
        public async Task Test_UpdateEmployee_ReturnsOK()
        {
            // Arrange
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/employees", Method.Post);
            request.AddHeader("Authorization", _token);

            // Updated computed data (Dependants = 2)
            int salary = 52000;
            int gross = 2000;
            float benefit = 76.92308f;
            float netPay = 1923.0769f;

            var timestamp = DateTime.Now.ToString("HHmmss");
            var newEmployee = new Employee_Post
            {
                firstName = "John Doe" + timestamp,
                lastName = "Developer" + timestamp,
                dependants = 1
            };

            var jsonBody = JsonConvert.SerializeObject(newEmployee);
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            var receivedNewEmployee = JsonConvert.DeserializeObject<Employee_Get>(response.Content);
            Assert.IsNotNull(receivedNewEmployee, "Response content should not be null");


            // Act
            var employeeID = receivedNewEmployee.id;

            var updatedEmployee = new Employee_Put
            {
                id = employeeID,
                firstName = "Updated",
                lastName = "Updated",
                dependants = 2
            };

            request = new RestRequest("/employees", Method.Put);
            request.AddHeader("Authorization", _token);
            jsonBody = JsonConvert.SerializeObject(updatedEmployee);
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            response = await client.ExecuteAsync(request);


            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Expected status code OK");  
            var receivedUpdatedEmployee = JsonConvert.DeserializeObject<Employee_Get>(response.Content);
            Assert.IsNotNull(receivedUpdatedEmployee, "Response content should not be null");
            Assert.AreEqual(updatedEmployee.firstName, receivedUpdatedEmployee.firstName, "Employee firstName should match");
            Assert.AreEqual(updatedEmployee.lastName, receivedUpdatedEmployee.lastName, "Employee lastName should match");
            Assert.AreEqual(updatedEmployee.id, receivedUpdatedEmployee.id, "Employee id should match");
            Assert.AreEqual(updatedEmployee.dependants, receivedUpdatedEmployee.dependants, "Employee dependants should match");
            Assert.AreEqual(salary, receivedUpdatedEmployee.salary, "Employee lastName salary match");
            Assert.AreEqual(gross, receivedUpdatedEmployee.gross, "Employee lastName gross match");
            Assert.AreEqual(benefit, receivedUpdatedEmployee.benefitsCost, "Employee benefitsCost should match");
            Assert.AreEqual(netPay, receivedUpdatedEmployee.net, "Employee net should match");
        }

        private class Employee_Get
        {
            public string? partitionKey;
            public string? sortKey;
            public string? username;
            public string? id;
            public string? firstName;
            public string? lastName;
            public int? dependants;
            public int? salary;
            public int? gross;
            public float? benefitsCost;
            public float? net;
        }

        private class Employee_Post
        {
            public string? firstName;
            public string? lastName;
            public int? dependants;
        }

        private class Employee_Put
        {
            public string? id;
            public string? firstName;
            public string? lastName;
            public int? dependants;
        }
    }
}
