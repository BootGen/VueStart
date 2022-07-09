using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VueStart.Data;

namespace VueStart.Services;

public class InputStatisticService
{
    private readonly ILogger<InputStatisticService> logger;

    private List<InputData> InputData { get; } = new List<InputData>();

    public InputStatisticService(ILogger<InputStatisticService> logger)
    {
        this.logger = logger;
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


    public void StoreStatisticRecord(GenerateRequest request, ActionType actionType, bool error)
    {
        int hash = StringHash(request.Data.ToString());
        var inputData = InputData.FirstOrDefault(r => r.Hash == hash);
        var record = new StatisticRecord {
            Download = actionType == ActionType.Download,
            Readonly = request.Settings.IsReadonly
        };
        if (inputData == null)
        {
            inputData = new InputData {
                Hash = hash,
                Data = request.Data,
                FirstUse = DateTime.UtcNow,
                LastUse = DateTime.UtcNow,
                Error = error,
                StatisticRecords = new List<StatisticRecord>() { record }
            };
            InputData.Add(inputData);
        } else {
            var existingRecord = inputData.StatisticRecords.FirstOrDefault(r => r.IsSameKind(record));
            if (existingRecord != null)
            {
                record = existingRecord;
            } else {
                inputData.StatisticRecords.Add(record);
            }
        }
        UpdateRecord(record, request.Settings.Frontend.ToFrontendType());
    }

    private void UpdateRecord(StatisticRecord record,  Frontend cssType)
    {
        switch (cssType)
        {
            case Frontend.Bootstrap:
                record.BootstrapCount += 1;
            break;
            case Frontend.Tailwind:
                record.TailwindCount += 1;
            break;
            case Frontend.Vanilla:
                record.VanillaCount += 1;
            break;
        }
    }

    public void SaveRecords(ApplicationDbContext dbContext)
    {
        logger.Log(LogLevel.Information, $"Saving {InputData.Count} input records.");
        foreach (var inputData in InputData)
        {
            var existingInputData = dbContext.InputData.FirstOrDefault(r => r.Hash == inputData.Hash);
            if (existingInputData != null)
            {
                dbContext.Entry(existingInputData).Collection(i => i.StatisticRecords).Load();
                existingInputData.LastUse = DateTime.UtcNow;
                foreach(var record in inputData.StatisticRecords) {
                    var existingRecord = existingInputData.StatisticRecords.FirstOrDefault(r => r.IsSameKind(record));
                    if (existingRecord != null) {
                        existingRecord.BootstrapCount += record.BootstrapCount;
                        existingRecord.TailwindCount += record.TailwindCount;
                        existingRecord.VanillaCount += record.VanillaCount;
                    } else {
                        existingInputData.StatisticRecords.Add(record);
                    }
                }
            }
            else
            {
                dbContext.InputData.Add(inputData);
            }
        }
        InputData.Clear();
    }
}
