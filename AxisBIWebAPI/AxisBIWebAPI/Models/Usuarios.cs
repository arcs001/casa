using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserWebAPI.Models
{
    public class Usuarios : TableEntity
    {
        public Usuarios()
        {
            this.FiscalPeriod = -1;
            this.Scorecard = -1;
        }
        public Usuarios(string PartitionKey, string RowKey) : this()

        {
            this.PartitionKey = PartitionKey;
            this.RowKey = RowKey;
        }

        public string Nome { get; set; }
        public string Email { get; set; }
        public int AliasEmployeeId
        {
            get
            {
                int rowKey;
                int.TryParse(this.RowKey, out rowKey);
                return rowKey;
            }
        }
        public string TFSUsername
        {
            get
            {
                return string.Format("{0} <{1}>", this.Nome, this.Email);
            }
        }
        public int FiscalPeriod { get; set; }
        public int Scorecard { get; set; }

        public void Merge(Models.Usuarios from)
        {
            if (!string.IsNullOrEmpty(from.Nome) && !this.Nome.Equals(from.Nome, StringComparison.Ordinal))
                this.Nome = from.Nome;

            if (!string.IsNullOrEmpty(from.Email) && !this.Email.Equals(from.Email, StringComparison.Ordinal))
                this.Email = from.Email;

            if (!string.IsNullOrEmpty(from.ETag) && !this.Email.Equals(from.ETag, StringComparison.Ordinal))
                this.ETag = from.ETag;

            if (from.FiscalPeriod > 0 && !this.FiscalPeriod.Equals(from.FiscalPeriod))
                this.FiscalPeriod = from.FiscalPeriod;

            if (from.Scorecard > 0 && !this.Scorecard.Equals(from.Scorecard))
                this.Scorecard = from.Scorecard;
        }
    }
}