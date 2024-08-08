/*
© 2024  David James Straley
*/

using Parser;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

// Obviously this connection string will need to be modified for your SQL Server installation.
string? _connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=Parse;Trusted_Connection=True;";

// Read a JSON file from the web and parse it into a C# object
string url = "https://microsoftedge.github.io/Demos/json-dummy-data/64KB.json";

/* These are environment variables that are set in the "launchSettings.json" file in "Properties" folder.
 * DOTNET_ENVIRONMENT - set to "Development" or "Production" to determine how much information is displayed in the console if there is an exception when running the application.
 * TESTBREAK - set to "True" to cause a test exception to be thrown when downloading the JSON file, otherwise should be set to "False" for normal operation.
 */

string? _environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
string? _testbreak = Environment.GetEnvironmentVariable("TESTBREAK") ?? "False";
try
{
    List<Person> data = await DownloadAndParseJsonAsync(url);
    MassageData(ref data);
    WriteToDatabase(data);
    ReadFromDatabase();
}
catch (Exception ex)
{
    Console.WriteLine($"Error! Exception type: {ex.GetType().Name}");
    Console.WriteLine($"Exception Message: {ex.Message}");
    if(_environment == "Development")
    {
        // This is sensitive information that should not be displayed in production
        Console.WriteLine($"Exception Stack Trace: {ex.StackTrace}");
    }
    Console.WriteLine("Results may be incomplete.");
}
finally
{
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();
}



// Download and parse JSON file
async Task<List<Person>> DownloadAndParseJsonAsync(string url)
{
    using (HttpClient client = new HttpClient())
    {
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        if (_testbreak == "True")
        {
            // This is a test exception to see how the program handles it.
            throw new Exception("This is a test exception when downloading JSON file.");
        }
        string jsonContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Person>>(jsonContent);
    }
}

// Use ADO.Net to write records to database
void WriteToDatabase(List<Person> data)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        connection.Open();
        foreach (var item in data)
        {
            // insert record via a stored procedure 
            using (SqlCommand command = new SqlCommand("PersonInsert", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@name", item.name);
                command.Parameters.AddWithValue("@language", item.language);
                command.Parameters.AddWithValue("@id", item.id);
                command.Parameters.AddWithValue("@bio", item.bio);
                command.Parameters.AddWithValue("@version", item.version);
                command.Parameters.AddWithValue("@firstname", item.firstname);
                command.Parameters.AddWithValue("@lastname", item.lastname);
                command.ExecuteNonQuery();
            }
        }
    }
    return;
}

void MassageData(ref List<Person> data)
{
    string sFullName;
    string[] sWords;

    for (int i = 0; i < data.Count; i++)
    {
        // split the name into first and last name
        sFullName = data[i].name;
        sWords = sFullName.Split(' ');
        data[i].firstname = sWords[0];
        data[i].lastname = sWords[1];
    }
}



// select data from database using stored procedure 
void ReadFromDatabase()
{
    double dVersion = 0.0;
    string sVersion = "";
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        connection.Open();
        using (SqlCommand command = new SqlCommand("PersonSelectAll", connection))
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;
            // we're using a firehose cursor here, i.e. it is considered to be
            // the fastest way to read data from the database.
            // It is a forward-only, read-only cursor.
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    dVersion = (double)reader["version"];
                    sVersion = dVersion.ToString("0.00");
                    Console.WriteLine($"Name: {reader["name"]}   id: {reader["id"]} version: {sVersion}  language: {reader["language"]}");
                    Console.WriteLine($"  bio: {reader["bio"]}");
                    Console.WriteLine();
                }
            }
        }
    }
    return;
}

void ReadFromDatabase2()
{
    // This is just an alternative method to read from the database.  
    // I'm leaving this in here for reference's sake.  This is not used in the program.
    // The primary benefit of this methodology is that it is read/write.
    // The datatable rows and columns can be updated while it is even disconnected from
    // the database manager, and then the changes can be written back to the database
    // via the data adapter once it is reconnected.  This is a very powerful feature of ADO.Net.
    // Read from database into DataTable
    DataTable dt = new();
    using (SqlConnection connection = new (_connectionString))
    {
        using (SqlCommand command = new ("PersonSelectAll", connection))
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;

            using (SqlDataAdapter adapter = new (command))
            {
                connection.Open();
                adapter.Fill(dt);
            }
        }
    }
    for (int i = 0; i < dt.Rows.Count; i++)
    {
        Console.WriteLine($"Name: {dt.Rows[i]["name"]}   id: {dt.Rows[i]["id"]} version: {dt.Rows[i]["version"]} -  language: {dt.Rows[i]["language"]}");
        Console.WriteLine($"  bio: {dt.Rows[i]["bio"]}");
        Console.WriteLine();
    }
    return;
}
