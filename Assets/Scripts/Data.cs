using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    private FileReader _fileReader;
    [HideInInspector] public FileReader fileReader
    {
        get
        {
            if (_fileReader == null) _fileReader = GetComponentInParent<FileReader>();
            return _fileReader;
        }
    }

    [HideInInspector] public FileReader.File file;

    public int nParticles = 0;

    void OnDestroy()
    {
        if (fileReader != null)
            if (fileReader.data != null)
                if (fileReader.data.ContainsKey(file))
                    fileReader.data.Remove(file);
    }
    
    public virtual void Read(bool verbose = false) { }

    public virtual Vector3[] GetPositions() { return null; }
    public virtual float[] GetSizes() { return null; }
}
