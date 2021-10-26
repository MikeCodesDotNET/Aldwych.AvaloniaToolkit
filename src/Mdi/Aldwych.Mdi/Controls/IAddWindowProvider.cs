using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldwych.Mdi.Controls
{
    public interface IAddWindowProvider
    {
        Workspace TargetLayout { get; set; }
    }
}
