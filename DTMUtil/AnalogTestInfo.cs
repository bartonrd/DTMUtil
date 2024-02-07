using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTMUtil
{
    public struct AnalogTestInfo
    {
        public AnalogTestInfo(Point point)
        {
            source = point.Source;
            name = point.Name;
            units = ((AnalogPoint)point).Unit;
            statPath = point.Anal.StatPath;
            lowLim = ((AnalogPoint)point).LowLimit;
            highLim = ((AnalogPoint)point).HighLimit;
            dnpMulti = ((AnalogPoint)point).Multi;
            emsID = point.Anal.EmsID;
        }

        public override string ToString()
        {
            return $"Source: {source}, Name: {name}, units: {units}, statPath: {statPath}\n\tlowLim: {lowLim}, highLim: {highLim}, dnpMulti: {dnpMulti}, emsID: {emsID}";
        }

        public string source, name, units, statPath;
        public float lowLim, highLim, dnpMulti;
        public int emsID;
    }
}
