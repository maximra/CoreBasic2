using File_Organizer__Command_Line_Tool___5;
using WebApplication1;
using WebApplication1.Services;

string encryptedFileName = "encrypted.txt";
string decryptedFileName = "decrypted.txt";
string inputFileName;
string loggingDirectory = "logs";
string encryptionAndDecryptionDirectory = "EncryptionAndDecryption";

try
{
    if (!Directory.Exists(loggingDirectory))
        Directory.CreateDirectory(loggingDirectory);
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
    return;
}

try
{
    if (!Directory.Exists(encryptionAndDecryptionDirectory))
        Directory.CreateDirectory(encryptionAndDecryptionDirectory);
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
    return;
}
AppLogger.Instance.Information("Logging information started.");
inputFileName = AppLogger.GetCurrentLogFileName();       // get the name for the encryption
// Create the builder
var builder = WebApplication.CreateBuilder(args);

// New registered services from the Job Queue
builder.Services.AddSingleton<IJobQueue, JobQueue>();
builder.Services.AddHostedService<MyBackgroundWorker>();

// Register services
builder.Services.AddHostedService<MyBackgroundWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Hook into the application lifetime
var lifetime = app.Lifetime;

lifetime.ApplicationStarted.Register(() =>
{
    AppLogger.Instance.Information("Application has started.");
});

lifetime.ApplicationStopping.Register(() =>
{
    AppLogger.Instance.Information("Application is stopping...");
});

lifetime.ApplicationStopped.Register(() =>
{
    AppLogger.Instance.Information("Application has stopped.");
    AppLogger.Instance.CloseAndFlush(); // flush and close logs cleanly
    FileEncryptionService myFileEncryptionService = new FileEncryptionService(inputFileName, encryptedFileName, decryptedFileName, encryptionAndDecryptionDirectory+"/");
    myFileEncryptionService.PerformAesEncryption();
    myFileEncryptionService.PerformAesDecryption();
});

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();