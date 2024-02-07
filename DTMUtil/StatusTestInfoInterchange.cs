
using System.Collections.Generic;


namespace DTMUtil
{
    public class StatusTestInfoInterChange
    {
        public StatusTestInfoInterChange(object[] objs)
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
            var points = reader.GetStatusPointTree(devicesToSkip);
            foreach (var point in points)
            {
                statusTestInfos.Add(new StatusTestInfo(point));
            }
        }

        public int Count() { return statusTestInfos.Count; }
        public void Reset()
        {
            position = 0;
        }
        public StatusTestInfo Current()
        {
            return statusTestInfos[position];
        }

        public void MoveNext()
        {
            ++position;
        }

        private int position = 0;
        private List<StatusTestInfo> statusTestInfos = new List<StatusTestInfo>();
        private PCDReader reader;
    }
}
