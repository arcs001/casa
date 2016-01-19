using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxisBIWebAPI
{
    public class WorkItem
    {
        public WorkItem()
        {
        }

        public WorkItem(decimal remainingWork)
        {
            fieldsOut = new List<Field>();
            fieldsOut.Add(new Field() { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork", value = remainingWork.ToString() });
        }

        public WorkItem(string ross, DateTime date, decimal hours, string user)
        {

            fieldsOut = new List<Field>();

            fieldsOut.Add(new Field() { op = "add", path = "/fields/System.Title", value = string.Format("{0} - {1}", ross, date.ToString("dd/MM/yy")) });
            fieldsOut.Add(new Field() { op = "add", path = "/fields/System.AssignedTo", value = user });
            fieldsOut.Add(new Field() { op = "add", path = "/fields/System.Description", value = string.Format("{0} Burned ", hours) });
        }


        public int id { get; set; }
        public int rev { get; set; }
        public IList<Field> fieldsOut { get; set; }
        public IDictionary<string, object> fields { get; set; }
        public string url { get; set; }

       



        internal decimal GetRemainingWork()
        {
            return decimal.Parse(fields["Microsoft.VSTS.Scheduling.RemainingWork"].ToString());
            
        }
    }


    public class Field
    {
        public string op { get; set; }
        public string path { get; set; }
        public string value { get; set; }
    }
}
