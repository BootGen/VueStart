using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BootGen;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace VueStart.Services;
public class GenerationService
{
    private readonly IMemoryCache memoryCache;

    public GenerationService(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }
    public string Generate(JsonElement json, string title, string templateFileName, string color, bool forDownload, out string appjs, out string indexhtml)
    {
        var generator = new VueStartGenerator(json, memoryCache);
        Generate(json, title, templateFileName, color, forDownload, out appjs, out indexhtml, generator);
        return generator.Id;
    }

    private static void Generate(JsonElement json, string title, string templateFileName, string color, bool forDownload, out string appjs, out string indexhtml, VueStartGenerator generator)
    {
        var jsParameters = new Dictionary<string, object> {
                {"classes", generator.DataModel.CommonClasses}
            };
        if (forDownload)
            jsParameters.Add("input", json.ToString());
        appjs = generator.Render(templateFileName, jsParameters);
        var indexParameters = new Dictionary<string, object> {
                {"title", $"{title}"}
            };
        if (!forDownload)
            indexParameters.Add("base_url", $"/api/files/{generator.Id}/");
        indexParameters.Add("color", color);
        if (Brightness(ColorTranslator.FromHtml($"#{color}")) > 170)
        {
            indexParameters.Add("text_color", "2c3e50");
        }
        else
        {
            indexParameters.Add("text_color", "ffffff");
        }
        indexhtml = generator.Render("index.sbn", indexParameters);
    }

    private static int Brightness(Color c)
    {
        return (int)Math.Sqrt(
            c.R * c.R * .241 +
            c.G * c.G * .691 +
            c.B * c.B * .068);
    }

    public JsonElement Fix(JsonElement json)
    {
        var jObject = JsonConvert.DeserializeObject<JObject>(json.ToString(), new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-ddTHH:mm",
        });
        try
        {
            var dataModel = new DataModel
            {
                TypeToString = TypeScriptGenerator.ToTypeScriptType
            };
            dataModel.LoadRootObject("App", jObject);
        }
        catch (NamingException e)
        {
            string jsonString;
            if (e.IsArray)
                jsonString = jObject.RenamingArrays(e.ActualName, e.SuggestedName).ToString();
            else
                jsonString = jObject.RenamingObjects(e.ActualName, e.SuggestedName).ToString();
            return JsonDocument.Parse(jsonString).RootElement;
        }
        return json;
    }

    public string GenerateToCache(JsonElement json, string title, string templateFileName, string color)
    {
        var generator = new VueStartGenerator(json, memoryCache);
        Generate(json, title, templateFileName, color, false, out var appjs, out var indexhtml, generator);
        memoryCache.Set($"{generator.Id}/app.js", Minify(appjs), TimeSpan.FromMinutes(30));
        memoryCache.Set($"{generator.Id}/index.html", Minify(indexhtml), TimeSpan.FromMinutes(30));
        Generate(json, title, templateFileName, color, true, out var pAppjs, out var pIndexhtml, generator);
        memoryCache.Set($"{generator.Id}/app.js_display", pAppjs, TimeSpan.FromMinutes(30));
        memoryCache.Set($"{generator.Id}/index.html_display", pIndexhtml, TimeSpan.FromMinutes(30));
        return generator.Id;
    }

    private string Minify(string value)
    {
#if DEBUG
        return value;
#else
            value = value.Replace("\n", " ");
            value = value.Replace("\r", " ");
            value = value.Replace("\t", " ");
            int length;
            do {
                length = value.Length;
                value = value.Replace("  ", " ");
            } while(value.Length != length);

            return value;
#endif
    }
}
struct TemplateCacheKey
{
    public string Path { get; init; }
}

class VueStartGenerator
{
    public DataModel DataModel { get; }
    public string Id { get; }
    private readonly TypeScriptGenerator generator;

    private IMemoryCache memoryCache;
    public VueStartGenerator(JsonElement json, IMemoryCache memoryCache)
    {
        ClassModel.IdName = "$ID$";
        this.memoryCache = memoryCache;
        DataModel = new DataModel
        {
            TypeToString = TypeScriptGenerator.ToTypeScriptType
        };
        var jObject = JsonConvert.DeserializeObject<JObject>(json.ToString(), new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-ddTHH:mm",
        });
        DataModel.LoadRootObject("App", jObject);
        var collection = new ResourceCollection(DataModel);
        var seedStore = new SeedDataStore(collection);
        seedStore.Load(jObject);

        Id = Guid.NewGuid().ToString();
        generator = new TypeScriptGenerator(null);
        generator.Templates = Load("templates");
        this.memoryCache = memoryCache;
    }

    private VirtualDisk Load(string path)
    {
        return memoryCache.GetOrCreate(new TemplateCacheKey { Path = path }, entry =>
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
        });
    }

    internal string Render(string templateFileName, Dictionary<string, object> parameters)
    {
        return generator.Render(templateFileName, parameters);
    }
}