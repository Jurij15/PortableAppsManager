using ABI.Windows.AI.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Services
{
    public class TagsService
    {
        public HashSet<string> GetAllTags()
        {
            HashSet<string> tags = new HashSet<string>();

            foreach (var item in Globals.Settings.Apps)
            {
                foreach (var tag in item.Tags)
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }
    }
}
