using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class StripDDSMipMaps
{
    const int INT_32_SIZE = sizeof(int);

    public static void StripAll(in string absTextureDir)
    {
        string[] files = Directory.GetFiles(absTextureDir);
        foreach (string file in files)
        {
            if (!file.EndsWith(".dds")) { continue; }
            Strip(file);
        }
    }

    static void Strip(in string absFilePath)
    {
        byte[] ddsData = File.ReadAllBytes(absFilePath);

        // header
        int i = INT_32_SIZE; // magic

        int dwSize = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwFlags = BitConverter.ToInt32(ddsData, i);
        int dwFlagsOffset = i;
        i += INT_32_SIZE;

        int dwHeight = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwWidth = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwPitchOrLinearSize = BitConverter.ToInt32(ddsData, i);
        bool has_DDSD_PITCH = (dwPitchOrLinearSize & 0x8) == 1;
        bool has_DDSD_LINEARSIZE = (dwPitchOrLinearSize & 0x80000) == 1;
        i += INT_32_SIZE;

        int dwDepth = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwMipMapCount = BitConverter.ToInt32(ddsData, i);
        if(dwMipMapCount <= 1) 
        { 
            return; 
        }
        int mipOffset = i;
        i += INT_32_SIZE;

        i += INT_32_SIZE * 11; // dwReserved1[11]

        //ddspf
        int dwSizePF = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwFlagsPF = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwFourCCPF = BitConverter.ToInt32(ddsData, i);
        //string fourCCLabel = "";
        int blockSize = 0;
        switch (dwFourCCPF)
        {
            case 0x31545844:
                //fourCCLabel = "DXT1";
                blockSize = 8;
                break;
            case 0x32545844:
                //fourCCLabel = "DXT2";
                blockSize = 8;
                break;
            case 0x33545844:
                //fourCCLabel = "DXT3";
                blockSize = 16;
                break;
            case 0x34545844:
                //fourCCLabel = "DXT4";
                blockSize = 16;
                break;
            case 0x35545844:
                //fourCCLabel = "DXT5";
                blockSize = 16;
                break;
            case 0x30315844:
                //fourCCLabel = "DXT10";
                blockSize = 16;
                break;
            case 0x31495441:
                //fourCCLabel = "ATI1";
                blockSize = 16;
                break;
            case 0x32495441:
                //fourCCLabel = "ATI2";
                blockSize = 16;
                break;
            default:
                //fourCCLabel = "UNKNOWN";
                Debug.LogError($"Failed to strip {absFilePath}: unknown compression");
                return;
        }
        //int test1 = Math.Max(1, ((dwWidth + 3) / 4)) * Math.Max(1, ((dwHeight + 3) / 4)) * 8;
        //int test2 = Math.Max(1, ((dwWidth + 3) / 4)) * Math.Max(1, ((dwHeight + 3) / 4)) * 16;
        int mipSize = Math.Max(1, ((dwWidth + 3) / 4)) * Math.Max(1, ((dwHeight + 3) / 4)) * blockSize;
        i += INT_32_SIZE;

        int dwRGBBitCountPF = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwRBitMaskPF = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwGBitMaskPF = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwBBitMaskPF = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwABitMaskPF = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;
        //ddspf end

        int dwCaps1 = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        int dwCaps2 = BitConverter.ToInt32(ddsData, i);
        i += INT_32_SIZE;

        i += INT_32_SIZE * 3; // dwReserved2[3]

        if (dwFourCCPF == 0x30315844)
        {
            i += INT_32_SIZE * 20; // DDS_HEADER_DXT10
        }
        // header end

        i += mipSize;

        int newMipCount = 1;
        byte[] newMipBytes = BitConverter.GetBytes(newMipCount);
        Buffer.BlockCopy(newMipBytes, 0, ddsData, mipOffset, sizeof(int));

        int newdwFlags = dwFlags | 0x20000;
        byte[] newdwFlagsBytes = BitConverter.GetBytes(newdwFlags);
        Buffer.BlockCopy(newdwFlagsBytes, 0, ddsData, dwFlagsOffset, sizeof(int));

        byte[] newDDSData = ddsData.Take(i).ToArray();
        File.WriteAllBytes(absFilePath, newDDSData);
    }
}
