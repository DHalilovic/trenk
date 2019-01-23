﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class MultiplayersBuildAndRun
{

    [MenuItem("File/Run Multiplayer/Windows/1 Player")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }

    [MenuItem("File/Run Multiplayer/Windows/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }

    [MenuItem("File/Run Multiplayer/Windows/3 Players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }

    [MenuItem("File/Run Multiplayer/Windows/4 Players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        for (int i = 1; i <= playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Win64/" + GetProjectName() + i.ToString() + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }

    [MenuItem("File/Run Multiplayer/Mac OSX/1 Player")]
    static void PerformOSXBuild1()
    {
        PerformOSXBuild(1);
    }

    [MenuItem("File/Run Multiplayer/Mac OSX/2 Players")]
    static void PerformOSXBuild2()
    {
        PerformOSXBuild(2);
    }

    [MenuItem("File/Run Multiplayer/Mac OSX/3 Players")]
    static void PerformOSXBuild3()
    {
        PerformOSXBuild(3);
    }

    [MenuItem("File/Run Multiplayer/Mac OSX/4 Players")]
    static void PerformOSXBuild4()
    {
        PerformOSXBuild(4);
    }

    static void PerformOSXBuild(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        for (int i = 1; i <= playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/OSX/" + GetProjectName() + i.ToString() + ".app", BuildTarget.StandaloneOSX, BuildOptions.AutoRunPlayer);
        }

    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
            Debug.Log(EditorBuildSettings.scenes[i].path);
        }

        return scenes;
    }

}