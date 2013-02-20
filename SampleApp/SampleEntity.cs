using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityInfo.DataEntities.Entities;
using InfinityInfo.DataEntities;

namespace SampleApp
{
    public class SampleEntity : DataEntity
    {
        public SampleEntity() : this(null)
        {

        }

        public SampleEntity(object id) : base("SampleTable", "Id")
        {
            CreateFieldMappings();
        }

        private void CreateFieldMappings()
        {
            this.FieldMappings.Add(new StringDataField(this.EntityTableName, "FirstName"));
            this.FieldMappings.Add(new StringDataField(this.EntityTableName, "LastName"));
            this.FieldMappings.Add(new StringDataField(this.EntityTableName, "AddressLine"));
            this.FieldMappings.Add(new StringDataField(this.EntityTableName, "City"));
            this.FieldMappings.Add(new DateTimeDataField(this.EntityTableName, "CreateDate"));
        }

        public StringDataField FirstName
        {
            get { return (StringDataField)FieldMappings["FirstName"]; }
        }

        public StringDataField LastName
        {
            get { return (StringDataField)FieldMappings["LastName"]; }
        }

        public StringDataField AddressLine
        {
            get { return (StringDataField)FieldMappings["AddressLine"]; }
        }

        public StringDataField City
        {
            get { return (StringDataField)FieldMappings["City"]; }
        }

        public DateTimeDataField CreateDate
        {
            get { return (DateTimeDataField)FieldMappings["CreateDate"]; }
        }
    }
}
