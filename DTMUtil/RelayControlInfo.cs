using System.Collections.Generic;

namespace DTMUtil
{
    public class RelayControlInfo
    {
        public RelayControlInfo() { }
        public RelayControlInfo(Point point)
        {
            var pointType = point.GetPointType();
            switch (pointType)
            {
                case "StatusControl":
                    control_path = DTMPath.GetDTMPath(point.Source, point.Ctl.CtlPath);
                    control_type = point.Ctl.CtlVal;
                    control_normal = point.Norm;
                    status_paths = new string[1] { DTMPath.GetDTMPath(point.Source, point.Stat.StatPath) };
                    status_normal = new int[1] { point.Norm };
                    break;
                case "Control":
                    control_path = DTMPath.GetDTMPath(point.Source, point.Ctl.CtlPath);
                    control_type = point.Ctl.CtlVal;
                    control_normal = point.Norm;
                    if (point.points == null)
                        break;
                    List<string> paths = new List<string>();
                    List<int> norms = new List<int>();
                    foreach (var i in point.points)
                    {
                        paths.Add(DTMPath.GetDTMPath(i.Source, i.Stat.StatPath));
                        norms.Add(i.Norm);
                    }
                    status_paths = paths.ToArray();
                    status_normal = norms.ToArray();
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            string str = $"Control: {control_path}, Type: {control_type}, Control Norm: {control_normal}";
            if (status_paths != null)
            {
                for (int i = 0; i < status_paths.Length; i++)
                {
                    str += "\n\t" + status_paths[i] + ", Norm: " + status_normal[i];
                }
            }
            return str;
        }

        public string control_path, control_type;
        public int control_normal;
        public string[] status_paths = null;
        public int[] status_normal = null;
    }
}
