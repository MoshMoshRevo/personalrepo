using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LogFile : MonoBehaviour
{
    StreamWriter m_FStream = null;
    public string m_FilePath;
    public bool m_FileStreamExists;

    void Start()
    {
        if(File.Exists(m_FilePath))
        {
            File.Delete(m_FilePath);
        }

        m_FStream = new StreamWriter(m_FilePath, true);
        Application.logMessageReceived += HandleLog;
    }

    void Update()
    {
        if (m_FStream != null)
        {
            Debug.Log(Time.realtimeSinceStartup);
            m_FileStreamExists = true;
        }
        else m_FileStreamExists = false;

    }

    void OnApplicationQuit()
    {
        if (m_FStream != null)
        {
            Application.logMessageReceived -= HandleLog;
            m_FStream.Close();
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (m_FStream != null)
        {
            m_FStream.WriteLine(logString);
        }
    }
}
