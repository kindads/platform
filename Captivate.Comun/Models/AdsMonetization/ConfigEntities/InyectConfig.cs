using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class InyectConfig
    {
        public DefaultConfig defaultConfig { set; get; }

        public StickyConfig stickyConfig { set; get; }

        public InyectConfig()
        {
            defaultConfig = new DefaultConfig();
            stickyConfig = new StickyConfig();
        }
    }
}
