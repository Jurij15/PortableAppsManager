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
        public List<string> GetAllTags()
        {
            List<string> tags = new List<string>();

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
