using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileReader : MonoBehaviour
{
    [SerializeField] private List<File> _files;

    [Header("Debugging")]
    [SerializeField] private bool verbose = false;
    
    
    
    [HideInInspector] public List<File> files { get => _files; }

    private Dictionary<File, Data> _data = new Dictionary<File, Data>();
    [HideInInspector] public Dictionary<File, Data> data
    {
        get => _data;
        private set => _data = value;
    }
    
    public enum FileType
    {
        StarSmasher
    }

    [System.Serializable]
    public class File
    {
        public FileType type;
        public string directory;
        public string filename;
        public string path => System.IO.Path.Combine(new string[2] { directory, filename });
    }


    

    public Data GetData(File file) => data[file];

    /// <summary>
    /// Read the file at the given index in the list of files. Returns the Data component of the read file.
    /// </summary>
    public Data Read(int index)
    {
        File file = files[index];
        
        if (this.data == null) this.data = new Dictionary<File, Data>();
        
        // Don't read again if we have already read it in the past
        if (this.data.ContainsKey(file))
            if (this.data[file].gameObject != null) return this.data[file];

        GameObject go;
        if (file.type == FileType.StarSmasher) go = new GameObject(file.type.ToString(), typeof(StarSmasher));
        else throw new System.Exception("Unrecognized filed type " + file.type);
        go.transform.SetParent(transform);
        Data data = go.GetComponent<Data>();
        data.file = file;
        data.Read(verbose);

        this.data.Add(file, data);
        return data;
    }


    public static T ReadBinary<T>(System.IO.BinaryReader reader)
    {
        System.Type type = typeof(T);
        if (type == typeof(bool)) return (T)(object)reader.ReadBoolean();
        else if (type == typeof(decimal)) return (T)(object)reader.ReadDecimal();
        else if (type == typeof(double)) return (T)(object)reader.ReadDouble();
        else if (type == typeof(System.Int16)) return (T)(object)reader.ReadInt16();
        else if (type == typeof(int)) return (T)(object)reader.ReadInt32();
        else if (type == typeof(System.Int64)) return (T)(object)reader.ReadInt64();
        else if (type == typeof(float)) return (T)(object)reader.ReadSingle();
        else if (type == typeof(System.UInt16)) return (T)(object)reader.ReadUInt16();
        else if (type == typeof(uint)) return (T)(object)reader.ReadUInt32();
        else if (type == typeof(System.UInt64)) return (T)(object)reader.ReadUInt64();
        else throw new System.Exception("Unsupported type " + typeof(T));
    }
    public static T ReadBinary<T>(System.IO.BinaryReader reader, ref uint record)
    {
        System.Type type = typeof(T);
        record -= (uint)System.Runtime.InteropServices.Marshal.SizeOf<T>();
        return (T)(object)ReadBinary<T>(reader);
    }
}
