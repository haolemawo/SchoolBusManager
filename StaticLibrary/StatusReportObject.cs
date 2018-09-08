using System;

namespace WBPlatform.StatusReport
{
    public class StatusReportObject : IDisposable
    {
        public DateTime ReportTime { get; set; }
        public int SessionsCount { get; set; }
        public bool SessionThread { get; set; }
        public int Tokens { get; set; }
        public bool WeChatRCVDThreadStatus { get; set; }
        public bool WeChatSENTThreadStatus { get; set; }
        public int WeChatRCVDListCount { get; set; }
        public int WeChatSENTListCount { get; set; }
        public bool Database { get; set; }
        public bool CoreMessageSystemThread { get; set; }
        public int CoreMessageSystemCount { get; set; }
        public bool MessageBackupThread { get; set; }
        public int MessageBackupCount { get; set; }
        public string StartupTime { get; set; }
        public string ServerVer { get; set; }
        public string CoreLibVer { get; set; }
        public string NetCoreCLRVer { get; set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StatusReportObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}