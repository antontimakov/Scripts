// Application/Models/ClickModel.cs
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Catatonia.Application.Models;
[System.Serializable]
public class ClickModel
{
    public string OldElemName;
    public string NewElemName;
    public int X;
    public int Y;
}
