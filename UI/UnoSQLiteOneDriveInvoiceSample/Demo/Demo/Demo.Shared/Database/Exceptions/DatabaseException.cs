using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Database.Exceptions
{
    public class DatabaseException : Exception
    {
        #region Properties

        public string PropertyName { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }

        #endregion

        #region Method(s)

        public DatabaseException(string typeName, string methodName, string description) : base(string.Format($"SoloDatabaseException : TYPE = {typeName}, METHOD = {methodName}, DESCRIPTION = {description}")) { }

        public override string ToString()
        {
            return base.ToString();
        }
        #endregion

    }
}
