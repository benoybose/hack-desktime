using System;
using System.Text;

namespace DeskTime
{
    public class DdeClient : IDisposable
    {
        private static Win32.DdeDelegate delegateAlive = DdeCallback;

        private uint _pidInst;

        private IntPtr _conversation = IntPtr.Zero;

        private static IntPtr DdeCallback(uint uType, uint uFmt, IntPtr hconv, IntPtr hsz1, IntPtr hsz2, IntPtr hdata, UIntPtr dwData1, UIntPtr dwData2)
        {
            return IntPtr.Zero;
        }

        public DdeClient(string service, string topic)
        {
            IntPtr intPtr = IntPtr.Zero;
            IntPtr intPtr2 = IntPtr.Zero;
            try
            {
                if (Win32.DdeInitialize(ref _pidInst, delegateAlive, 16u, 0u) != 0)
                {
                    throw new Exception("DDE initialisation error.");
                }
                intPtr = Win32.DdeCreateStringHandle(_pidInst, service, 1200);
                if (intPtr == IntPtr.Zero)
                {
                    throw new Exception("DDE error - unable to create string.");
                }
                intPtr2 = Win32.DdeCreateStringHandle(_pidInst, topic, 1200);
                if (intPtr2 == IntPtr.Zero)
                {
                    throw new Exception("DDE error - unable to create string.");
                }
                _conversation = Win32.DdeConnect(_pidInst, intPtr, intPtr2, IntPtr.Zero);
                if (_conversation == IntPtr.Zero)
                {
                    throw new Exception("DDE error - unable to conect to service.");
                }
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                {
                    Win32.DdeFreeStringHandle(_pidInst, intPtr);
                }
                if (intPtr2 != IntPtr.Zero)
                {
                    Win32.DdeFreeStringHandle(_pidInst, intPtr2);
                }
            }
        }

        protected virtual void DoDispose()
        {
            if (_conversation != IntPtr.Zero)
            {
                Win32.DdeDisconnect(_conversation);
            }
            if (_pidInst != 0)
            {
                Win32.DdeUninitialize(_pidInst);
            }
        }

        ~DdeClient()
        {
            DoDispose();
        }

        public void Dispose()
        {
            DoDispose();
            GC.SuppressFinalize(this);
        }

        public string GetData(string item, int timeout)
        {
            IntPtr intPtr = IntPtr.Zero;
            IntPtr intPtr2 = IntPtr.Zero;
            try
            {
                intPtr = Win32.DdeCreateStringHandle(_pidInst, item, 1200);
                if (intPtr == IntPtr.Zero)
                {
                    throw new Exception("DDE error - unable to create string.");
                }
                uint pdwResult = 0u;
                intPtr2 = Win32.DdeClientTransaction(IntPtr.Zero, 0u, _conversation, intPtr, 1u, 8368u, (uint)timeout, ref pdwResult);
                if (intPtr2 == IntPtr.Zero)
                {
                    throw new Exception("DDE error - unable to create transaction.");
                }
                uint num = Win32.DdeGetData(intPtr2, null, 0u, 0u);
                if (num == 0)
                {
                    throw new Exception();
                }
                byte[] array = new byte[num];
                if (Win32.DdeGetData(intPtr2, array, num, 0u) == 0)
                {
                    throw new Exception();
                }
                return Encoding.ASCII.GetString(array);
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                {
                    Win32.DdeFreeStringHandle(_pidInst, intPtr);
                }
                if (intPtr2 != IntPtr.Zero)
                {
                    Win32.DdeFreeDataHandle(intPtr2);
                }
            }
        }
    }
}
