namespace DTMUtil
{
    public struct SourceInfo
    {
        public string nodeName;
        public string source;
        public string model;
        public string network;
        public string routingAdd;
        public string xmlns;

        override public string ToString()
        {
            return $"nodeName: {nodeName}\n\tsource: {source}\n\tmodel: {model}\n\tnetwork: {network}\n\troutingAdd: {routingAdd}\n\txmlns: {xmlns}";
        }
    }
}
