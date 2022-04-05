using ItchBundleHelper.Models;
using Notion.Client;
using System.Text.Json;

namespace ItchBundleHelper
{
    public class Helper
    {
        private readonly Settings _settings;
        private readonly SelectPropertyValue _statusValue;
        private readonly StreamWriter _logStream;
        private int _delayPerCreate = 1000;        

        public Helper(Settings settings)
        {
            _settings = settings;
            _logStream = new StreamWriter(new FileStream("log.txt", FileMode.Append));

            if (string.IsNullOrEmpty(_settings.NotionApiKey))
            {
                throw new ArgumentNullException($"{nameof(_settings.NotionApiKey)} is required.");
            }

            if (string.IsNullOrEmpty(_settings.ParentPageId))
            {
                throw new ArgumentNullException($"{nameof(_settings.ParentPageId)} is required.");
            }

            _statusValue = new SelectPropertyValue()
            {
                Select = new SelectOption { Name = "Review" },
            };
        }


        public async Task Run()
        {
            try
            {
                var bundleDataTask = GetBundleData();

                var client = NotionClientFactory.Create(new ClientOptions
                {
                    AuthToken = _settings.NotionApiKey
                });

                string dbName = $"Itch Bundle - {DateTime.Now.ToShortDateString()}";
                var db = await client.Databases.CreateAsync(GetDbParams(dbName));
                var bundleData = await bundleDataTask;

                foreach (var game in bundleData.games)
                {
                    try
                    {
                        Log($"Adding {game.id} - {game.title}...");
                        var pageParams = BuildPage(db.Id, game);
                        var page = await client.Pages.CreateAsync(pageParams);
                        LogLine($" done.", omitPrefix: true);
                        await Task.Delay(_delayPerCreate);
                    }
                    catch (NotionApiException ex)
                    {
                        if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                        {
                            LogLine($"{game.id} - {game.title}: Too Many Requests. Sleeping...");
                            await Task.Delay(30000);

                            var newDelay = _delayPerCreate + 1000;
                            LogLine($"Setting delay to {newDelay}.");
                            _delayPerCreate = newDelay;
                        }
                        else
                        {
                            LogLine($"{ex.StatusCode}: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch(NotionApiException ex)
            {
                LogLine($"{ex.StatusCode}: {ex.Message}");
                throw;
            }
            finally
            {
                _logStream.Flush();
                _logStream.Close();
            }            
        }

        private DatabasesCreateParameters GetDbParams(string dbName)
        {
            var statusSchema = new OptionWrapper<SelectOptionSchema>()
            {
                Options = new List<SelectOptionSchema>
                {
                    new SelectOptionSchema() { Name = "Review", Color = "default" },
                    new SelectOptionSchema() { Name = "Try", Color = "orange" },
                    new SelectOptionSchema() { Name = "Ignore", Color = "red" },
                    new SelectOptionSchema() { Name = "Keep", Color = "green" },
                }
            };

            // Create schema for DB (ie. what properties can be used in pages)
            var dbProperties = new Dictionary<string, IPropertySchema>
            {
                { "Name", new TitlePropertySchema() { Title = new Dictionary<string, object>() } },
                { "gameid", new NumberPropertySchema() { Number = new Number { Format = "number" } } },
                { "url", new URLPropertyScheam() { Url = new Dictionary<string, object>() } },
                { "cover", new URLPropertyScheam() { Url = new Dictionary<string, object>() } },
                { "classification", new RichTextPropertySchema() { RichText = new Dictionary<string, object>() } },
                { "short_text", new RichTextPropertySchema() {RichText = new Dictionary<string, object>() } },
                { "price", new RichTextPropertySchema() { RichText = new Dictionary<string, object>() } },
                { "Status", new SelectPropertySchema() { Select = statusSchema } }
            };

            // Set properties of the DB itself
            var dbParams = new DatabasesCreateParameters
            {
                Parent = new ParentPageInput { PageId = _settings.ParentPageId },
                Title = dbName.ToInputList(),
                Properties = dbProperties,
            };

            return dbParams;
        }

        private PagesCreateParameters BuildPage(string parentDatabaseId, Game game)
        {         
            var pagesCreateParameters = PagesCreateParametersBuilder
                .Create(new DatabaseParentInput { DatabaseId = parentDatabaseId})
                .AddProperty("Name", game.title.ToTitlePropertyValue())
                .AddProperty("gameid", ((double)game.id).ToNumberPropertyValue())
                .AddProperty("url", game.url.ToUrlPropertyValue())
                .AddProperty("cover", game.cover.ToUrlPropertyValue())
                .AddProperty("classification", game.classification.ToRichTextPropertyValue())
                .AddProperty("short_text", game.short_text.ToRichTextPropertyValue())
                .AddProperty("price", game.price.ToRichTextPropertyValue())
                .AddProperty("Status", _statusValue)
                .Build();

            return pagesCreateParameters;
        }

        private async Task<Bundle> GetBundleData()
        {
            if (_settings.BundleFile == null)
            {
                throw new ArgumentNullException($"{nameof(_settings.BundleFile)} is required.");
            }

            var serializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
            };

            var bundle = await JsonSerializer.DeserializeAsync<Bundle>(new FileStream(_settings.BundleFile, FileMode.Open), serializerOptions);

            if(bundle == null)
            {
                throw new NullReferenceException("Couldn't parse JSON");
            }

            return bundle;
        }

        private void Log(string text, bool omitPrefix = false)
        {
            string prefix = omitPrefix ? "" : $"[{DateTime.Now}] - ";

            Console.Write(text);
            _logStream.Write(prefix + text);
        }

        private void LogLine(string text, bool omitPrefix = false)
        {
            string prefix = omitPrefix ? "" : $"[{DateTime.Now}] - ";

            Console.WriteLine(text);
            _logStream.WriteLine(prefix + text);
        }
    }
}
