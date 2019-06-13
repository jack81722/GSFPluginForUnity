using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace GameSystem.GameCore.Debugger
{
    /// <summary>
    /// Default debugger by using console
    /// </summary>
    public class ConsoleDebugger : IDebugger
    {
        #region Time setting properties
        /// <summary>
        /// Turn On/Off showing time
        /// </summary>
        public bool ShowTime { get; set; } = true;
        /// <summary>
        /// Time string format
        /// </summary>
        public string TimeFormat = "yyyy/MM/dd HH:MM:ss";
        #endregion

        #region Font setting properties
        public ConsoleColor NormalColor { get; set; } = ConsoleColor.White;
        public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;
        public ConsoleColor WarningColor { get; set; } = ConsoleColor.Yellow;
        #endregion

        private bool queueable;
        private Timer printTimer;
        private List<TypedMsg> messages;

        #region Inner data structure
        /// <summary>
        /// Temporary class of saved message
        /// </summary>
        private class TypedMsg
        {
            public MsgType type;
            public string msg;
        }

        /// <summary>
        /// Type of saved message
        /// </summary>
        private enum MsgType : byte
        {
            Normal,
            Error,
            Warning
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor with print period
        /// </summary>
        /// <param name="period">period of printing</param>
        public ConsoleDebugger(float period = 1f)
        {
            messages = new List<TypedMsg>();
            if (period > 0)
            {
                queueable = true;
                printTimer = new Timer(period);
                printTimer.Elapsed += PrintTimer_Elapsed;
                printTimer.Start();
            }
            else
            {
                queueable = false;
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Process print method in period
        /// </summary>
        private void PrintTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (messages)
            {
                if (messages.Count <= 0)
                    return;
                MsgType type = MsgType.Normal;
                Console.ForegroundColor = getColor(type);
                // print all of messages
                for (int i = 0; i < messages.Count; i++)
                {
                    if (type != messages[i].type)
                    {
                        Console.ForegroundColor = getColor(messages[i].type);
                        type = messages[i].type;
                    }
                    Console.WriteLine(messages[i].msg);
                }
                // clear temporary saved messages
                messages.Clear();
            }
        }

        /// <summary>
        /// Build message by object
        /// </summary>
        private string buildMsg(object obj)
        {
            if (ShowTime)
                return string.Format("[{0}]: {1}", DateTime.Now.ToString(TimeFormat), obj);
            else
                return obj.ToString();
        }

        /// <summary>
        /// Print message by type
        /// </summary>
        private void print(MsgType type, string msg)
        {
            Console.ForegroundColor = getColor(type);
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Queue message by type
        /// </summary>
        private void queue(MsgType type, string msg)
        {
            lock (messages)
            {
                TypedMsg tMsg = new TypedMsg() { type = type, msg = msg };
                messages.Add(tMsg);
            }
        }

        /// <summary>
        /// Get correspond color by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ConsoleColor getColor(MsgType type)
        {
            switch (type)
            {
                case MsgType.Normal:
                    return NormalColor;
                case MsgType.Error:
                    return ErrorColor;
                case MsgType.Warning:
                    return WarningColor;
                default:
                    return NormalColor;
            }
        }
        #endregion

        public void Close()
        {
            printTimer.Stop();
            printTimer.Close();
            messages.Clear();
        }

        #region IDebugger methods
        public void Log(object obj)
        {
            if (queueable)
                queue(MsgType.Normal, buildMsg(obj));
            else
                print(MsgType.Normal, buildMsg(obj));
        }

        public void LogError(object obj)
        {
            if (queueable)
                queue(MsgType.Error, buildMsg(obj));
            else
                print(MsgType.Error, buildMsg(obj));
        }

        public void LogWarning(object obj)
        {
            if (queueable)
                queue(MsgType.Warning, buildMsg(obj));
            else
                print(MsgType.Warning, buildMsg(obj));
        }
        #endregion

    }
}
