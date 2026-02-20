#:property PublishAot=false

// DESAFIO: Gerenciador de Configurações da Aplicação
// PROBLEMA: Uma aplicação precisa carregar configurações de banco de dados, APIs e cache
// uma única vez e compartilhar entre todos os componentes. O código atual permite múltiplas
// instâncias, causando inconsistências e desperdício de recursos

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    public class ConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> LazyInstance =
            new(() => new ConfigurationManager());

        private readonly Dictionary<string, string> _settings;
        private bool _isLoaded;

        private ConfigurationManager()
        {
            _settings = new Dictionary<string, string>();
            _isLoaded = false;
            Console.WriteLine("✅ Instância única de ConfigurationManager criada.");
        }

        public static ConfigurationManager Instance => LazyInstance.Value;

        public void LoadConfigurations()
        {
            if (_isLoaded)
            {
                Console.WriteLine("Configurações já carregadas.");
                return;
            }

            Console.WriteLine("🔄 Carregando configurações...");
            
            // Simulando operação custosa de carregamento
            System.Threading.Thread.Sleep(200);

            // Carregando configurações de diferentes fontes
            _settings["DatabaseConnection"] = "Server=localhost;Database=MyApp;";
            _settings["ApiKey"] = "abc123xyz789";
            _settings["CacheServer"] = "redis://localhost:6379";
            _settings["MaxRetries"] = "3";
            _settings["TimeoutSeconds"] = "30";
            _settings["EnableLogging"] = "true";
            _settings["LogLevel"] = "Information";

            _isLoaded = true;
            Console.WriteLine("✅ Configurações carregadas com sucesso!\n");
        }

        public string GetSetting(string key)
        {
            if (!_isLoaded)
                LoadConfigurations();

            if (_settings.ContainsKey(key))
                return _settings[key];

            return string.Empty;
        }

        public void UpdateSetting(string key, string value)
        {
            _settings[key] = value;
            Console.WriteLine($"Configuração atualizada: {key} = {value}");
        }
    }

    // Serviços da aplicação que precisam das configurações
    public class DatabaseService
    {
        private readonly ConfigurationManager _config;

        public DatabaseService()
        {
            _config = ConfigurationManager.Instance;
        }

        public void Connect()
        {
            var connectionString = _config.GetSetting("DatabaseConnection");
            Console.WriteLine($"[DatabaseService] Conectando ao banco: {connectionString}");
        }
    }

    public class ApiService
    {
        private readonly ConfigurationManager _config;

        public ApiService()
        {
            _config = ConfigurationManager.Instance;
        }

        public void MakeRequest()
        {
            var apiKey = _config.GetSetting("ApiKey");
            Console.WriteLine($"[ApiService] Fazendo requisição com API Key: {apiKey}");
        }
    }

    public class CacheService
    {
        private readonly ConfigurationManager _config;

        public CacheService()
        {
            _config = ConfigurationManager.Instance;
        }

        public void Connect()
        {
            var cacheServer = _config.GetSetting("CacheServer");
            Console.WriteLine($"[CacheService] Conectando ao cache: {cacheServer}");
        }
    }

    public class LoggingService
    {
        private readonly ConfigurationManager _config;

        public LoggingService()
        {
            _config = ConfigurationManager.Instance;
        }

        public void Log(string message)
        {
            var logLevel = _config.GetSetting("LogLevel");
            Console.WriteLine($"[LoggingService] [{logLevel}] {message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Configurações ===\n");

            Console.WriteLine("Inicializando serviços...\n");
            
            var dbService = new DatabaseService();
            var apiService = new ApiService();
            var cacheService = new CacheService();
            var logService = new LoggingService();

            Console.WriteLine("\nUsando os serviços...\n");
            
            dbService.Connect();
            apiService.MakeRequest();
            cacheService.Connect();
            logService.Log("Sistema iniciado");

            Console.WriteLine("\n--- Atualização compartilhada (Singleton) ---\n");

            var sharedConfig = ConfigurationManager.Instance;
            sharedConfig.UpdateSetting("LogLevel", "Debug");

            Console.WriteLine($"LogLevel global: {ConfigurationManager.Instance.GetSetting("LogLevel")}");
            logService.Log("Log após atualização global");

            Console.WriteLine("\n--- Resultado ---");
            Console.WriteLine("A configuração foi carregada uma única vez");
            Console.WriteLine("Todos os serviços compartilham o mesmo estado");
        }
    }
}
