using System;
using System.Collections.Generic;


namespace DTMUtil
{
    public struct ControlTestInfo
    {
        public string control_source, control_name, control_ctlPath, control_desc_0, control_desc_1, control_type;
        public int control_item, control_emsID, length, statlength, control_norm;
        public int status_emsID;
        public int[] statNorms;
        public string[] target_source, target_name, target_ctlPath, target_desc_0, target_desc_1, target_statPaths;
        public int[] target_item;
        public ControlTestInfo(Point point)
        {
            control_norm = point.Norm;
            status_emsID = -1;
            switch (point.GetPointType())
            {
                case "Control":
                    control_source = ((ControlPoint)point).Source;
                    control_desc_0 = ((ControlPoint)point).Desc_0;
                    control_desc_1 = ((ControlPoint)point).Desc_1;
                    control_ctlPath = ((ControlPoint)point).Ctl.CtlPath;
                    control_emsID = ((ControlPoint)point).Ctl.EmsID;
                    control_item = ((ControlPoint)point).Item;
                    control_name = ((ControlPoint)point).Name;
                    control_type = point.Ctl.CtlVal;

                    if(point.Stat != null)
                        status_emsID = point.Stat.EmsID;

                    length = 1;

                    target_source = new string[1] { control_source };
                    target_desc_0 = new string[1] { control_desc_0 };
                    target_desc_1 = new string[1] { control_desc_1 };
                    target_ctlPath = new string[1] { control_ctlPath };
                    target_item = new int[1] { control_item };
                    target_name = new string[1] { control_name };
                    List<string> strings = new List<string>();
                    List<int> temp_status_norms = new List<int>();
                    if (point.points != null)
                        foreach (var item in point.points)
                        {
                            strings.Add(DTMPath.GetDTMPath(point.Source, item.Stat.StatPath));
                            temp_status_norms.Add(item.Norm);
                        }
                    statlength = strings.Count;
                    target_statPaths = strings.ToArray();
                    statNorms = temp_status_norms.ToArray();

                    break;
                case "StatusControl":
                    control_source = ((StatusControlPoint)point).Source;
                    control_desc_0 = ((StatusControlPoint)point).Desc_0;
                    control_desc_1 = ((StatusControlPoint)point).Desc_1;
                    control_ctlPath = ((StatusControlPoint)point).Ctl.CtlPath;
                    control_emsID = ((StatusControlPoint)point).Ctl.EmsID;
                    control_item = ((StatusControlPoint)point).Item;
                    control_name = ((StatusControlPoint)point).Name;
                    control_type = point.Ctl.CtlVal;

                    if (point.Stat != null)
                        status_emsID = point.Stat.EmsID;

                    length = 1;

                    target_source = new string[1] { control_source };
                    target_desc_0 = new string[1] { control_desc_0 };
                    target_desc_1 = new string[1] { control_desc_1 };
                    target_ctlPath = new string[1] { control_ctlPath };
                    target_item = new int[1] { control_item };
                    target_name = new string[1] { control_name };
                    target_statPaths = new string[1] { DTMPath.GetDTMPath(point.Source, point.Stat.StatPath) };
                    statNorms = new int[1] { point.Norm };
                    statlength = 1;
                    break;
                case "Master":
                    control_source = ((MasterPoint)point).Source;
                    control_desc_0 = ((MasterPoint)point).Desc_0;
                    control_desc_1 = ((MasterPoint)point).Desc_1;
                    control_ctlPath = ((MasterPoint)point).Ctl.CtlPath;
                    control_emsID = ((MasterPoint)point).Ctl.EmsID;
                    control_item = ((MasterPoint)point).Item;
                    control_name = ((MasterPoint)point).Name;

                    if (point.Stat != null)
                        status_emsID = point.Stat.EmsID;

                    List<string> temp_target_ctlPath = new List<string>();
                    List<string> temp_target_desc_0 = new List<string>();
                    List<string> temp_target_desc_1 = new List<string>();
                    List<int> temp_target_item = new List<int>();
                    List<string> temp_target_name = new List<string>();
                    List<string> temp_target_source = new List<string>();
                    List<string> temp_status_paths = new List<string>();
                    List<int> temp_status_normals = new List<int>();

                    control_type = point.points[0].Ctl.CtlVal;
                    if (point.points != null)
                        foreach (ControlPoint item in ((MasterPoint)point).points)
                        {
                            temp_target_source.Add(item.Source);
                            temp_target_name.Add(item.Name);
                            temp_target_desc_0.Add(item.Desc_0);
                            temp_target_desc_1.Add(item.Desc_1);
                            temp_target_item.Add(item.Item);
                            temp_target_ctlPath.Add(item.Ctl.CtlPath);

                            foreach (var st in item.points)
                            {
                                temp_status_paths.Add(DTMPath.GetDTMPath(item.Source, st.Stat.StatPath));
                                temp_status_normals.Add(st.Norm);
                            }
                        }

                    target_ctlPath = temp_target_ctlPath.ToArray();
                    target_desc_0 = temp_target_desc_0.ToArray();
                    target_desc_1 = temp_target_desc_1.ToArray();
                    target_item = temp_target_item.ToArray();
                    target_name = temp_target_name.ToArray();
                    target_source = temp_target_source.ToArray();
                    length = point.points.Count;
                    target_statPaths = temp_status_paths.ToArray();
                    statlength = target_statPaths.Length;
                    statNorms = temp_status_normals.ToArray();
                    break;
                default: throw new XMLReaderException("Unknown Point type");
            }

        }
        public override string ToString()
        {
            string str = $"Source: {control_source}, Name: {control_name}, ctlPath: {control_ctlPath}, Desc_0: {control_desc_0}, Desc_1: {control_desc_1}, Item: {control_item}, EmsID: {control_emsID}";

            for (int i = 0; i < target_source.Length; ++i)
            {
                str += $"\n\tSource: {target_source[i]}, Name: {target_name[i]}, ctlPath: {target_ctlPath[i]}, Desc_0: {target_desc_0[i]}, Desc_1: {target_desc_1[i]}, Item: {target_item[i]}";
            }

            return str;
        }

    }
}
