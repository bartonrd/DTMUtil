using System.Xml;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DTMUtil
{
    public class PCDReader
    {
        public PCDReader()
        {

        }
        public PCDReader(string filePath)
        {
            LoadFile(filePath);
        }
        public void LoadFile(string filePath)
        {
            // load the PCD file
            xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.Load(filePath);
            nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsMgr.AddNamespace("ns", @"http://www.sce.com/SEMT/61850/PCL");
        }
        public List<Point> GetControlPointTree(string[] skipTheseDevices)
        {
            List<Point> points = new List<Point>();
            List<StatusControlPoint> statusControlPoints = new List<StatusControlPoint>();

            var pcdPoints = xmlDoc.SelectNodes("ns:PCL/Substation/IED/Point", nsMgr);
            if (pcdPoints == null)
                return points;

            foreach (XmlNode point in pcdPoints)
            {
                string device;
                try
                {
                    // Try to get the device name
                    device = point.ParentNode.Attributes["name"].Value;
                }
                catch
                {
                    // If we are here the PCD is malformed for this device
                    continue;
                }
                // If this device is one we should skip - skip it
                if (skipTheseDevices != null && skipTheseDevices.Contains(device))
                    continue;
                // Get the point type, for this method we only want status and control points
                if (point.Attributes["name"].Value.Contains("SETTINGS"))
                {
                    //Console.WriteLine(point.Attributes["name"].Value);
                }
                var i = Point.PointType(point);

                if (i == "StatusControl")
                {
                    try
                    {
                        points.Add(new StatusControlPoint(point));
                    }
                    catch
                    {
                        // We've encountered an improper point
                        continue;
                    }
                }
                else if(i == "Control")
                {
                    try
                    {
                        points.Add(new ControlPoint(point));
                    }
                    catch
                    {
                        // We've encountered an improper point
                        continue;
                    }
                }
            }

            return points;
        }

        public List<Point> GetStatusPointTree(string[] skipTheseDevices)
        {
            List<Point> points = new List<Point>();

            // this method can't throw an exception, unless it has a bad xPath argument
            // it will return null if there are no matching nodes
            var pcdPoints = xmlDoc.SelectNodes("ns:PCL/Substation/IED/Point", nsMgr);

            if (pcdPoints == null)
                return points;

            foreach (XmlNode point in pcdPoints)
            {
                string device;
                try
                {
                    // Try to get the device name
                    device = point.ParentNode.Attributes["name"].Value;
                }
                catch
                {
                    // If we are here the PCD is malformed for this device
                    continue;
                }
                // If this device is one we should skip - skip it
                if (skipTheseDevices != null && skipTheseDevices.Contains(device))
                    continue;
                var i = Point.PointType(point);
                try
                {
                    switch (i)
                    {
                        case "Master":
                            // skip
                            break;
                        case "Analog":
                            // skip
                            break;
                        case "Status":
                            points.Add(new StatusPoint(point));
                            break;
                        case "StatusControl":
                            points.Add(new StatusControlPoint(point));
                            break;
                        case "Control":
                            // skip
                            break;
                        default:
                            throw new XMLReaderException("Unknown Point Type");
                    }
                }
                catch
                {
                    // If we are here either a points constructor failed or it has an abnormal type - pass a message up but continue to the next point
                    continue;
                }
            }
            return points;
        }
        public List<Point> GetAnalogPointTree(string[] skipTheseDevices)
        {
            List<Point> points = new List<Point>();

            var pcdPoints = xmlDoc.SelectNodes("ns:PCL/Substation/IED/Point", nsMgr);
            if (pcdPoints == null)
                return points;

            foreach (XmlNode point in pcdPoints)
            {
                string device;
                try
                {
                    // Try to get the device name
                    device = point.ParentNode.Attributes["name"].Value;
                }
                catch
                {
                    // If we are here the PCD is malformed for this device
                    continue;
                }
                // If this device is one we should skip - skip it
                if (skipTheseDevices != null && skipTheseDevices.Contains(device))
                    continue;
                var i = Point.PointType(point);
                try
                {
                    switch (i)
                    {
                        case "Master":
                            // skip
                            break;
                        case "Analog":
                            points.Add(new AnalogPoint(point));
                            break;
                        case "Status":
                            // skip status points
                            break;
                        case "StatusControl":
                            // skip status and control points
                            break;
                        case "Control":
                            // skip
                            break;
                        default:
                            throw new XMLReaderException("Unknown Point Type");
                    }
                }
                catch
                {
                    continue;
                }
            }

            return points;
        }
        public List<Point> GetCallbackPoints(string[] skipTheseDevices)
        {
            List<Point> points = new List<Point>();
            List<StatusControlPoint> statusControlPoints = new List<StatusControlPoint>();
            List<ControlPoint> controlPoints = new List<ControlPoint>();
            List<StatusPoint> statusPoints = new List<StatusPoint>();

            var pcdPoints = xmlDoc.SelectNodes("ns:PCL/Substation/IED/Point", nsMgr);
            if (pcdPoints == null)
                return points;

            foreach (XmlNode point in pcdPoints)
            {
                string device;
                try
                {
                    // Try to get the device name
                    device = point.ParentNode.Attributes["name"].Value;
                }
                catch
                {
                    // If we are here the PCD is malformed for this device
                    continue;
                }
                // If this device is one we should skip - skip it
                if (skipTheseDevices != null && skipTheseDevices.Contains(device))
                    continue;
                var i = Point.PointType(point);
                try
                {
                    switch (i)
                    {
                        case "Master":
                            // skip
                            break;
                        case "Analog":
                            // skip
                            break;
                        case "Status":
                            statusPoints.Add(new StatusPoint(point));
                            break;
                        case "StatusControl":
                            statusControlPoints.Add(new StatusControlPoint(point));
                            break;
                        case "Control":
                            controlPoints.Add(new ControlPoint(point));
                            break;
                        default:
                            throw new XMLReaderException("Unknown Point Type");
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (controlPoints != null)
                foreach (ControlPoint pnt in controlPoints)
                {
                    PointRule rl;
                    if ((rl = HasARule(pnt)) != null)
                    {
                        pnt.Ctl.CtlVal = rl.control_type;
                        pnt.points = new List<Point>();
                        pnt.points.AddRange(statusPoints.FindAll(x =>
                        {
                            return rl.status(x) && (x.Source == pnt.Source || rl.lookOutside);
                        }));
                        statusPoints.RemoveAll(x =>
                        {
                            return rl.status(x) && (x.Source == pnt.Source || rl.lookOutside);
                        });
                    }
                }
            points.AddRange(controlPoints.Cast<Point>().ToList());
            points.AddRange(statusControlPoints.Cast<Point>().ToList());
            return points;
        }

        private PointRule HasARule(ControlPoint pnt)
        {
            if (rules == null)
                return null;
            foreach (PointRule rl in rules)
            {
                if (pnt.Name.Contains(rl.control))
                {
                    return rl;
                }
            }
            return null;
        }

        PointRule[] rules = new PointRule[] { new PointRule("RESET TGTS", false, x => x.Name.Contains("TRGT"), "reset"),
            new PointRule("L/O RSET",false, x => x.Name.Contains("L/O"), "reset"), new PointRule("SETTINGS", false, x => x.Name.Contains("SET GRP") , "barrel"),
            new PointRule("TEST SW", false, x => x.Name.Contains("TEST") || x.Name.Contains("PERM"), "barrel")
            ,new PointRule("SP RESET", false, x => x.Name.Contains("SP TRIP / LO"), "reset")
        };
        XmlDocument xmlDoc = null;
        XmlNamespaceManager nsMgr = null;
    }

    public class PointRule
    {
        public PointRule() { }
        public PointRule(string control, bool lookOutside, Predicate<Point> status, string control_type)
        {
            this.control = control;
            this.lookOutside = lookOutside;
            this.status = status;
            this.control_type = control_type;
        }

        public string control_type;
        public string control;
        public Predicate<Point> status;
        public bool lookOutside;
    }
}
