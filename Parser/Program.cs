// By David J. Straley, copyright 2024

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using Parser;
using Newtonsoft.Json;
using System.Data.SqlClient;

// Read a JSON file from the web and parse it into a C# object
string url = "https://microsoftedge.github.io/Demos/json-dummy-data/64KB.json";

List<Person> data = await DownloadAndParseJsonAsync(url);
MassageData(ref data);
WriteToDatabase(data);
ReadFromDatabase();

// Download and parse JSON file
async Task<List<Person>> DownloadAndParseJsonAsync(string url)
{
    using (HttpClient client = new HttpClient())
    {
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string jsonContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Person>>(jsonContent);
    }
}

// Use ADO.Net to write records to database
void WriteToDatabase(List<Person> data)
{
    string? connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=Parse;Trusted_Connection=True;";
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        foreach (var item in data)
        {
            //// write to console
            //Console.WriteLine($"Name: {item.name}, Name: {item.id}");

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
    string? connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=Parse;Trusted_Connection=True;";
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        using (SqlCommand command = new SqlCommand("PersonSelectAll", connection))
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Name: {reader["firstname"]}, Name: {reader["lastname"]}");
                }
            }
        }
    }
    return;
}

Console.ReadKey();
