using Mapsui.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSampleCommon
{
    public interface ISample
    {
        string Name { get; }
        string Category { get; }
        void Setup(IMapControl mapControl);
    }
}
