using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace DTMUtil
{
    public class AnalogTestInfoInterChange
    {
        public AnalogTestInfoInterChange(object[] objs)
        {
            string filePath = (string)objs[0];
            string[] devicesToSkip = null;
            if (objs.Length > 1)
            {
                devicesToSkip = new string[objs.Length - 1];
                for (int i = 0; i < devicesToSkip.Length; i++)
                {
                    devicesToSkip[i] = (string)objs[i + 1];
                }
            }
            reader = new PCDReader(filePath);
            var points = reader.GetAnalogPointTree(devicesToSkip);
            foreach (var point in points)
            {
                analogTestInfos.Add(new AnalogTestInfo(point));
            }
        }

        public int Count() { return analogTestInfos.Count; }
        public void Reset()
        {
            position = 0;
        }
        public AnalogTestInfo Current()
        {
            return analogTestInfos[position];
        }

        public void MoveNext()
        {
            ++position;
        }

        private int position = 0;
        private List<AnalogTestInfo> analogTestInfos = new List<AnalogTestInfo>();
        private PCDReader reader;
    }
}
