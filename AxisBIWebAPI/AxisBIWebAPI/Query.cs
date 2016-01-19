using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxisBIWebAPI.Query
{
    public class Column
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Field
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class SortColumn
    {
        public Field field { get; set; }
        public bool descending { get; set; }
    }

    public class Target
    {
        public int id { get; set; }
        public string url { get; set; }
    }

    public class WorkItemRelation
    {
        public Target target { get; set; }
    }

    public class WorkItem
    {
        public string queryType { get; set; }
        public string asOf { get; set; }
        public List<Column> columns { get; set; }
        public List<SortColumn> sortColumns { get; set; }
        public List<WorkItemRelation> workItemRelations { get; set; }
    }
}
