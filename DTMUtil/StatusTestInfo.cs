using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DTMUtil
{
    public class StatusTestInfo
    {
        public StatusTestInfo() { }
        public StatusTestInfo(Point point)
        {
            InitializeTest(point);
        }

        public void InitializeTest(Point point)
        {
            normal = point.Norm;
            source = point.Source;
            name = point.Name;
            statPath = point.Stat.StatPath;
            desc_0 = ((DigitalPoint)point).Desc_0;
            desc_1 = ((DigitalPoint)point).Desc_1;
            item = point.Item;
            this.summary = point.Summary;
            this.emsID = point.Stat.EmsID;
        }

        public override string ToString()
        {
            return $"Source: {source}, Name: {name}, statPath: {statPath}, Summary: {summary}, Item: {item}, emsID: {emsID}";
        }

        public string source, name, statPath, desc_0, desc_1;
        public int item, emsID, summary, normal;
    }
}
