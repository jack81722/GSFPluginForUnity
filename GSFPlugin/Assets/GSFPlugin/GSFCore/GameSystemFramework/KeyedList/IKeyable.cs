using System;
using System.Collections.Generic;
using System.Text;

namespace ExCollection
{
    public interface IKeyable<T> where T : IComparable<T>
    {
        T Key { get; set; }
    }
}
