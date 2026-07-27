using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartFactorySimple;

public class Login
{
    private readonly string CREDENTIALS_FILE;
    private readonly Dictionary<string, EmployeeCredential> credentials = [];

    public class EmployeeCredential
    {
        public string EmployeeId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public Login()
    {
        // Set credentials file path in the project directory
        CREDENTIALS_FILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employees.txt");
        LoadCredentials();
    }

   
    private void LoadCredentials()
    {
        try
        {
            if (!File.Exists(CREDENTIALS_FILE))
            {
                Console.WriteLine($"Error: {CREDENTIALS_FILE} not found. Please ensure the file exists.");
                return;
            }

            credentials.Clear();
            string[] lines = File.ReadAllLines(CREDENTIALS_FILE);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(';');
                if (parts.Length != 4)
                {
                    Console.WriteLine($"Warning: Invalid line format: {line}");
                    continue;
                }

                var credential = new EmployeeCredential
                {
                    EmployeeId = parts[0].Trim(),
                    Username = parts[1].Trim(),
                    Password = parts[2].Trim(),
                    Role = parts[3].Trim()
                };

                credentials[credential.Username] = credential;
            }

            Console.WriteLine($"Loaded {credentials.Count} employee credentials.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading credentials: {ex.Message}");
        }
    }

    
    public EmployeeCredential Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        if (credentials.ContainsKey(username))
        {
            EmployeeCredential credential = credentials[username];
            if (credential.Password == password)
            {
                return credential;
            }
        }

        return null;
    }

    
    public EmployeeCredential PromptLogin()
    {
        Console.WriteLine("\n========== SMART FACTORY LOGIN ==========");
        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = ReadPassword();

        EmployeeCredential credential = Authenticate(username, password);

        if (credential != null)
        {
            Console.WriteLine($"\nWelcome {username}! Role: {credential.Role}\n");
            Logging.Log(credential.Username, "Successful login");
            return credential;
        }
        else
        {
            Console.WriteLine("\nInvalid username or password!\n");
            var userForLog = string.IsNullOrWhiteSpace(username) ? "unknown" : username;
            Logging.Log(userForLog, "Failed login attempt");
            return null;
        }
    }

    
    public EmployeeCredential LoginWithAttempts(int maxAttempts = 3)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            EmployeeCredential credential = PromptLogin();
            if (credential != null)
            {
                return credential;
            }

            if (attempt < maxAttempts)
            {
                Console.WriteLine($"Attempt {attempt} failed. {maxAttempts - attempt} attempts remaining.\n");
            }
        }

        Console.WriteLine("Login failed after maximum attempts. Exiting...");
        return null;
    }

   
    public bool SaveEmployeeCredential(string employeeId, string username, string password, string role)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(employeeId) || string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                Console.WriteLine("Error: All credential fields must be provided!");
                return false;
            }

            if (credentials.ContainsKey(username))
            {
                Console.WriteLine($"Error: Username '{username}' already exists!");
                return false;
            }

            string credentialLine = $"{employeeId};{username};{password};{role}";
            
            // Append to file
            File.AppendAllText(CREDENTIALS_FILE, credentialLine + Environment.NewLine);
            
            // Update in-memory dictionary
            var credential = new EmployeeCredential
            {
                EmployeeId = employeeId,
                Username = username,
                Password = password,
                Role = role
            };
            credentials[username] = credential;

            Console.WriteLine($"Credentials saved for employee {username} ({role})");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving credentials: {ex.Message}");
            return false;
        }
    }

    // Read password from console while masking input with '*'
    public string ReadPassword()
    {
        var pwd = new System.Text.StringBuilder();
        ConsoleKeyInfo key;
        while (true)
        {
            key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (pwd.Length > 0)
                {
                    pwd.Length--;
                    // move cursor back, write space, move back again
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                pwd.Append(key.KeyChar);
                Console.Write("*");
            }
        }

        return pwd.ToString();
    }
}
