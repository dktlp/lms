using System;

using log4net;
using log4net.Core;

using LMS.Model.Resource;
using LMS.Model.Resource.Logging;

namespace LMS.Service.Logging
{
    public static class AuditLog
    {
        private static readonly ILogger Logger = LogManager.GetLogger("Audit").Logger;
        
        public static void Log(AuditEvent auditEvent)
        {
            LoggingEvent loggingEvent = new LoggingEvent(new LoggingEventData()
            {
                Level = new Level(50000, "AUDIT"),
                TimeStamp = auditEvent.Event.EffectiveTime,
                Message = auditEvent.ToString()
            });

            Logger.Log(loggingEvent);
        }
    }
}