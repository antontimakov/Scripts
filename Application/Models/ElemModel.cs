// Application/Models/ElemModel.cs
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Catatonia.Application.Models
{
    [System.Serializable]
    public class ElemModel
    {
        public string elem_name;
        public int x;
        public int y;
        public bool elem_plantable;
        public bool elem_harvestable;
        public bool elem_weed;
        public int elem_lifetime;
        public string updated;
        public DateTime? updated_modefied;
    }
}
