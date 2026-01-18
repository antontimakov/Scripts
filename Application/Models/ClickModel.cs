// Application/Models/ClickModel.cs
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Catatonia.Application.Models;
[System.Serializable]
public class ClickModel
{
    public string old_elem_name;
    public string new_elem_name;
    public int x;
    public int y;
}
