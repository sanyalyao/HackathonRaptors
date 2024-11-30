﻿using NLog;

namespace QAHackathon.Core.LoggingLogic
{
    public class LoggingBL
    {
        public static Guid Guid { get; private set; }

        private static LoggingBL instance;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly object locker = new object();
        private LoggingBL() { }

        public static LoggingBL Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new LoggingBL();
                    }
                    return instance;
                }
            }
        }

        public void Info(string text) => logger.Info(text);
        public void Error(Exception exception) => logger.Info(exception);
        public void Error(string text) => logger.Error(text);
        public void Trace(string text) => logger.Trace(text);

        public void GenerateGuid()
        {
            Guid = Guid.NewGuid();
        }
    }
}