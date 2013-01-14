using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySynch.Core.DataTypes
{
    public class ChangePushPackage
    {
        public string Source { get; set; }

        public string SourceRootName { get; set; }

        public List<ChangePushItem> ChangePushItems { get; set; }
    }
}
