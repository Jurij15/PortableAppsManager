using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Enums
{
    public enum AppItemModificationType
    {
        None, //no modifications done
        Modified, //data modifications (path, name, tags, ...)
        Deleted //app was deleted
    }
}
