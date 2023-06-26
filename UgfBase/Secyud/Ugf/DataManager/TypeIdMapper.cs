using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Secyud.Ugf.DataManager;

public static class TypeIdMapper
{
    private static readonly MD5 MD5 = MD5.Create();
    private static readonly ConcurrentDictionary<string, Guid> IDDictionary = new();
    private static readonly ConcurrentDictionary<Guid, Type> TypeDictionary = new();


    public static void SetId(
        string type, Guid id)
    {
        if (id == default)
            id = GenerateId(type);

        IDDictionary[type] = id;
    }

    public static Guid GetId(Type type)
    {
        string fullName = type.FullName!;

        if (!IDDictionary.TryGetValue(fullName, out Guid id))
        {
            id = GenerateId(fullName);
            IDDictionary[fullName] = id;
            return id;
        }

        return id;
    }

    public static Type GetType(Guid id)
    {
        return TypeDictionary[id];
    }

    public static void SetType(
        [NotNull] Type type, Guid id)
    {
        string fullName = type.FullName!;

        if (id == default)
            id = GenerateId(fullName);

        if (TypeDictionary.TryGetValue(id, out Type typeExist)
            && typeExist != type)
            Debug.LogError($"Type conflict for id {id}: " +
                           $" Type exist - {typeExist} && " +
                           $" Added Type - {type}");
        TypeDictionary[id] = type;
        IDDictionary[fullName] = id;
    }

    private static Guid GenerateId(string typeFullName)
    {
        return new Guid(MD5.ComputeHash(Encoding.UTF8.GetBytes(typeFullName)));
    }

    public static void WriteLoggedGuid(string path)
    {
        using FileStream stream = File.OpenWrite(path);
        using StreamWriter writer = new(stream);
        foreach ((string key, Guid value) in IDDictionary)
            writer.WriteLine($"{value} {key}");
    }

    public static IDictionary<Guid,Type> Types()
    {
        return TypeDictionary;
    }
    public static List<Tuple<Guid,string>> SubTypes(Type type)
    {
        return TypeDictionary.Where(u=> type.IsAssignableFrom( u.Value))
            .Select(u=>new Tuple<Guid,string>(u.Key,u.Value.Name))
            .ToList();
    }
    
}