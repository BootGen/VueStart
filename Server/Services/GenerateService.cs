using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BootGen;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace VueStart.Services
{
    public class GenerateService
    {
        private readonly IMemoryCache memoryCache;

        public GenerateService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        public string Generate(JsonElement json, string title, string templateFileName, out string appjs, out string indexhtml) {
            var dataModel = new DataModel();
            var jObject = JObject.Parse(json.ToString(), new JsonLoadSettings { CommentHandling = CommentHandling.Ignore, DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            dataModel.LoadRootObject("App", jObject);
            var collection = new ResourceCollection(dataModel);
            var seedStore = new SeedDataStore(collection);
            seedStore.Load(jObject);

            var id = Guid.NewGuid().ToString();
            var generator = new TypeScriptGenerator(null);
            generator.Templates = Load("templates");
            appjs = generator.Render(templateFileName, new Dictionary<string, object> {
                {"classes", dataModel.CommonClasses}
            });
            indexhtml = generator.Render("index.sbn", new Dictionary<string, object> {
                {"base_url", $"/files/{id}/"},
                {"title", $"{title}"}
            });
            return id;
        }

        public string Generate(JsonElement json, string title, string templateFileName) {
            string id = Generate(json, title, templateFileName, out string appjs, out string indexhtml);
            memoryCache.Set($"{id}/app.js", Minify(appjs), TimeSpan.FromMinutes(1));
            memoryCache.Set($"{id}/index.html", Minify(indexhtml), TimeSpan.FromMinutes(1));
            return id;
        }

        private string Minify(string value) {
            value = value.Replace("\n", " ");
            value = value.Replace("\r", " ");
            value = value.Replace("\t", " ");
            int length;
            do {
                length = value.Length;
                value = value.Replace("  ", " ");
            } while(value.Length != length);

            return value;
        }

        private static VirtualDisk Load(string path)
        {
            var templates = new VirtualDisk();
            foreach (var file in Directory.EnumerateFiles(path))
            {
                templates.Files.Add(new VirtualFile
                {
                    Name = Path.GetFileName(file),
                    Path = "",
                    Content = System.IO.File.ReadAllText(file)
                });
            }

            return templates;
        }
    }
}