using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Model
{
    [DataContract]
    public abstract class LoadMoreItem
    {
        public bool IsLoadMore { get; set; }
    }
}
