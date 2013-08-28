/*******************************************************************************************************************************
 * AK.Commons.Providers.Azure.Logging.LogEntryEntity
 * Copyright © 2013 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Library Provider Set (AKCLPS).
 *  
 * AKCLPS is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCLPS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCLPS.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using AK.Commons.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;

#endregion

namespace AK.Commons.Providers.Azure.Logging
{
    /// <summary>
    /// Table Entity used to store log entries.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class LogEntryEntity : TableEntity
    {
        private readonly LogEntry logEntry;

        public LogEntryEntity() {}

        public LogEntryEntity(LogEntry logEntry, string partitionKey)
        {
            this.logEntry = logEntry;
            this.PartitionKey = partitionKey;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public string ApplicationName 
        { 
            get { return this.logEntry.ApplicationName; }
            set { this.logEntry.ApplicationName = value; }
        }

        public DateTime TimeStamp
        {
            get { return this.logEntry.TimeStamp; }
            set { this.logEntry.TimeStamp = value; }
        }

        public string LogLevel
        {
            get { return this.logEntry.LogLevel.ToString(); }
            set { this.logEntry.LogLevel = (LogLevel) Enum.Parse(typeof (LogLevel), value); }
        }

        public string CallingMethod
        {
            get { return this.logEntry.CallingMethod; }
            set { this.logEntry.CallingMethod = value; }
        }

        public string Message
        {
            get { return this.logEntry.Message; }
            set { this.logEntry.Message = value; }
        }
    }
}