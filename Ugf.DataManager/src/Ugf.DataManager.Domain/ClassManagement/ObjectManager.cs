using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DataManager;
using Volo.Abp.Domain.Services;

namespace Ugf.DataManager.ClassManagement;

public class ObjectManager : DomainService
{
    private readonly ISpecificObjectRepository _objectRepository;

    public ObjectManager(
        ISpecificObjectRepository objectRepository)
    {
        _objectRepository = objectRepository;
    }

    public async Task GenerateConfigAsync(Guid classId, int? bundleId)
    {
        Logger.LogInformation("ClassManager: Begin search object");

        List<SpecificObject> results =
            (await _objectRepository.FilteredQueryableAsync(
                await _objectRepository.GetQueryableAsync(),
                null, bundleId, classId))
            .ToList();

        string path = Path.Combine(AppContext.BaseDirectory, "OutConfigs");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        Type type = TypeIdMapper.GetType(classId);
        
        path = Path.Combine(path, $"{type.Name}-{bundleId}.binary");

        Logger.LogInformation("ClassManager: Start write config to path: {Path}", path);
        await using FileStream stream = File.OpenWrite(path);
        await using DefaultArchiveWriter writer = new(stream);

        writer.Write(results.Count);

        foreach (SpecificObject result in results)
        {
            writer.Write(result.ClassId);
            writer.Write(result.Name);
            writer.Write(result.ArchivedData.Length);
            writer.Write(result.ArchivedData);
            writer.Write(result.InitialedData.Length);
            writer.Write(result.InitialedData);
            writer.Write(result.IgnoredData.Length);
            writer.Write(result.IgnoredData);
        }

        Logger.LogInformation("Config successfully write {Count} objects", results.Count);
    }
}