using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public class DashBoardTests : PageTest
{
    private readonly string _userName = "TestUser435";
    private readonly string _password = "T5HK}JFk='}c";
    private readonly string _loginUrl = "https://wmxrwq14uc.execute-api.us-east-1.amazonaws.com/Prod/Account/Login";

    [TestMethod]
    public async Task UserLogin()
    {
        await LoginToDashBoard();
    }

    [TestMethod]
    public async Task AddEmployee()
    {
        /// Arrange
        await LoginToDashBoard();

        var timestamp = DateTime.Now.ToString("HHmmss");
        var firstName = "FirstNameTest_" + timestamp;
        var lastName = "LastNameTest_" + timestamp;
        var dependantsNo = "2";
        var salary = "52000.00";
        var gross = "2000.00";
        var benefit = "76.92";
        var netPay = "1923.08";


        /// Act
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Click the Add Employee button
        await Page.ClickAsync("#add");

        await Page.Locator("#employeeModal").WaitForAsync();

        // Fill data
        await Page.FillAsync("#firstName", firstName);
        await Page.FillAsync("#lastName", lastName);
        await Page.FillAsync("#dependants", dependantsNo);

        // Click Add button
        await Page.ClickAsync("#addEmployee");

        // Wait till new record is displayed
        await Page.WaitForSelectorAsync($"tr:has-text('{firstName}')");

        // Take new row and extract cells
        var row = await Page.QuerySelectorAsync($"tr:has-text('{firstName}')");
        var matchingCells = new List<IElementHandle>();
        var cells = await row.QuerySelectorAllAsync("td");
        matchingCells.AddRange(cells);


        /// Assert 
        Assert.IsNotNull(row, "New Employee row not found in Employee Table.");
        Assert.IsTrue(matchingCells.Any(), "New Employee not added to Employee Table.");
        Assert.AreEqual(await matchingCells[3].InnerTextAsync(), dependantsNo, "Dependants number not equal.");
        Assert.AreEqual(await matchingCells[4].InnerTextAsync(), salary, "Salary not equal.");
        Assert.AreEqual(await matchingCells[5].InnerTextAsync(), gross, "Gross not equal.");
        Assert.AreEqual(await matchingCells[6].InnerTextAsync(), benefit, "Benefit not equal.");
        Assert.AreEqual(await matchingCells[7].InnerTextAsync(), netPay, "Net Pay not equal.");


        /// Clear test data
        await ClearTestData(firstName);
    }

    [TestMethod]
    public async Task UpdateEmployee()
    {
        /// Arrange
        await LoginToDashBoard();

        // New record data
        var timestamp = DateTime.Now.ToString("HHmmss");
        var firstName = "FirstNameTest_" + timestamp;
        var lastName = "LastNameTest_" + timestamp;
        var dependantsNo = "1";

        // Updated data
        var updatedFirstName = firstName + "edit";
        var updatedLastName = lastName + "edit";
        var updatedDependantsNo = "2";

        // Define computed data after update (Dependants = 2)
        var salary = "52000.00";
        var gross = "2000.00";
        var benefit = "76.92";
        var netPay = "1923.08";

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Add record
        await Page.ClickAsync("#add");

        await Page.Locator("#employeeModal").WaitForAsync();

        await Page.FillAsync("#firstName", firstName);
        await Page.FillAsync("#lastName", lastName);
        await Page.FillAsync("#dependants", dependantsNo);

        await Page.ClickAsync("#addEmployee");

        // Wait till new record is displayed
        await Page.WaitForSelectorAsync($"tr:has-text('{firstName}')");


        /// Act
        // Take new row and extract cells 
        var row = await Page.QuerySelectorAsync($"tr:has-text('{firstName}')");
        var matchingCells = new List<IElementHandle>();
        var cells = await row.QuerySelectorAllAsync("td");
        matchingCells.AddRange(cells);

        // Edit record
        var editButton = await matchingCells[8].QuerySelectorAsync("i.fa-edit");
        Assert.IsNotNull(editButton, "Edit button not found.");
        await editButton.ClickAsync();

        await Page.Locator("#employeeModal").WaitForAsync();

        await Page.FillAsync("#firstName", updatedFirstName);
        await Page.FillAsync("#lastName", updatedLastName);
        await Page.FillAsync("#dependants", updatedDependantsNo);

        await Page.ClickAsync("#updateEmployee");

        // Wait till updated record is displayed
        await Page.WaitForSelectorAsync($"tr:has-text('{updatedFirstName}')");

        // Take updated row and extract cells 
        row = await Page.QuerySelectorAsync($"tr:has-text('{updatedFirstName}')");
        matchingCells = new List<IElementHandle>();
        cells = await row.QuerySelectorAllAsync("td");
        matchingCells.AddRange(cells);


        /// Assert 
        Assert.IsNotNull(row, "Updated Employee row not found in Employee Table.");
        Assert.IsTrue(matchingCells.Any(), "Updated Employee not added to Employee Table.");
        Assert.AreEqual(await matchingCells[3].InnerTextAsync(), updatedDependantsNo, "Dependants number not equal.");
        Assert.AreEqual(await matchingCells[4].InnerTextAsync(), salary, "Salary not equal.");
        Assert.AreEqual(await matchingCells[5].InnerTextAsync(), gross, "Gross not equal.");
        Assert.AreEqual(await matchingCells[6].InnerTextAsync(), benefit, "Benefit not equal.");
        Assert.AreEqual(await matchingCells[7].InnerTextAsync(), netPay, "Net Pay not equal.");


        /// Clear test data
        await ClearTestData(firstName);
    }


    // Helper method for clearing test data from table
    private async Task ClearTestData(string targetFirstName)
    {
        // Find row and click delete
        await Page.ClickAsync($"tr:has-text('{targetFirstName}') i.fa-times");

        // Wait for delete modal
        await Page.Locator("#deleteModal").WaitForAsync();

        // Delete record
        await Page.ClickAsync("#deleteEmployee");
    }

    // Helper method for logging in
    private async Task LoginToDashBoard()
    {
        // Replace with your actual login page URL
        await Page.GotoAsync(_loginUrl);

        // Fill in username and password
        await Page.FillAsync("#Username", _userName);
        await Page.FillAsync("#Password", _password);

        // Click the login button
        await Page.ClickAsync("text=\"Log In\"");

        var tableLocator = Page.Locator("#employeesTable");
        await tableLocator.WaitForAsync();
        var thLocator = tableLocator.Locator("thead th");
        var headerHandles = await thLocator.ElementHandlesAsync();

        // Check if success by checking table header
        Assert.AreEqual(9, headerHandles.Count(), "Table header not correctly loaded, not succesfull with login.");
    }
}