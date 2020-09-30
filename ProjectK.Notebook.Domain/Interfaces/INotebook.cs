using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectK.Notebook.Domain.Interfaces
{
    public interface INotebook : INode
    {
        public string Description { get; set; }
    }
}
