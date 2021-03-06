﻿using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Contains file IO functionality related to the program
/// </summary>
public static class FileManagement
{
    const string MSG_BEGIN = 
        "@echo off\n" +
        "echo This script was created with Project Archivist v0.1\n" +
        "echo Beginning archiving...";
    const string MSG_CURRENT_PREFIX = "echo Archiving: ";

    const string CMD = "7z a";
    const string SPACE = " ";
    const string QUOTE = "\"";

    const string PARAM_LEVEL = "-mx=";
    const string PARAM_ENCRYPT = "-mhe";
    const string PARAM_PASS = "-p";
    const string PARAM_EXCLUDE = "-x!\"";
    const string PARAM_EXCLUDE_RECURSIVE = "-xr!\"";
    const string PARAM_TYPE = "-t";

    const string EXTENSION_SEVENZIP = "7z";
    const string EXTENSION_ZIP = "zip";

    const string TYPE_LZMA = "-m0=LZMA";
    const string TYPE_LZMA2 = "-m0=LZMA2";
    const string TYPE_BZIP2 = "-m0=BZip2";
    const string TYPE_PPMD = "-m0=PPMd";

    /// <summary>
    /// Saves a set of ArchivedItems to a designated path
    /// </summary>
    /// <param name="itemsToSave">The items to save</param>
    /// <param name="path">The file path to write to</param>
    /// <returns>An exception if one was caught, otherwise null</returns>
    public static Exception SaveFile(List<ArchivedItem> itemsToSave, string path)
    {
        StreamWriter writer = null;

        try
        {
            writer = new StreamWriter(path);

            writer.WriteLine(MSG_BEGIN);

            foreach(ArchivedItem item in itemsToSave)
            {
                writer.WriteLine(MSG_CURRENT_PREFIX + item.itemName);
                string working = CMD + SPACE;

                if (item.type == ArchiveType.sevenz)
                    working += PARAM_TYPE + EXTENSION_SEVENZIP + SPACE;
                else
                    working += PARAM_TYPE + EXTENSION_ZIP + SPACE;

                switch(item.compressionMethod)
                {
                    case CompressionMethod.BZip2:
                        working += TYPE_BZIP2 + SPACE;
                        break;
                    case CompressionMethod.PPMd:
                        working += TYPE_PPMD + SPACE;
                        break;
                    case CompressionMethod.LZMA2:
                        working += TYPE_LZMA2 + SPACE;
                        break;
                    case CompressionMethod.LZMA:
                    default:
                        working += TYPE_LZMA + SPACE;
                        break;
                }

                working += PARAM_LEVEL + item.compressionLevel.ToString() + SPACE;

                if (item.password.Length > 0)
                    working += PARAM_PASS + item.password + SPACE + PARAM_ENCRYPT + SPACE;

                foreach(string s in item.exclusions)
                {
                    if (item.exclusionRecursiveDefinitions[s])
                        working += PARAM_EXCLUDE_RECURSIVE + s + QUOTE + SPACE;
                    else
                        working += PARAM_EXCLUDE + s + QUOTE + SPACE;
                }

                working += QUOTE + item.destinationPath + "\\" + item.fileName;
                if (item.type == ArchiveType.sevenz)
                    working += "." + EXTENSION_SEVENZIP + QUOTE + SPACE;
                else
                    working += "." + EXTENSION_ZIP + QUOTE + SPACE;

                working += QUOTE + item.sourcePath + "\\*" + QUOTE + "\n";

                writer.WriteLine(working);
            }

            writer.WriteLine("PAUSE");
        }

        catch (Exception e)
        {
            return e;
        }

        finally
        {
            if (writer != null)
                writer.Close();
        }

        return null;
    }
}
