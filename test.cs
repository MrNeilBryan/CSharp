using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

class Program
{
    static void Main(string[] args)
    {
        // The URL of your Key Vault
        string keyVaultUrl = "https://<your-keyvault-name>.vault.azure.net/";

        // Authenticate using the managed identity of the app
        var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

        // Retrieve the secret by name (latest version)
        KeyVaultSecret secret = client.GetSecret("DbPassword");

        Console.WriteLine($"Secret Name: {secret.Name}");
        Console.WriteLine($"Secret Value: {secret.Value}");
    }
}
