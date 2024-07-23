using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using RabbitMQ.Client;
using System.Diagnostics;
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using Shared.Models;
using Shared.Services;

namespace InfoCSV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase, IDisposable
    {
        private readonly string _mySqlConnectionStr;
        private readonly IConnection _rabbitMqConnection;
        private readonly IModel _rabbitMqChannel;
        private readonly ILogger<UsersController> _logger;
        private readonly FileStateService _fileStateService;

        public UsersController(MySqlConnection mySqlConnection, IMongoDatabase mongoDatabase, ILogger<UsersController> logger, FileStateService fileStateService)
        {
            _mySqlConnectionStr = mySqlConnection.ConnectionString;
            _logger = logger;
            _fileStateService = fileStateService;

            var factory = new ConnectionFactory() { HostName = "localhost" };
            _rabbitMqConnection = factory.CreateConnection();
            _rabbitMqChannel = _rabbitMqConnection.CreateModel();
            _rabbitMqChannel.QueueDeclare(queue: "csv_queue",
                                          durable: false,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);
        }

        [HttpGet]
        public IActionResult TestDatabaseConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(_mySqlConnectionStr))
                {
                    connection.Open();
                    connection.Close();
                }
                _logger.LogInformation("MySQL database connection test successful.");
                return Ok("MySQL database connection test successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"MySQL database connection error: {ex.Message}");
                return StatusCode(500, $"MySQL database connection error: {ex.Message}");
            }
        }

        [HttpGet("fetch")]
        public IActionResult FetchAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (var connection = new MySqlConnection(_mySqlConnectionStr))
                {
                    connection.Open();
                    string query = "SELECT * FROM users";
                    using (var command = new MySqlCommand(query, connection))
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    connection.Close();
                }

                var jsonResult = JsonConvert.SerializeObject(dataTable);
                return Ok(jsonResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching data from MySQL: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email ID is required.");
            }

            try
            {
                using (var connection = new MySqlConnection(_mySqlConnectionStr))
                {
                    connection.Open();
                    string query = "DELETE FROM users WHERE email_id = @Email";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        int rowsAffected = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowsAffected > 0)
                        {
                            _logger.LogInformation($"Record with Email ID {email} deleted successfully from MySQL.");
                            return Ok($"Record with Email ID {email} deleted successfully from MySQL.");
                        }
                        else
                        {
                            _logger.LogWarning($"No record found with Email ID {email} in MySQL.");
                            return NotFound($"No record found with Email ID {email} in MySQL.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting data from MySQL: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload")]
public async Task<IActionResult> UploadCsv(IFormFile file)
{
    var stopwatch = Stopwatch.StartNew();

    if (file == null || file.Length == 0)
    {
        return BadRequest("No file uploaded.");
    }

    try
    {
        var dataTable = new DataTable();
        using (var reader = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)))
        {
            using (var dr = new CsvDataReader(csv))
            {
                dataTable.Load(dr);
            }
        }

        // Generate a unique fileId and create a new FileState object
        var fileId = file.GetHashCode();
        var fileState = new FileState
        {
            FileId = fileId.ToString(),
            FileName = file.FileName,
            FileStatus = "Uploaded",
            IsProcessed = false,
            DateProcessed = DateTime.Now,
            BatchesProcessed = 0,
            TotalBatches = (int)Math.Ceiling((double)dataTable.Rows.Count / 10000)
        };

        // Save file state to MongoDB using FileStateService
        await _fileStateService.InsertFileStateAsync(fileState);

        // Include fileId in the message
        var message = new
        {
            FileId = fileId,
            Data = dataTable
        };

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);
        _rabbitMqChannel.BasicPublish(exchange: "",
                                      routingKey: "csv_queue",
                                      basicProperties: null,
                                      body: body);

        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        _logger.LogInformation($"Uploaded file processed in {elapsedMilliseconds} ms");
        await _fileStateService.UpdateFileStateAsync(fileState.FileId, "Queued");
        return Ok("Data uploaded and queued successfully.");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Internal server error: {ex.Message}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}


        public void Dispose()
        {
            _rabbitMqChannel?.Close();
            _rabbitMqConnection?.Close();
            _rabbitMqChannel?.Dispose();
            _rabbitMqConnection?.Dispose();
        }
    }
}
