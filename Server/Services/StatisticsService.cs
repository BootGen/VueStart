using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace VueStart.Services
{
    public enum ActionType {
        Generate,
        Download
    }

    public enum ArtifactType {
        View,
        Form,
        Editor
    }

    public class StatisticsService
    {
        private readonly IConfiguration configuration;

        public StatisticsService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private static int StringHash(string text)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in text)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        private void UpdateRecord(StatisticRecord record, ActionType actionType, ArtifactType artifactType)
        {
            record.LastUse = DateTime.Now;
            switch (actionType)
            {
                case ActionType.Download:
                switch (artifactType)
                {
                    case ArtifactType.View:
                    record.ViewDownloadedCount += 1;
                    break;
                    case ArtifactType.Form:
                    record.FormDownloadedCount += 1;
                    break;
                    case ArtifactType.Editor:
                    record.EditorDownloadedCount += 1;
                    break;
                }
                break;
                case ActionType.Generate:
                switch (artifactType)
                {
                    case ArtifactType.View:
                    record.ViewGeneratedCount += 1;
                    break;
                    case ArtifactType.Form:
                    record.FormGeneratedCount += 1;
                    break;
                    case ArtifactType.Editor:
                    record.EditorGeneratedCount += 1;
                    break;
                }
                break;
            }
        }

        public  async void  onEvent(string data, ActionType actionType, ArtifactType artifactType)
        {
            await Task.Run(() =>{
                int hash = StringHash(data);
                using (var dbContext = new ApplicationDbContext(configuration))
                {
                    var record = dbContext.StatisticRecords.Where(r => r.Hash == hash && r.Data == data).FirstOrDefault();
                    if (record == null)
                    {
                        record = new StatisticRecord
                        {
                            Data = data,
                            Hash = hash,
                            FirstUse = DateTime.Now
                        };
                        UpdateRecord(record, actionType, artifactType);
                        dbContext.StatisticRecords.Add(record);
                    } else {
                        UpdateRecord(record, actionType, artifactType);
                    }
                    dbContext.SaveChanges();
                }        
            });
        }
    }
}