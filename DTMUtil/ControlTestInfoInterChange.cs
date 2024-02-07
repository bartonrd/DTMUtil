using System.Collections.Generic;

namespace DTMUtil
{
    public class ControlTestInfoInterChange
    {
        public ControlTestInfoInterChange(object[] objs)
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
            var points = reader.GetControlPointTree(devicesToSkip);
            foreach (var point in points)
            {
                controlTestInfos.Add(new ControlTestInfo(point));
            }
        }

        public int Count() { return controlTestInfos.Count; }
        public void Reset()
        {
            position = 0;
        }
        public ControlTestInfo Current()
        {
            return controlTestInfos[position];
        }

        public void MoveNext()
        {
            ++position;
        }

        private int position = 0;
        private List<ControlTestInfo> controlTestInfos = new List<ControlTestInfo>();
        private PCDReader reader;

    }
}
