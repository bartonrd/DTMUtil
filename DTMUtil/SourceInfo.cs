namespace DTMUtil
{
    public struct SourceInfo
    {
        // Core
        public string nodeName;
        public string source;
        public string model;
        public string network;
        public string routingAdd;
        public string xmlns;

        // Additional
        public string ansiDevs;
        public string rackPosNo;
        public string pod;
        public string ncs;
        public string logic;
        public string flow;
        public string operatorTag;

    
        override public string ToString()
        {
            return $"nodeName: {nodeName}\n\tsource: {source}\n\tmodel: {model}\n\tnetwork: {network}\n\troutingAdd: {routingAdd}\n\txmlns: {xmlns}";
        }
    }
}
