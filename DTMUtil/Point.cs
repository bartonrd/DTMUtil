using System.Collections.Generic;
using System.Xml;

namespace DTMUtil
{
    public class Control
    {
        public Control()
        {
            CtlVal = "";
            CtlPath = "";
            EmsID = -1;
            MasterID = -1;
        }
        public Control(XmlNode ODA)
        {
            Initialize(ODA);
        }
        public void Initialize(XmlNode ODA)
        {
            // For a control - a point must have a 61850 path, a control value
            // It might have a emsID and masterID but does not need to

            // Must have these qualities
            string ldInst, prefix, lnClass, lnInst, ctl0;
            ldInst = ODA.Attributes["ldInst"].Value;
            prefix = ODA.Attributes["prefix"].Value;
            lnClass = ODA.Attributes["lnClass"].Value;
            lnInst = ODA.Attributes["lnInst"].Value;
            ctl0 = ODA.Attributes["Ctl_0"].Value;
            CtlPath = $"{ldInst}/{prefix}/{lnClass}/{lnInst}:{ctl0}";
            CtlVal = ODA.Attributes["ctlVal"].Value;

            // Might have these qualities
            EmsID = ODA.Attributes["emsID"] == null ? -1 : int.Parse(ODA.Attributes["emsID"].Value);
            MasterID = ODA.Attributes["masterID"] == null ? -1 : int.Parse(ODA.Attributes["masterID"].Value);
        }
        public string CtlVal { get; set; }
        public string CtlPath { get; set; }
        public int EmsID { get; set; }
        public int MasterID { get; set; }
    }
    public class Status
    {
        public Status()
        {
            StatPath = "";
            EmsID = -1;
        }
        public Status(XmlNode ODA)
        {
            Initialize(ODA);
        }
        public void Initialize(XmlNode ODA)
        {
            // A status must have a 61850 path
            string ldInst, prefix, lnClass, lnInst, ending;
            ldInst = ODA.Attributes["ldInst"].Value;
            prefix = ODA.Attributes["prefix"].Value;
            lnClass = ODA.Attributes["lnClass"].Value;
            lnInst = ODA.Attributes["lnInst"].Value;
            ending = ODA.Attributes["daName"].Value;
            StatPath = $"{ldInst}/{prefix}/{lnClass}/{lnInst}:{ending}";

            // It may have an emsID
            EmsID = ODA.Attributes["emsID"] == null ? -1 : int.Parse(ODA.Attributes["emsID"].Value);
        }


        public string StatPath { get; set; }
        public int EmsID { get; set; }
    }
    public class Analog : Status
    {
        public Analog() : base() { }
        public Analog(XmlNode ODA) : base(ODA) { }
    }
    public abstract class Point
    {
        public Point()
        {
            Item = -1;
            Name = "";
            Source = "";
            points = null;
            Summary = -1;
            Norm = 0;
            Anal = null;
            Ctl = null;
            Stat = null;
        }
        public Point(XmlNode point)
        {
            Initialize(point);
        }
        public void Initialize(XmlNode point)
        {
            // A point must have an Item and a Name
            // It also must have a source, but that is in its parent node
            Item = int.Parse(point.Attributes["item"].Value);
            Name = point.Attributes["name"].Value;
            Source = point.ParentNode.Attributes["name"].Value;

            // Points is a list of other nodes that might be related to this point. An example relation would be a control point holding the status points it operates on.
            points = null;

            // A point might have a summaryID and a Norm.
            Summary = point.Attributes["sumItem"] == null ? -1 : int.Parse(point.Attributes["sumItem"].Value);
            // If it doesnt have a norm we assign the norm 0 instead. It's possible we should consider this an error and enforce that all points must have a norm.
            Norm = point.Attributes["norm"] == null ? 0 : int.Parse(point.Attributes["norm"].Value);

            // A Point might contain a status, analog, or control point.
            // It should only contain up to one of each
            // The select single node method will not catch this and just return the first instance should there be two or more status ODA's for some reason.
            try
            {
                // Its the constructor method that throws.
                // SelectSingleNode should never throw.
                Anal = new Analog(point.SelectSingleNode("ODA[@type=\"mval\"]"));
            }
            catch
            {
                Anal = null;
            }
            try
            {
                Ctl = new Control(point.SelectSingleNode("ODA[@type=\"oper\"]"));
            }
            catch
            {
                Ctl = null;
            }
            try
            {
                Stat = new Status(point.SelectSingleNode("ODA[@type=\"stat\"]"));
            }
            catch
            {
                Stat = null;
            }
        }
        public abstract string GetPointType();

        public static string PointType(XmlNode point)
        {
            XmlNode operODA = point.SelectSingleNode("ODA[@type=\"oper\"]");
            XmlNode statODA = point.SelectSingleNode("ODA[@type=\"stat\"]");
            XmlNode mvalODA = point.SelectSingleNode("ODA[@type=\"mval\"]");
            bool stat = statODA != null;
            bool oper = operODA != null;
            bool mval = mvalODA != null;

            if (mval)
            {
                // need to add analog summary later
                return "Analog";
            }
            if (oper && stat)
            {
                return "StatusControl";
            }
            if (oper)
            {
                try
                {
                    return operODA.Attributes["Ctl_0"].Value == "MASTER" ? "Master" : "Control";
                }
                catch
                {
                    return "";
                }

            }
            if (stat)
                return "Status";
            return "";
        }
        public override abstract string ToString();

        public string Source { get; set; }
        public string Name { get; set; }

        public int Item { get; set; }
        public int Norm { get; set; }
        public int Summary { get; set; }
        public Status Stat { get; set; }
        public Control Ctl { get; set; }
        public Analog Anal { get; set; }
        public List<Point> points { get; set; }
        public string[] CtlPath { get; set; }
    }
    public class AnalogPoint : Point
    {
        public AnalogPoint() { }
        public AnalogPoint(XmlNode point) : base(point)
        {
            // An Analog point must have a unit, lowlimit, highlimit, and dnp multiplier
            Unit = point.Attributes["unit"].Value;
            LowLimit = float.Parse(point.Attributes["lowLim"].Value);
            HighLimit = float.Parse(point.Attributes["highLim"].Value);
            Multi = float.Parse(point.Attributes["mult"].Value);
        }
        public override string GetPointType()
        {
            return "Analog";
        }
        public override string ToString()
        {
            return "";
        }
        public string Unit { get; set; }
        public float LowLimit { get; set; }
        public float HighLimit { get; set; }
        public float Multi { get; set; }

    }
    public abstract class DigitalPoint : Point
    {
        public DigitalPoint() : base() { }
        public DigitalPoint(XmlNode point) : base(point)
        {
            // A digital point may have a description of 0 and/or 1
            Desc_0 = point.Attributes["desc_0"] == null ? "" : point.Attributes["desc_0"].Value;
            Desc_1 = point.Attributes["desc_1"] == null ? "" : point.Attributes["desc_1"].Value;
        }

        public string Desc_0 { get; set; }
        public string Desc_1 { get; set; }
    }
    public class MasterPoint : DigitalPoint
    {
        public MasterPoint() : base() { }
        public MasterPoint(XmlNode point) : base(point)
        {

        }
        public override string GetPointType()
        {
            return "Master";
        }
        public override string ToString()
        {
            string str = $"Source: {Source}, Name: {Name}, Control Path: {Ctl.CtlPath}";
            if (points == null)
                return str;
            foreach (var i in points)
            {
                str += "\n\t" + i.ToString();
            }
            return str;
        }
    }
    public class StatusControlPoint : DigitalPoint
    {
        public StatusControlPoint() : base() { }
        public StatusControlPoint(XmlNode point) : base(point)
        {

        }

        public override string GetPointType()
        {
            return "StatusControl";
        }
        public override string ToString()
        {
            string str = $"Source: {Source}, Name: {Name}, Control Path: {Ctl.CtlPath}, Control Type: {Ctl.CtlVal}, Status Path: {Stat.StatPath}";
            return str;
        }
    }
    public class StatusPoint : DigitalPoint
    {
        public StatusPoint() : base() { }
        public StatusPoint(XmlNode point) : base(point)
        {

        }
        public override string GetPointType()
        {
            return "Status";
        }
        public override string ToString()
        {
            string str = $"Source: {Source}, Name: {Name}, Status Path: {Stat.StatPath}";
            return str;
        }
    }
    public class ControlPoint : DigitalPoint
    {
        public ControlPoint() : base() { }
        public ControlPoint(XmlNode point) : base(point)
        {

        }
        public override string GetPointType()
        {
            return "Control";
        }
        public override string ToString()
        {
            string str = $"Source: {Source}, Name: {Name}, Control Path: {Ctl.CtlPath}, Control Type: {Ctl.CtlVal}";
            if (points == null)
                return str;
            foreach (var i in points)
            {
                str += "\n\t" + i.ToString();
            }
            return str;
        }
    }

    public class ControlDigital
    {
        public ControlDigital() { }
        public ControlDigital(XmlNode ODA)
        {
            Initialize(ODA);
        }
        public void Initialize(XmlNode ODA)
        {
            string ctl0 = "", ctl1 = "";
            string ldInst, prefix, lnClass, lnInst;
            ldInst = ODA.Attributes["ldInst"].Value;
            prefix = ODA.Attributes["prefix"].Value;
            lnClass = ODA.Attributes["lnClass"].Value;
            lnInst = ODA.Attributes["lnInst"].Value;
            if (ODA.Attributes["Ctl_0"] != null)
                ctl0 = $"{ldInst}/{prefix}/{lnClass}/{lnInst}:{ODA.Attributes["Ctl_0"].Value}";
            if (ODA.Attributes["Ctl_1"] != null)
                ctl1 = $"{ldInst}/{prefix}/{lnClass}/{lnInst}:{ODA.Attributes["Ctl_1"].Value}";
            CtlPath = new string[2] { ctl0, ctl1 };
            CtlVal = ODA.Attributes["ctlVal"].Value;
            if (ODA.Attributes["emsID"] != null)
                EmsID = int.Parse(ODA.Attributes["emsID"].Value);
            else
                EmsID = -1;
            MasterID = ODA.Attributes["masterID"] == null ? -1 : int.Parse(ODA.Attributes["masterID"].Value);
        }
        public string CtlVal { get; set; }
        public string[] CtlPath { get; set; }
        public int EmsID { get; set; }
        public int MasterID { get; set; }
    }




}