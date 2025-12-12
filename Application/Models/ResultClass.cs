// ResultClass.cs
using System;

namespace Catatonia.Application.Models
{
    [System.Serializable]
    public class ResultClass
    {
        public string time;
        public string status;
        public ElemModel[] received;
    }
}
