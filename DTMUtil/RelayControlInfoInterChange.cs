using System.Collections.Generic;


namespace DTMUtil
{
    public class RelayControlInfoInterChange
    {
        public RelayControlInfoInterChange(object[] objs)
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
            var points = reader.GetCallbackPoints(devicesToSkip);
            foreach (var point in points)
            {
                relayControlInfo.Add(new RelayControlInfo(point));
            }
        }

        public int Count() { return relayControlInfo.Count; }
        public void Reset()
        {
            position = 0;
        }
        public RelayControlInfo Current()
        {
            return relayControlInfo[position];
        }

        public void MoveNext()
        {
            ++position;
        }

        private int position = 0;
        private List<RelayControlInfo> relayControlInfo = new List<RelayControlInfo>();
        private PCDReader reader;
    }
}
